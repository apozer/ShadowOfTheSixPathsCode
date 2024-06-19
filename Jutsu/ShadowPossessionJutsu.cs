
using System.Collections.Generic;
using System.Linq;

namespace Jutsu
{
    using UnityEngine;
    using ThunderRoad;
    using UnityEngine.VFX;
    
    /**
     * Shadow Possession Jutsu VFX, SFX and functionality
     */
    public class ShadowPossessionJutsu : SpellCastCharge
    {
        
        //Shadow VFX and SFX
        public GameObject shadow;
        public GameObject shadowUpdate;
        public VisualEffect shadowVFX;
        private VisualEffect shadowEffect;
        public GameObject shadowSFX;
        public GameObject shadowSFXUpdate;
        public AudioSource shadowAudio;
        
        private bool instantiated = false;
        
        private bool started = false;
        
        //Transforms for VFX bezier curve
        private Transform A;
        private Transform B;
        private Transform C;
        private Transform D;
        private bool playing = false;
        private bool activate = false;
        
        //References to original positions
        private Vector3 ogBPos;
        private Vector3 currentBPos;
        private Vector3 ogCPos;
        private Vector3 currentCPos;
        
        private bool returnedTo = false;
        private bool soundPlayed = false;
        public override void Load(SpellCaster spellCaster, Level level)
        {
            base.Load(spellCaster, level);
            
            //Set local references for VFX and SFX
            shadow = JutsuEntry.local.shadow;
            shadowSFX = JutsuEntry.local.shadowSFX;
        }

        public override void Fire(bool active)
        {
            base.Fire(active);

            if (!instantiated)
            {
                //Intantiate VFX and SFX
                shadowSFXUpdate = GameObject.Instantiate(shadowSFX);
                shadowUpdate = GameObject.Instantiate(shadow);
                shadowUpdate.transform.position = Player.local.transform.position;
                shadowVFX = shadowUpdate.GetComponentInChildren<VisualEffect>();
                shadowVFX.SetFloat("start", 0f);
                shadowVFX.Play();
                
                //Set references for bezier points
                A = GameObject.Find("A").transform;
                B = GameObject.Find("B").transform;
                C = GameObject.Find("C").transform;
                D = GameObject.Find("D").transform;
                
                //Start position is players feet
                shadowSFXUpdate.transform.position = A.position;
                shadowAudio = shadowSFXUpdate.GetComponent<AudioSource>();
                instantiated = true;
            }
        }

        //Used for Lerps
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
                //If spell caster is loaded and trigger is held
                if (spellCaster.isFiring)
                {
                    
                    //If playing is false
                    if (!playing)
                    {
                        playing = true;   
                        shadowVFX.Play();
                        //Check if creature has the shadow possesion controller active
                        if(selectedCreature is Creature creature1 && creature1.gameObject.GetComponent<ShadowPossessionController>() is ShadowPossessionController spc) spc.Unpossess();
                        
                        //Reset values
                        selectedCreature = null;
                        D.position = Player.local.creature.transform.position;
                        
                        //Check if all active is not null or empty and there is not currently selected creature
                        if (!Creature.allActive.IsNullOrEmpty() && !selectedCreature)
                        {
                            
                            //Loop over active creature list
                            foreach (Creature creature in Creature.allActive)
                            {
                                //Verify creature is not the player
                                if (!creature.isPlayer)
                                {
                                    selectedCreature = creature;
                                    
                                    //Get randome distance and direction for B and C positions in Bezier curve
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
                                    
                                    //Set current postions and store them for use to return to normal later
                                    ogBPos = B.position = A.position + (direction.normalized * (0.3f * distance));
                                    currentBPos = B.position += (perpendicular * distB);
                                    ogCPos = C.position = A.position + (direction.normalized * (0.6f * distance));
                                    currentCPos = C.position += (-perpendicular * distC);
                                    
                                    //Adds shadow possesion controller
                                    if(!selectedCreature.gameObject.GetComponent<ShadowPossessionController>()) selectedCreature.gameObject.AddComponent<ShadowPossessionController>();
                                    else selectedCreature.gameObject.GetComponent<ShadowPossessionController>().Possess();
                                    //break after creature is selected.
                                    break;
                                }
                            }
                        }
                        
                    }
                    //If creature is found and shadow vfx has not started
                    if (selectedCreature && !shadowAttached)
                    {
                        
                        //Player SFX
                        if (!soundPlayed)
                        {
                            shadowAudio.Play();
                            soundPlayed = true;
                        }
                        //Update A position to player feet
                        A.position = new Vector3(Player.local.creature.player.head.transform.position.x,Player.local.creature.transform.position.y, Player.local.creature.player.head.transform.position.z);
                        D.position = selectedCreature.transform.position;
                        
                        //Get time since vfx started and track percentage between 0 and 1
                        elapsedTime += Time.deltaTime;
                        float percentageComplete = elapsedTime / ((D.position - A.position).magnitude * 0.05f);
                        
                        //Update particle positions along the bezier curve
                        shadowVFX.SetFloat("start", percentageComplete);
                        
                        if (percentageComplete >= 1f)
                        {
                            
                            //Once close enough (Vector3 comparison is inaccurate) set final values
                            shadowVFX.SetFloat("start", 1f);
                            elapsedTime = 0f;
                            D.position = B.position;
                            shadowAttached = true;
                        }
                        if (!returnedTo)
                        {
                            //Moves B and C Positions back to original point on the line
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
                    //If shadow is attached to the enemy creature
                    if (shadowAttached)
                    {
                        
                        //Update A position to the Player feet
                        A.transform.position = new Vector3(Player.local.creature.player.head.transform.position.x,Player.local.creature.transform.position.y, Player.local.creature.player.head.transform.position.z);
                        //Update D position to the enemy feet
                        D.position = selectedCreature.transform.position;
                        
                        if (!returnedTo)
                        {
                            //Moves B and C Positions back to original point on the line
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
                        //TODO: Add code to do the opposite of what the original lerp does, so vfx leaves in reverse (Cool effect)
                        
                        //Reset all values to default
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