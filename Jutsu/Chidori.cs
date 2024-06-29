using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad.AI.Action;
using UnityEngine.VFX;

namespace Jutsu
{
    using UnityEngine;
    using ThunderRoad;

    public enum HandSigns
    {
        None,
        Tiger,
        Monkey,
        Dragon,
        Rat,
        Bird,
        Snake,
        Ox,
        Dog,
        Horse,
        Boar,
        Ram,
        Hare
    }

    /**
     * Class for Chidori spell
     */
    public class Chidori : SkillData
    {
        private ItemData chidoriData;
        private Item chidori;
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
        private bool chidoriStarted = false;
        private bool vfxStarted = false;
        private Coroutine chidoriRun;

        private readonly List<HandSigns> chidoriSigns = new List<HandSigns>
        {
            HandSigns.Monkey, HandSigns.Dragon, HandSigns.Rat, HandSigns.Bird, HandSigns.Ox, HandSigns.Snake,
            HandSigns.Dog, HandSigns.Tiger, HandSigns.Monkey
        };
        public override void OnSkillLoaded(SkillData skillData, Creature creature)
        {
            base.OnSkillLoaded(skillData, creature);

            chidoriRun = GameManager.local.StartCoroutine(RunChidori());
            Seals seals = new Seals();
            var activated = JutsuEntry.local.root.Then(() => seals.HandDistance(activateChidori));
            activated.Then(seals.MonkeySeal)
                .Then(seals.DragonSeal)
                .Then(seals.RatSeal)
                .Then(seals.BirdSeal)
                .Then(seals.OxSeal)
                .Then(seals.SnakeSeal)
                .Then(seals.DogSeal)
                .Then(seals.TigerSeal)
                .Then(seals.MonkeySeal)
                .Do(() => activateChidori = true);
        }

        public override void OnSkillUnloaded(SkillData skillData, Creature creature)
        {
            base.OnSkillUnloaded(skillData, creature);
            GameManager.local.StopCoroutine(chidoriRun);
        }

        private bool handSignEventOver = false;
        private bool coroutineStarted = false;
        private bool activateChidori = false;
        
        private FixedJoint joint;
        private bool hasPenetrated = false;
        private Creature penetrated;
        private List<HandSigns> tempSigns = new List<HandSigns>();
        private bool chidoriTimerActive = false;
        private bool waitActive = false;
        private HandSigns prevHandSign;
        private bool disabled = false;
        
        IEnumerator RunChidori()
        {
            yield return new WaitForSeconds(2f);
            while (true)
            {
                JutsuEntry.local.root.Update();
                if (JutsuEntry.local.root.AtEnd()) JutsuEntry.local.root.Reset(); 
                
                if (Vector3.Distance(Player.local.handRight.ragdollHand.caster.transform.position,
                        Player.local.handLeft.ragdollHand.caster.transform.position) < 0.1f && !activateChidori)
                {
                    if (!JutsuEntry.local.spellWheelDisabled)
                    {
                        Player.local.handRight.ragdollHand.caster.DisableSpellWheel(this);
                        Player.local.handLeft.ragdollHand.caster.DisableSpellWheel(this);
                        JutsuEntry.local.spellWheelDisabled = true;
                    }
                }
                else
                {
                    if (JutsuEntry.local.spellWheelDisabled)
                    {
                        JutsuEntry.local.root.Reset();
                        Player.local.handRight.ragdollHand.caster.AllowSpellWheel(this);
                        Player.local.handLeft.ragdollHand.caster.AllowSpellWheel(this);
                        JutsuEntry.local.spellWheelDisabled = false;
                    }
                }
                if (activateChidori)
                {
                    if (!chidoriTimerActive)
                    {
                        chidoriTimerActive = true;
                        GameManager.local.StartCoroutine(ChidoriActiveTimer());
                    }
                    if (!chidori && !chidoriStarted)
                    {
                        chidoriStarted = true;
                        //Instantiate all Chidori GameObjects
                        //Set up references for Chidori VFX and SFX
                        Catalog.TryGetData("ChidoriItem", out chidoriData);
                        if (chidoriData != null)
                        {
                            chidoriData.SpawnAsync(item =>
                            {
                                chidori = item;
                                chidori.physicBody.useGravity = false;
                                chidori.transform.position =
                                    Player.local.handRight.ragdollHand.transform.position;
                                chidori.transform.rotation =
                                    Player.local.handRight.ragdollHand.transform.rotation;
                                chidori.IgnoreRagdollCollision(Player.local.creature.ragdoll);
                                joint = Player.local.handRight.ragdollHand.gameObject.AddComponent<FixedJoint>();
                                joint.breakForce = Mathf.Infinity;
                                joint.breakTorque =  Mathf.Infinity;
                                joint.connectedBody = chidori.physicBody.rigidBody;
                                
                                foreach (Creature c in Creature.allActive)
                                {
                                    if (!c.isPlayer)
                                    {
                                        Player.local.creature.ragdoll.rightUpperArmPart.ragdoll.IgnoreCollision(
                                            c.ragdoll, true);
                                        Player.local.creature.handRight.ragdoll.IgnoreCollision(c.ragdoll, true);
                                    }
                                }
                                chidoriVFX = chidori.gameObject.GetComponentInChildren<VisualEffect>();
                                chidoriStart = GameObject.Instantiate(JutsuEntry.local.chidoriStartSFX);
                                chidoriStartSFX = chidoriStart.GetComponent<AudioSource>();
                                chidoriLoop = GameObject.Instantiate(JutsuEntry.local.chidoriLoopSFX);
                                chidoriLoopSFX = chidoriLoop.GetComponent<AudioSource>();
                                chidoriVFX.Stop();
                            });
                        }
                        else
                        {
                            chidoriStarted = false;
                        }

                    }

                    else if(chidori)
                    {
                        //While spell active and holding trigger
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
                            if (!vfxStarted)
                            {
                                chidoriVFX.Play();
                                vfxStarted = true;
                            }

                            if (chidori.isPenetrating)
                            {
                                penetrated = chidori.GetComponentInParent<Creature>();
                                hasPenetrated = true;
                            }
                            if (!chidori.isPenetrating && hasPenetrated)
                            {
                                Debug.Log("HAS PENETRATED IS TRUE");
                                GameObject.DestroyImmediate(joint);
                                chidori.transform.position =
                                    Player.local.handRight.ragdollHand.transform.position;
                                chidori.transform.rotation =
                                    Player.local.handRight.ragdollHand.transform.rotation;
                                chidori.IgnoreRagdollCollision(Player.local.creature.ragdoll);
                                if(penetrated) chidori.IgnoreRagdollCollision(penetrated.ragdoll);
                                Debug.Log("JOINT IS: " + joint);
                                joint = Player.local.handRight.ragdollHand.gameObject.AddComponent<FixedJoint>();
                                joint.breakForce = Mathf.Infinity;
                                joint.breakTorque =  Mathf.Infinity;
                                joint.connectedBody = chidori.physicBody.rigidBody;
                                chidori.ResetRagdollCollision();
                                chidori.IgnoreRagdollCollision(Player.local.creature.ragdoll);
                                hasPenetrated = false;
                            }
                    }
                }
                else
                {
                    //Reset all values to default while still updating Chidori position
                    if (chidori && vfxStarted)
                    {
                        if (chidoriStartSFX.isPlaying)
                        {
                            chidoriStartSFX.Stop();
                        }
                        else if (chidoriLoopSFX.isPlaying)
                        {
                            chidoriLoopSFX.Stop();
                        }
                        startSoundPlayed = false;
                        chidoriVFX.Stop();
                        chidoriStarted = false;
                        vfxStarted = false;
                        foreach (Creature c in Creature.allActive)
                        {
                            if (!c.isPlayer)
                            {
                                Player.local.creature.ragdoll.rightUpperArmPart.ragdoll
                                    .IgnoreCollision(c.ragdoll, false);
                                Player.local.creature.handRight.ragdoll.IgnoreCollision(c.ragdoll, false);
                            }
                        }
                        chidori.Despawn();
                        chidoriTimerActive = false;
                    }
                }

                yield return null;
            }
        }

        IEnumerator ChidoriActiveTimer()
        {
            yield return new WaitForSeconds(10f);
            activateChidori = false;
            if (disabled)
            {
                Player.local.handRight.ragdollHand.caster.AllowSpellWheel(this);
                Player.local.handLeft.ragdollHand.caster.AllowSpellWheel(this);
                disabled = false;
            }
        }
    }
}