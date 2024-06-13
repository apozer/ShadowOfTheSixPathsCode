
using System.Collections.Generic;
using System.Linq;

namespace Jutsu
{
    using UnityEngine;
    using ThunderRoad;
    using UnityEngine.VFX;
    public class ShadowPossessionJutsu : SpellCastCharge
    {
        public GameObject shadow;
        public GameObject shadowUpdate;
        public VisualEffect shadowVFX;
        public GameObject shadowSFX;
        public GameObject shadowSFXUpdate;
        public AudioSource shadowAudio;
        private bool instantiated = false;
        private VisualEffect shadowEffect;
        private bool started = false;
        private Transform A;
        private Transform B;
        private Transform C;
        private Transform D;
        private bool PA = false;
        private bool AB = false;
        private bool BC = false;
        private bool CD = false;
        private bool playing = false;
        private GameObject ADebug;
        private GameObject BDebug;
        private GameObject CDebug;
        private GameObject DDebug;
        private bool activate = false;
        private bool debug = false;
        private Vector3 ogBPos;
        private Vector3 currentBPos;
        private Vector3 ogCPos;
        private Vector3 currentCPos;
        private bool returnedTo = false;
        private bool soundPlayed = false;
        public override void Load(SpellCaster spellCaster, Level level)
        {
            base.Load(spellCaster, level);
            shadow = JutsuEntry.local.shadow;
            shadowSFX = JutsuEntry.local.shadowSFX;
        }

        public override void Fire(bool active)
        {
            base.Fire(active);

            if (!instantiated)
            {
                shadowSFXUpdate = GameObject.Instantiate(shadowSFX);
                shadowUpdate = GameObject.Instantiate(shadow);
                shadowUpdate.transform.position = Player.local.transform.position;
                shadowVFX = shadowUpdate.GetComponentInChildren<VisualEffect>();
                shadowVFX.SetFloat("start", 0f);
                shadowVFX.Play();
                A = GameObject.Find("A").transform;
                B = GameObject.Find("B").transform;
                C = GameObject.Find("C").transform;
                D = GameObject.Find("D").transform;
                shadowSFXUpdate.transform.position = A.position;
                shadowAudio = shadowSFXUpdate.GetComponent<AudioSource>();
                instantiated = true;
            }
        }


        private float elapsedTime = 0f;
        private float returnElapsedTime = 0f;
        private Creature selectedCreature;
        private bool shadowAttached = false;
        private bool aPosSet = false;
        private Vector3 apos;
        public override void UpdateCaster()
        {
            base.UpdateCaster();
            if (instantiated)
            {
                if (spellCaster.isFiring)
                {
                    if (!playing)
                    {
                     
                        playing = true;   shadowVFX.Play();
                        if(selectedCreature is Creature creature1 && creature1.gameObject.GetComponent<ShadowPossessionController>() is ShadowPossessionController spc) spc.Unpossess();
                        selectedCreature = null;
                        D.position = Player.local.creature.transform.position;
                        if (!Creature.allActive.IsNullOrEmpty() && !selectedCreature)
                        {
                            foreach (Creature creature in Creature.allActive)
                            {
                                if (!creature.isPlayer)
                                {
                                    selectedCreature = creature;
                                    var distance = Vector3.Distance(A.position, selectedCreature.transform.position);
                                    var direction = (selectedCreature.transform.position - A.position);
                                    var perpendicular = Vector3.Cross(Vector3.up, direction).normalized;
                                    
                                    var randomBOperator = Random.Range(0, 100);
                                    var distB = Random.Range(0.8f, 2f);
                                    if (randomBOperator < 0)
                                    {
                                        distB *= -1;
                                    }

                                    var randomCOperator = Random.Range(0, 100);
                                    var distC = Random.Range(2f, 0.8f);
                                    if (randomCOperator < 0)
                                    {
                                        distC *= -1;
                                    }
                                    
                                    ogBPos = B.position = A.position + (direction.normalized * (0.3f * distance));
                                    currentBPos = B.position += (perpendicular * distB);
                                    ogCPos = C.position = A.position + (direction.normalized * (0.6f * distance));
                                    currentCPos = C.position += (-perpendicular * distC);
                                    //add code to lerp back to original positions on main line
                                    
                                    
                                    if(!selectedCreature.gameObject.GetComponent<ShadowPossessionController>()) selectedCreature.gameObject.AddComponent<ShadowPossessionController>();
                                    else selectedCreature.gameObject.GetComponent<ShadowPossessionController>().Possess();
                                    break;
                                }
                            }
                        }
                        
                    }
                    if (selectedCreature && !shadowAttached)
                    {
                        if (!soundPlayed)
                        {
                            shadowAudio.Play();
                            soundPlayed = true;
                        }
                        A.position = new Vector3(Player.local.creature.player.head.transform.position.x,Player.local.creature.transform.position.y, Player.local.creature.player.head.transform.position.z);
                        D.position = selectedCreature.transform.position;
                        
                        elapsedTime += Time.deltaTime;
                        float percentageComplete = elapsedTime / ((D.position - A.position).magnitude * 0.05f);
                        shadowVFX.SetFloat("start", percentageComplete);
                        if (percentageComplete >= 1f)
                        {
                            shadowVFX.SetFloat("start", 1f);
                            elapsedTime = 0f;
                            D.position = B.position;
                            shadowAttached = true;
                        }
                        if (!returnedTo)
                        {
                            returnElapsedTime += Time.deltaTime;
                            float percentageBComplete = returnElapsedTime / 1.2f;
                            float percentageCComplete = returnElapsedTime / 1.2f;
                            B.position = Vector3.Lerp(currentBPos, ogBPos, percentageBComplete);
                            C.position = Vector3.Lerp(currentCPos, ogBPos, percentageCComplete);
                            if (Vector3.Distance(B.position, ogBPos) < 0.01f)
                            {
                                if (Vector3.Distance(C.position, ogCPos) < 0.01f)
                                {
                                    returnElapsedTime = 0f;
                                    returnedTo = true;
                                }
                            }
                        }
                    }

                    if (shadowAttached)
                    {
                        A.transform.position = new Vector3(Player.local.creature.player.head.transform.position.x,Player.local.creature.transform.position.y, Player.local.creature.player.head.transform.position.z);
                        D.position = selectedCreature.transform.position;
                        if (!returnedTo)
                        {
                            returnElapsedTime += Time.deltaTime;
                            float percentageBComplete = returnElapsedTime / 1.2f;
                            float percentageCComplete = returnElapsedTime / 1.2f;
                            B.position = Vector3.Lerp(currentBPos, ogBPos, percentageBComplete);
                            C.position = Vector3.Lerp(currentCPos, ogBPos, percentageCComplete);
                            if (Vector3.Distance(B.position, ogBPos) < 0.01f)
                            {
                                if (Vector3.Distance(C.position, ogCPos) < 0.01f)
                                {
                                    returnElapsedTime = 0f;
                                    returnedTo = true;
                                }
                            }
                        }
                    }
                    
                }
                else
                {
                    if (playing && selectedCreature)
                    {
                        soundPlayed = false;
                        
                        shadowVFX.Stop();
                        shadowVFX.SetFloat("start", 0f);
                        activate = false;
                        elapsedTime = 0f;
                        returnElapsedTime = 0f;
                        D.position = Player.local.transform.position;
                        A.position = Player.local.transform.position;
                        playing = false;
                        returnedTo = false;
                        shadowAttached = false;
                        selectedCreature.brain.SetState(Brain.State.Alert);
                    }
                }
            }
        }
    }
}