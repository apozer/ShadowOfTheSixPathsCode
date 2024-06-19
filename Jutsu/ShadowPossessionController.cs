using System;
using UnityEngine;
using ThunderRoad;
using Newtonsoft.Json;

namespace Jutsu
{
    /**
     * Shadow Possession Jutsu Controller for enemy creature ragdoll manipulation
     */
    public class ShadowPossessionController : MonoBehaviour
    {
        private Creature creature;
        private Vector3 relativeTargetDirectionRight;
        private Vector3 relativeAnchorDirectionRight;
        private Vector3 relativeAnchorDirectionLeft;
        private Vector3 relativeTargetDirectionLeft;
        private bool instantiated = false;
        private Transform localRightTransform;
        private Transform oneHandTransform;
        private Transform oneHandAnchor;
        private BrainModuleDetection moduleDetection;
        
        public void Start()
        {
            creature = GetComponentInParent<Creature>();
            
            //Code attempt to manipulate creature physics to mimic player ragdoll and movement
            this.moduleDetection = creature.brain.instance.GetModule<BrainModuleDetection>();
            this.oneHandTransform = this.moduleDetection.defenseCollider.transform.FindOrAddTransform("WeaponPosition", this.moduleDetection.defenseCollider.transform.position, new Quaternion?(this.moduleDetection.defenseCollider.transform.rotation));
            this.oneHandAnchor = this.oneHandTransform.FindOrAddTransform("WeaponAnchor", this.oneHandTransform.position, new Quaternion?(this.oneHandTransform.rotation));
            creature.ragdoll.ik.handRightEnabled = true;
            creature.ragdoll.ik.handLeftEnabled = true;
            creature.ragdoll.ik.enabled = true;
            //var handle = creature.handRight.handles[0];
            //creature.ragdoll.ik.SetShoulderAnchor(Side.Right, handle.bodyAnchor);
            /*creature.handRight.ragdoll.ik.SetShoulderState(Side.Left, true, true);
            creature.handRight.ragdoll.ik.SetShoulderWeight(Side.Left, 1f,1f);*/
            
            instantiated = true;
        }

        public void Possess()
        {
            creature.brain.Stop();
            Transform transform1 = this.oneHandTransform;
            Transform anchor = this.oneHandAnchor;
            Transform transform2 = this.creature.GetHand(Side.Right).transform;
            UnityEngine.Vector3 position1;
            Quaternion rotation1;
            transform2.GetPositionAndRotation(out position1, out rotation1);
            UnityEngine.Vector3 position2;
            Quaternion rotation2;
            transform1.GetPositionAndRotation(out position2, out rotation2);
            transform1.SetPositionAndRotation(Player.currentCreature.ragdoll.ik.handRightTarget.localPosition, Player.currentCreature.ragdoll.ik.handRightTarget.localRotation);
            this.creature.ragdoll.ik.SetHandAnchor(Side.Right, anchor, transform2.localRotation);
            var anchorRight = GameObject.Instantiate(new GameObject()).transform;
            creature.ragdoll.ik.handRightTarget.transform.localPosition = Player.currentCreature.ragdoll.ik.handRightTarget.localPosition + Vector3.up;
        }

        public void Unpossess()
        {
        }

        private void FixedUpdate()
        {
            if (instantiated)
            {
                Possess();
                //relativeTargetDirectionRight = (Player.local.creature.handRight.transform.position - Player.local.creature.transform.position);
                /*relativeTargetDirectionLeft = (Player.local.creature.ragdoll.ik.handLeftTarget.transform.position -
                                         Player.local.creature.handLeft.transform.position);
                creature.ragdoll.ik.handLeftTarget.transform.position = creature.handLeft.transform.position + relativeTargetDirectionLeft;
                creature.ragdoll.ik.handRightTarget.transform.position = creature.handRight.transform.position + relativeTargetDirectionRight;*/

            }
        }
    }
}