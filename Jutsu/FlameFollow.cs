using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ThunderRoad;
using Random = UnityEngine.Random;

namespace Jutsu
{
    public class FlameFollow : MonoBehaviour
    {
        private float elapsedTime = 0f;
        private Item item;
        private bool setupDone = false;
        private Vector3[] positions;
        private int maxIntervals = 32;
        private Vector3 pos1;
        private Vector3 pos2;
        private Vector3 pos3;
        private Rigidbody rigidBody;
        private int currentPosition = 0;
        private bool end = false;
        private Creature target;
        
        public void Setup(Creature creature)
        {
            this.target = creature;
        }
        private void Start()
        {
            item = GetComponent<Item>();
            positions = new Vector3[maxIntervals];
            rigidBody = item.physicBody.rigidBody;
            pos1 = Player.local.head.transform.position;
            pos3 = target.ragdoll.targetPart.transform.position;
            pos2 = ((pos1 + pos3) / 2);
            pos2.y = Random.Range(pos2.y - 5, pos1.y + 5f);
            float t = 0;
            for (int i = 1; i < maxIntervals + 1; i++)
            {
                t = i / (float)maxIntervals;
                Debug.Log(t);
                positions[i - 1] = GetBezierPosition(t, pos1,pos2, pos3);
                Debug.Log("Position at interval " + (i-1) +  ":" + positions[i - 1]);
            }
            setupDone = true;
            /*LineRenderer lineRenderer = new GameObject().AddComponent<LineRenderer>();
            lineRenderer.positionCount = maxIntervals;
            lineRenderer.SetPositions(positions);*/
        }

        private Vector3 ClosestPoint()
        {
            float currentMinDist = 0;
            Vector3 closestPoint = new Vector3();
            foreach (Vector3 point in positions)
            {
                if (currentMinDist != 0)
                {
                    float tempDist = Vector3.Distance(point, rigidBody.position);
                    if (tempDist < currentMinDist)
                    {
                        currentMinDist = tempDist;
                        closestPoint = point;
                    }
                }
                else
                {
                    currentMinDist = Vector3.Distance(point, rigidBody.position);
                    closestPoint = point;
                    if (closestPoint == positions[positions.Length - 1])
                    {
                        end = !end;
                    }
                }
            }

            return closestPoint;
        }
        

        private void Update()
        {
            if (!end)
            {
                Vector3 nextPoint = target.transform.position;
                Quaternion.LookRotation(nextPoint - item.flyDirRef.forward);
                rigidBody.AddForce((nextPoint - item.flyDirRef.transform.position) * 50f, ForceMode.Force);
            }
        }

        Vector3 GetBezierPosition(float t, Vector3 pos1, Vector3 pos2, Vector3 pos3)
        {
            float u = 1 - t;
            float tSquared = t * t;
            float uSquared = u * u;
            Vector3 point = (uSquared * pos1);
            point += 2 * u * t * pos2;
            point += tSquared * pos3;
            return point;
        }
    }
}