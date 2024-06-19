using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine.VFX;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.VFX.SDF;
namespace Jutsu
{
    public class SDFBakeTool : MonoBehaviour
    {
        MeshToSDFBaker m_Baker;
        Mesh m_Mesh;
        public int maxResolution = 64;
        public GameObject center;
        public Vector3 sizeBox;
        public int signPassCount = 1;
        public float threshold = 0.5f;

        public Tuple<RenderTexture, Vector3, Vector3> SetupItemBake(Item item)
        {

            sizeBox = new Vector3(0,0,0);
            center = GameObject.Instantiate(new GameObject());
            if (item.gameObject.GetComponent<MeshFilter>() is MeshFilter meshF)
            {
                m_Mesh = meshF.mesh;
            }   
            else  m_Mesh = item.gameObject.GetComponentInChildren<MeshFilter>().mesh;
            
            List<Damager> pierce = item.gameObject.GetComponentsInChildren<Damager>().ToList();
            float pierceLength = 0f;
            Transform pierceTransform = null;
            float pierceDepth = 0f;
            bool pierceSelected = false;
            bool depthSelected = false;   
            foreach (var damager in pierce)
            {
                if (damager.direction == Damager.Direction.Forward && !pierceSelected)
                {
                    pierceLength = damager.penetrationDepth;
                    pierceSelected = true;
                    pierceTransform = damager.transform;
                }
                else if (damager.direction == Damager.Direction.ForwardAndBackward && !depthSelected)
                {
                    pierceDepth = damager.penetrationDepth;
                    depthSelected = true;
                }   
            }
            sizeBox.y = pierceLength;
            sizeBox.x = pierceDepth - (pierceDepth * 0.3f) * 20f;
            sizeBox.z =  item.colliderGroups[0].colliders[0].bounds.size.z + pierceDepth * 10f;
            if (pierceTransform)
            {
                center.transform.position = pierceTransform.position + (-(pierceTransform.forward * (pierceLength / 2f)));
            }

            center.transform.parent = item.transform;
            center.transform.rotation = item.transform.rotation;
            GameObject go = GameObject.Instantiate(JutsuEntry.local.debugObject);
            go.GetComponent<BoxCollider>().enabled = false;
            go.transform.position = center.transform.position;
            go.transform.localScale = sizeBox;
            go.transform.parent = item.transform;
            go.transform.position = center.transform.position;
            go.transform.rotation = center.transform.rotation;
            m_Baker = new MeshToSDFBaker(sizeBox, center.transform.position, maxResolution, m_Mesh, signPassCount, threshold);
            m_Baker.BakeSDF();
            return new Tuple<RenderTexture, Vector3, Vector3>(m_Baker.SdfTexture, sizeBox, center.transform.position);
        }

        void OnDestroy()
        {
            if (m_Baker != null)
            {
                m_Baker.Dispose();
            }
        }
    }
}