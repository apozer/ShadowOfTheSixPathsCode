using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ThunderRoad;

namespace Jutsu
{
    public class RasenganMono : MonoBehaviour
    {
        private Item item;

        private void Start()
        {
            item = GetComponent<Item>();
        }


        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.gameObject.GetComponentInParent<RagdollHand>() is RagdollHand testParent)
            {
                //Debug.Log("Parent from collider: " + testParent);
                if (testParent.gameObject.GetComponentInParent<Creature>() is Creature testParentCreature)
                {
                    Debug.LogWarning("testParentCreature " + testParentCreature);
                }
                else if (testParent.gameObject.GetComponent<Creature>() is Creature testCreature)
                {
                    Debug.LogWarning("testParentCreature " + testCreature);
                }
                else if (testParent.gameObject.GetComponentInChildren<Creature>() is Creature testCreatureChild)
                {
                    Debug.LogWarning("testParentCreature " + testCreatureChild);
                }
                return;
            }

            if (other.collider.gameObject.GetComponentInChildren<RagdollHand>() is RagdollHand testChild)
            {
                //Debug.Log("Child from collider: " + testChild);
                if (testChild.gameObject.GetComponentInParent<Creature>() is Creature testParentCreature)
                {
                    Debug.LogWarning("testParentCreature " + testParentCreature);
                }
                else if (testChild.gameObject.GetComponent<Creature>() is Creature testCreature)
                {
                    Debug.LogWarning("testParentCreature " + testCreature);
                }
                else if (testChild.gameObject.GetComponentInChildren<Creature>() is Creature testCreatureChild)
                {
                    Debug.LogWarning("testParentCreature " + testCreatureChild);
                }
                return;
            }

            if (other.collider.gameObject.GetComponent<RagdollHand>() is RagdollHand testCurrent)
            {
                //Debug.Log("Current from collider: " + testCurrent);
                if (testCurrent.gameObject.GetComponentInParent<Creature>() is Creature testParentCreature)
                {
                    Debug.LogWarning("testParentCreature " + testParentCreature);
                }
                else if (testCurrent.gameObject.GetComponent<Creature>() is Creature testCreature)
                {
                    Debug.LogWarning("testParentCreature " + testCreature);
                }
                else if (testCurrent.gameObject.GetComponentInChildren<Creature>() is Creature testCreatureChild)
                {
                    Debug.LogWarning("testParentCreature " + testCreatureChild);
                }
                return;
            }
            if (other.collider.gameObject.GetComponentInParent<Creature>() is Creature testCParent)
            {
                if (!testCParent.isPlayer)
                {
                    testCParent.ragdoll.SetState(Ragdoll.State.Destabilized);
                    testCParent.locomotion.isGrounded = false;
                    Debug.Log("Parent creature from collider: " + testCParent);
                    foreach (Rigidbody rb in testCParent.ragdoll.parts.Select(part => part.physicBody.rigidBody))
                    {
                        rb.AddForce(Player.local.handLeft.ragdollHand.PalmDir *  (100f * rb.mass), ForceMode.Impulse);
                    }
                    item.Despawn();
                }
                return;
            }

            if (other.collider.gameObject.GetComponentInChildren<Creature>() is Creature testCChild)
            {
                if (!testCChild.isPlayer)
                {
                    testCChild.ragdoll.SetState(Ragdoll.State.Destabilized);
                    testCChild.locomotion.isGrounded = false;
                    Debug.Log("Child from collider: " + testCChild);
                    foreach (Rigidbody rb in testCChild.ragdoll.parts.Select(part => part.physicBody.rigidBody))
                    {
                        rb.AddForce(Player.local.handLeft.ragdollHand.PalmDir * (100f * rb.mass), ForceMode.Impulse);
                    }
                    item.Despawn();
                }
                return;
            }

            if (other.collider.gameObject.GetComponent<Creature>() is Creature testCCurrent)
            {
                if (!testCCurrent.isPlayer)
                {
                    testCCurrent.ragdoll.SetState(Ragdoll.State.Destabilized);
                    testCCurrent.locomotion.isGrounded = false;
                    Debug.Log("Current from collider: " + testCCurrent);
                    foreach (Rigidbody rb in testCCurrent.ragdoll.parts.Select(part => part.physicBody.rigidBody))
                    {
                        rb.AddForce(Player.local.handLeft.ragdollHand.PalmDir *  (100f * rb.mass), ForceMode.Impulse);
                    }
                    item.Despawn();
                }
                return;
            }
            Debug.LogWarning("None happened, thats why I am here.");
            RaycastHit[] array = Physics.SphereCastAll(other.transform.position, 2f, Player.local.handLeft.ragdollHand.PalmDir, 5f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            foreach (var hit in array)
            {
                if (hit.collider.gameObject.GetComponentInParent<Rigidbody>() is Rigidbody rigid && rigid.gameObject.GetComponentInParent<Creature>() is Creature c)
                {
                    //Debug.Log(c.name);
                    if (!c.isPlayer)
                    {
                        c.ragdoll.SetState(Ragdoll.State.Destabilized);
                        c.locomotion.isGrounded = false;
                        rigid.AddExplosionForce( (100f * rigid.mass), other.transform.position, 2f);
                        if(hit.Equals(array[array.Length - 1]))item.Despawn();
                    }
                }
                else
                {
                    if (hit.collider.gameObject.GetComponentInParent<Rigidbody>() is Rigidbody rb  && !hit.collider.gameObject.GetComponentInParent<RagdollHand>())
                    {
                        //Debug.Log("Here ignore RagdollHand");
                        rb.AddExplosionForce( (100f * rb.mass), other.transform.position, 2f);
                        if(hit.Equals(array[array.Length - 1]))item.Despawn();
                    }
                }
            }
        }
    }
}