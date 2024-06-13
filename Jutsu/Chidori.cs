using UnityEngine.VFX;

namespace Jutsu
{
    using UnityEngine;
    using ThunderRoad;
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
        public override void Fire(bool active)
        {
            if (!chidori)
            {
                chidori = GameObject.Instantiate(JutsuEntry.local.chidori);
                chidoriVFX = chidori.GetComponentInChildren<VisualEffect>();
                chidoriStart = GameObject.Instantiate(JutsuEntry.local.chidoriStartSFX);
                chidoriStartSFX = chidoriStart.GetComponent<AudioSource>();
                chidoriLoop = GameObject.Instantiate(JutsuEntry.local.chidoriLoopSFX);
                chidoriLoopSFX = chidoriLoop.GetComponent<AudioSource>();
                chidoriVFX.Play();
                instantiated = true;
            }
        }


        public override void UpdateCaster()
        {
            //base.UpdateCaster();
            if (instantiated)
            {
                if (spellCaster.isFiring)
                {
                    if (!startSoundPlayed)
                    {
                        chidoriStartSFX.Play();
                        startSoundPlayed = true;
                    }

                    if ((startSoundPlayed && !chidoriStartSFX.isPlaying) && !chidoriLoopSFX.isPlaying)
                    {
                        chidoriLoopSFX.Play();
                    }

                    var transform = spellCaster.ragdollHand.caster.magicSource.transform;
                    chidori.transform.position = transform.position + (-transform.forward * 0.05f);
                    chidori.transform.rotation = transform.rotation;

                    if (!damager)
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

                    }
                }

                else
                {
                    startSoundPlayed = false;
                    chidoriVFX.Stop();
                    chidori.transform.position = spellCaster.ragdollHand.caster.magicSource.transform.position;
                    chidori.transform.rotation = spellCaster.ragdollHand.caster.magicSource.transform.rotation;
                }
            }
        }

        public override void Unload()
        {
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