using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Manikin;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace Jutsu
{
    public class WaterCloneJutsu : SpellCastProjectile
    {
        public string waterMaterialAddress = "apoz123.WaterStyle.Material.WaterMaterial";
        public string waterFallAddress = "apoz123Jutsu.WaterStyleJutsu.WaterClone.Effect.WaterFall";
        private CreatureData creatureData;
        private Material material;
        private GameObject waterVFX;
        private Material[] originalMaterials;
        private GameObject sound1;
        private GameObject sound2;
        private bool hit = false;
        /*
         * Used to load all assets for this spell whenever the Catalog is refereshed.
         */
        public override IEnumerator OnCatalogRefreshCoroutine()
        {
            Catalog.LoadAssetAsync<GameObject>(waterFallAddress, gameobject => { waterVFX = gameobject; },
                "WaterFallEffect");
            
            Catalog.LoadAssetAsync<GameObject>("apoz123.WaterStyle.WaterClone.SFX.Spawning",
                gameobject => { sound1 = gameobject; }, "WaterSFX1");
            
            Catalog.LoadAssetAsync<GameObject>("apoz123.WaterStyle.WaterClone.SFX.Splash1", obj => { sound2 = obj;}, "SplashSFX");

            yield return Catalog.LoadAssetCoroutine<Material>(waterMaterialAddress, waterMaterial => { material = waterMaterial; },
                "WaterMaterial");
        }

        /*
         * Functionality when projectile hits the ground, or other objects.
         */
        protected override void OnProjectileCollision(ItemMagicProjectile projectile, CollisionInstance collisionInstance)
        {
            base.OnProjectileCollision(projectile, collisionInstance);
            Vector3 position = collisionInstance.contactPoint;
            
            /*Set all Creature data beforehand that will not cause bug issues*/
            creatureData = Player.currentCreature.data; //Clone data object, so the Player creature data isnt affected. Without a deep copy, it will update the player to act like an npc
            creatureData.containerID = "Empty";
            creatureData.brainId = "HumanMedium"; 
            
            /*spawn logic*/
            creatureData.SpawnAsync(position, 0, null, false, null, creature =>
            {
                GameObject temporary = GameObject.Instantiate(sound1); //Instantiate sfx object
                temporary.transform.position = creature.transform.position; // update position (Spatial audio)
                temporary.GetComponent<AudioSource>().Play(); // Play sfx
                
                creature.OnDamageEvent += OnDamageEvent;
                
                creature.brain.SetState(Brain.State.Idle); // Prevents brain from acting on ragdoll parts, this can be buggy if it does.
                creature.ragdoll.SetState(Ragdoll.State.Disabled); // Disables the mesh, so it cannot be affected by physics.
                
                SetCreatureLooks(creature);
                
                /*Loop over all renderers to update materials*/
                foreach (Creature.RendererData data in creature.renderers)
                {
                    
                    if (data.renderer)
                    {
                        Material temp = material.DeepCopyByExpressionTree(); // Deep copy of material
                        Material[] materials = data.renderer.materials.DeepCopyByExpressionTree(); // original materials on creature renderer
                        Material[] tempArray = new Material[materials.Length]; // Water material array
                        for (int i = 0; i < tempArray.Length; i++)
                        {
                            tempArray[i] = temp.DeepCopyByExpressionTree();
                        }

                        data.renderer.materials = tempArray.DeepCopyByExpressionTree(); // set renderer material array to deep copy of the water materials array

                        /*Add transitioning class to creature*/
                        creature.gameObject.AddComponent<WaterCloneActive>().Setup(data.renderer, materials,
                            data.renderer.materials.DeepCopyByExpressionTree(), creature);
                    }
                }
                
                //Only need one of this class type to be active
                creature.gameObject.AddComponent<WaterCloneSizing>().Setup(creature);
            });
            
        }

        /*
         * On damage event when creature is hit
         */
        private void OnDamageEvent(CollisionInstance collisioninstance, EventTime eventtime)
        {
            if (!hit)
            {
                Creature creature = collisioninstance.targetCollider.GetComponentInParent<Creature>(); //hit creature
                GameManager.local.StartCoroutine(timeAfter(creature)); // async coroutine
                hit = !hit; // invert for timeAfter method
            }
        }

        IEnumerator timeAfter(Creature creature)
        {
            yield return new WaitForSeconds(0.7f); // wait 0.7 seconds
            VisualEffect vfx = this.waterVFX.GetComponentInChildren<VisualEffect>();
            VisualEffect go = Object.Instantiate(vfx);
            go.transform.position = creature.ragdoll.headPart.transform.position;
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y - creature.GetHeight(),
                go.transform.position.z);
            GameObject tempSound = Object.Instantiate(sound2);
            tempSound.GetComponent<AudioSource>().Play();
            creature.Despawn();
            hit = !hit;
        }

        private static void SetCreatureEquipment(Creature creature)
        {
            //check the creature exists tho
            Creature playerCreature = Player.currentCreature;
            if(playerCreature == null) return;
            //check the containers not empty/null
            Container playerContainer = playerCreature.container;
            if (playerContainer == null || playerContainer.contents == null) return;
            
            //try catch just incase woo
            try
            {
                foreach (ContainerData.Content content in playerContainer.contents)
                {
                    //add the content to the creatures container
                    creature.container.contents.Add(content);
                    //check if its a wardrobe item and equip it
                    if (content.itemData.TryGetModule(out ItemModuleWardrobe _))
                    {
                        creature.equipment.EquipWardrobe(content);
                    }

                    //check if its a holder item and spawn and snap it
                    if (content.TryGetState(out ContentStateHolder state))
                    {
                        content.Spawn(item => {
                            foreach (Holder holder in creature.holders)
                            {
                                if (holder.name != state.holderName) continue;
                                holder.Snap(item, true, true);
                            }
                        });
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Something went wrong when setting the creatures equipment {e}");
            }
        }

        private static void SetCreatureLooks(Creature creature)
        {
            //Used because SetHeight causes some issues when trying to hard set a position with SpellCastProjectile
            try
            {
                creature.SetHeight(Player.currentCreature.GetHeight());
                creature.SetColor(Player.characterData.hairColor, Creature.ColorModifier.Hair);
                creature.SetColor(Player.characterData.hairSecondaryColor, Creature.ColorModifier.HairSecondary);
                creature.SetColor(Player.characterData.hairSpecularColor, Creature.ColorModifier.HairSpecular);
                creature.SetColor(Player.characterData.skinColor, Creature.ColorModifier.Skin);
                creature.SetColor(Player.characterData.eyesIrisColor, Creature.ColorModifier.EyesIris);
                creature.SetColor(Player.characterData.eyesScleraColor, Creature.ColorModifier.EyesSclera);
            } 
            catch (System.Exception e)
            {
                Debug.LogError($"Something went wrong when setting the creatures height and colours {e}");
            }
        }
    }
}