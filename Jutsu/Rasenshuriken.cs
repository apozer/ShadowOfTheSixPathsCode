using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace Jutsu
{
    /**
     * Rasenshuriken spell class for VFX and functionality
     */
    public class Rasenshuriken : SpellCastCharge
    {
        public ItemData rasenshurikenData;
        private Item rasenshuriken;
        private bool currentlyCasting = false;
        private bool started = false;
        
        /**
         * Load necessary rasenshuriken data
         */
        public override void Load(SpellCaster spellCaster, Level level)
        {
            base.Load(spellCaster, level);
            rasenshurikenData = Catalog.GetData<ItemData>("apoz123.Jutsu.Chakra.Nature.Rasenshuriken");
        }

        /**
         * Future coroutine to destroy vfx and gameobject after fire is done
         */
        IEnumerator DestroyObject(Item item)
        {
            Item.Destroy(item);
            yield return null;
            started = false;
        }


        public override void UpdateCaster()
        {
            //Check if spell is loaded and trigger is held
            if (spellCaster.isFiring)
            {
                //Verify rasenshurikenData and rasenshuriken is allowed to spawn
                if (rasenshurikenData != null && !currentlyCasting)
                {
                    currentlyCasting = true;
                    
                    //Spawn item from SpawnAsync method using ItemData class
                    rasenshurikenData.SpawnAsync(item =>
                    {
                        rasenshuriken = item;
                        rasenshuriken.IgnoreRagdollCollision(Player.local.creature.ragdoll);
                        rasenshuriken.transform.GetChild(0).GetChild(0).gameObject.AddComponent<ShurikenRotate>();
                        rasenshuriken.transform.position = spellCaster.magic.transform.position;
                        rasenshuriken.transform.rotation = spellCaster.magic.transform.rotation;

                    });
                }
                //Verify rasenshuriken Item is not null
                if (rasenshuriken)
                {
                    //Update position to spellcaster magic transform every frame
                    rasenshuriken.transform.position = spellCaster.magic.transform.position;
                    rasenshuriken.transform.rotation = spellCaster.magic.transform.rotation;
                }
            }

            else
            {
                //Rasenshuriken is not null and destroy hasnt started (Dysfunctional)
                if (rasenshuriken && !started)
                {
                    started = true;
                    GameObject.Destroy(rasenshuriken);
                }
                
                //Reset once rasenshuriken is destroyed (Dysfunctional)
                if (!rasenshuriken)
                {
                    started = false;
                }
            }
        }
    }
}