using System;
using UnityEngine;
using ThunderRoad;

namespace Jutsu
{
    public class Despawn : MonoBehaviour
    {
        private Item item;
        public Despawn()
        {
            this.item = this.gameObject.GetComponentInParent<Item>();
        }
        private void OnCollisionEnter(Collision other)
        {
            Destroy(item.gameObject);
        }
    }
}