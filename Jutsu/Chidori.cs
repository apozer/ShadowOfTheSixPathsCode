using UnityEngine.VFX;

namespace Jutsu
{
    using UnityEngine;
    using ThunderRoad;
    
    /**
     * Class for Chidori spell
     */
    public class Chidori : SpellCastCharge
    {
        private GameObject chidori;
        private VisualEffect chidoriVFX;
        private GameObject chidoriStart;
        private GameObject chidoriLoop;
        private AudioSource chidoriStartSFX;
        private AudioSource chidoriLoopSFX;
        private bool startSoundPlayed = false;
        private bool instantiated = false;
        private bool damager = false;
        private Damager pierce;
        private GameObject pierceGO;
        
        /**
         * Fire
         * parameters - active (boolean)
         * Runs code for setting up Chidori assets
         */
        public override void Fire(bool active)
        {
            //Execute once
            if (!chidori)
            {
                //Instantiate all Chidori GameObjects
                //Set up references for Chidori VFX and SFX
                chidori = GameObject.Instantiate(JutsuEntry.local.chidori);
                chidoriVFX = chidori.GetComponentInChildren<VisualEffect>();
                chidoriStart = GameObject.Instantiate(JutsuEntry.local.chidoriStartSFX);
                chidoriStartSFX = chidoriStart.GetComponent<AudioSource>();
                chidoriLoop = GameObject.Instantiate(JutsuEntry.local.chidoriLoopSFX);
                chidoriLoopSFX = chidoriLoop.GetComponent<AudioSource>();
                chidoriVFX.Play();
                
                //Set once
                instantiated = true;
            }
        }


        public override void UpdateCaster()
        {
            //Run if all references are assigned
            if (instantiated)
            {
                
                //While spell active and holding trigger
                if (spellCaster.isFiring)
                {
                    //Check to see if sound is played
                    if (!startSoundPlayed)
                    {
                        chidoriStartSFX.Play();
                        startSoundPlayed = true;
                    }
                    //Check to see if sound can begin looping
                    if ((startSoundPlayed && !chidoriStartSFX.isPlaying) && !chidoriLoopSFX.isPlaying)
                    {
                        chidoriLoopSFX.Play();
                    }
                    
                    //Get SpellCaster magic transform position and set chidori to that (Adjusted some to go into the hand, rather than in the palm)
                    //Update position and rotation every frame
                    var transform = spellCaster.ragdollHand.caster.magicSource.transform;
                    chidori.transform.position = transform.position + (-transform.forward * 0.05f);
                    chidori.transform.rotation = transform.rotation;
                    
                    /*
                     * Currently not functional code
                     */
                    /*if (!damager)
                    {
                        damager = true;
                        GameObject go = new GameObject();
                        go.AddComponent<Damager>();
                        pierceGO = GameObject.Instantiate(go);
                        pierce = pierceGO.GetComponent<Damager>();
                        pierceGO.transform.parent = spellCaster.ragdollHand.fingerMiddle.tip.transform;
                        pierce.direction = Damager.Direction.Forward;
                        pierce.penetrationLength = 1f;
                        pierce.transform.position = spellCaster.ragdollHand.fingerMiddle.tip.transform.position;
                        pierce.transform.rotation = spellCaster.ragdollHand.fingerMiddle.tip.transform.rotation;

                    }*/
                }
                else
                {
                    //Reset all values to default while still updating Chidori position
                    startSoundPlayed = false;
                    chidoriVFX.Stop();
                    chidori.transform.position = spellCaster.ragdollHand.caster.magicSource.transform.position;
                    chidori.transform.rotation = spellCaster.ragdollHand.caster.magicSource.transform.rotation;
                }
            }
        }

        public override void Unload()
        {
            
            //Remove assets when spell is unloaded
            base.Unload();
            GameObject.Destroy(chidori);
            GameObject.Destroy(chidoriVFX);
            GameObject.Destroy(chidoriStart);
            GameObject.Destroy(chidoriStartSFX);
            GameObject.Destroy(chidoriLoop);
            GameObject.Destroy(chidoriLoopSFX);
        }
    }
}