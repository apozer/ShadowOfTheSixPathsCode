using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace Jutsu
{
    public class Rasenshuriken : SpellCastCharge
    {

        public RagdollHand spawnHand;
        public ItemData rasenshurikenData;
        private Item rasenshuriken;
        private bool currentlyCasting = false;
        private bool started = false;
        
        /**
         * Load necessary rasenshuriken data
         */
        public override void Load(SpellCaster spellCaster)
        {
            base.Load(spellCaster);
            rasenshurikenData = Catalog.GetData<ItemData>("apoz123.Jutsu.Chakra.Nature.Rasenshuriken");
        }

        IEnumerator DestroyObject(Item item)
        {
            Item.Destroy(item);
            yield return null;
            started = false;
        }


        public override void UpdateCaster()
        {
            //base.UpdateCaster();

            if (spellCaster.isFiring)
            {
                if (rasenshurikenData != null && !currentlyCasting)
                {
                    currentlyCasting = true;
                    rasenshurikenData.SpawnAsync(item =>
                    {
                        rasenshuriken = item;
                        rasenshuriken.IgnoreRagdollCollision(Player.local.creature.ragdoll);
                        rasenshuriken.transform.GetChild(0).GetChild(0).gameObject.AddComponent<ShurikenRotate>();
                        //rasenshuriken.transform.position = spellCaster.magic.transform.position;
                        //rasenshuriken.transform.rotation = spellCaster.magic.transform.rotation;

                    });
                }

                if (rasenshuriken)
                {
                    //Update position to spellcaster magic transform every frame
                    //rasenshuriken.transform.position = spellCaster.magic.transform.position;
                    //rasenshuriken.transform.rotation = spellCaster.magic.transform.rotation;
                }
            }

            else
            {
                if (rasenshuriken && !started)
                {
                    started = true;
                    GameObject.Destroy(rasenshuriken);
                }

                if (!rasenshuriken)
                {
                    started = false;
                }
            }
        }
    }
}