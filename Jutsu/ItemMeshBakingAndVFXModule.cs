using System;
using ThunderRoad;
using UnityEngine;

namespace Jutsu
{
    public class ItemMeshBakingAndVFXModule : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<BakeVFX>();
        }
    }
}