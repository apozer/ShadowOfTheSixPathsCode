using System;
using UnityEngine;
using ThunderRoad;
using ThunderRoad.Skill.Spell;

namespace Jutsu
{
    public class WaterSharkBombJutsu : SpellCastProjectile
    {
        private ItemData itemData;
        public override void Load(SpellCaster spellCaster)
        {
            base.Load(spellCaster);
            itemData = Catalog.GetData<ItemData>("WaterShark");
        }

        public override void Fire(bool active)
        {
            base.Fire(active);
            if (!active) return;
            this.itemData.SpawnAsync(CastJutsu);
        }

        private void CastJutsu(Item spawned)
        {
            spawned.transform.position = Player.local.head.transform.position + (Player.local.transform.position * 2f);
            spawned.physicBody.rigidBody.useGravity = false;
            spawned.physicBody.rigidBody.AddForce(Player.local.head.transform.position * 20f, ForceMode.Impulse);
        }
    }
}