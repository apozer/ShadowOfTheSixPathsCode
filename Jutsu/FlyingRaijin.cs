using System;
using UnityEngine;
using ThunderRoad;

namespace Jutsu
{
    /**
     * Flying Raijin Jutsu
     * Script to teleport player to the Flying Raijin Dagger
     */
    public class FlyingRaijin : MonoBehaviour
    {
        private Item item;
        private bool teleported = false;
        private RagdollHand hand;
        private bool disabled = false;
        private HandlePose orientation;

        public void Start()
        {
            item = GetComponent<Item>();
            this.item.OnGrabEvent += grabbed_weapon;
            this.item.OnUngrabEvent += ungrabbed_weapon;
            Player.local.locomotion.OnGroundEvent += ground_event;
        }

        void ground_event(Locomotion locaLocomotion, UnityEngine.Vector3 groundPoint, UnityEngine.Vector3 velocity, Collider groundCollider)
        {
            Player.fallDamage = true;
        }

        void grabbed_weapon(Handle handle, RagdollHand hand)
        {
            this.hand = hand;
            foreach (var x in hand.grabbedHandle.orientations)
            {
                if (x.handle.item.mainHandler != null)
                {
                    this.orientation = x;
                    return;
                }
            }
        }
        void ungrabbed_weapon(Handle handle, RagdollHand hand, bool data)
        {
            this.teleported = false;
        }
        
        private void Update()
        {
            //Check if hand isnt null, grabbedHandle is null && Item this script is a child of is thrown
            if (this.hand && !this.hand.grabbedHandle && item.isThrowed)
            {
                //Spell wheel disable check
                if (!disabled)
                {
                    this.hand.caster.DisableSpellWheel(this);
                    this.disabled = true;
                }
                //Check if spell wheel is pressed and user has not teleported
                else if (this.hand.playerHand.controlHand.alternateUsePressed && !teleported)
                {
                    
                    //Code jenix sent me, needs tweeking to feel cleaner for the mod. Works really good out of the gate though
                    Quaternion rotation = Player.local.transform.rotation;
                    Vector3 position = Player.local.transform.position;
                    Common.MoveAlign(Player.local.transform, hand.grip, orientation.transform);
                    rotation = Player.local.transform.rotation;
                    Player.local.locomotion.prevPosition = item.handles[0].transform.position;
                    Common.MoveAlign(item.transform, orientation.transform,hand.grip);
                    if (hand.grabbedHandle == null)
                    {
                        this.hand.Grab(this.item.handles[0], this.orientation, this.item.handles[0].GetDefaultAxisLocalPosition());
                    }

                    Player.fallDamage = false;
                    Player.local.creature.ragdoll.ik.AddLocomotionDeltaPosition(Player.local.transform.position - position);
                    Player.local.creature.ragdoll.ik.AddLocomotionDeltaRotation(Player.local.transform.rotation * Quaternion.Inverse(rotation), Player.local.transform.TransformPoint(Player.local.creature.transform.localPosition));
                    Player.local.locomotion.velocity = new Vector3(0, 0, 0);
                    
                    //Not sure what this was meant to do.
                    /*Player.local.creature.Teleport(item.transform.position, Quaternion.Inverse(Player.local.creature.transform.rotation));
                    item.transform.position = this.hand.transform.position;
                    this.hand.Grab(this.item.handles[0]);*/
                    
                    //Reset default values
                    this.teleported = true;
                    this.disabled = false;
                    this.hand.caster.enabled = true;
                    
                    //Re-enable spell wheel
                    this.hand.caster.AllowSpellWheel(this);
                }
            }
        }
    }
}