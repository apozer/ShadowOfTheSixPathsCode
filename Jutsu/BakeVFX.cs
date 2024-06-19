using System;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace Jutsu
{
    public class BakeVFX : MonoBehaviour
    {
        private VisualEffect vfx;
        private Item item;

        private void Start()
        {
            item = GetComponentInParent<Item>();
            vfx = GameObject.Instantiate(JutsuEntry.local.vacuumBlade.GetComponent<VisualEffect>());
            vfx.Stop();
            Tuple<RenderTexture, Vector3, Vector3> tuple = JutsuEntry.local.bakeTool.SetupItemBake(item);
            Vector3 currentSize = this.vfx.GetVector3("size");
            this.vfx.SetTexture("sdfTexture", tuple.Item1);
            this.vfx.SetVector3("size", tuple.Item2);
            this.vfx.SetVector3("center", tuple.Item3);
            vfx.gameObject.transform.parent = item.transform;
            vfx.gameObject.transform.rotation = item.transform.rotation;
            vfx.gameObject.transform.position = tuple.Item3;
            vfx.gameObject.transform.localScale = item.transform.localScale * 200;
            item.OnGrabEvent += OnGrab;
            item.OnUngrabEvent += UnGrab;
        }

        private void UnGrab(Handle handle, RagdollHand ragdollhand, bool throwing)
        {
            this.vfx.Stop();
        }

        private void OnGrab(Handle handle, RagdollHand ragdollhand)
        {
            this.vfx.Play();
        }
    }
}