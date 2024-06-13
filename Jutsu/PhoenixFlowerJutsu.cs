using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace Jutsu
{
    public class PhoenixFlowerJutsu : SpellCastProjectile
    {
        private ItemData item;
        private bool delay = false;
        public bool globalActive = true;
        private List<Item> ignoreItemsList;

        public override void Load(SpellCaster spellCaster, Level level)
        {
            base.Load(spellCaster, level);
            item = Catalog.GetData<ItemData>("PhoenixFlame");
        }

        public override void Unload()
        {
            base.Unload();
            ignoreItemsList.Clear();
            ignoreItemsList.TrimExcess();
        }

        public override void Fire(bool active)
        {
            base.Fire(active);
            if (active)
            {
                ignoreItemsList = new List<Item>();
                for (int i = 0; i < 6; i++)
                {
                    item.SpawnAsync(item1 =>
                    {
                        CastJutsu(item1); //, creature);
                    });
                }
            }
        }

        private void CastJutsu(Item spawned) //, Creature creature)
        {
            if (!ignoreItemsList.Contains(spawned))
            {
                ignoreItemsList.Add(spawned);
            }

            if (!ignoreItemsList.IsNullOrEmpty())
            {
                foreach (Item item in ignoreItemsList)
                {
                    if (item != spawned)
                        Physics.IgnoreCollision(spawned.colliderGroups[0].colliders[0],
                            item.colliderGroups[0].colliders[0]);
                }
            }

            spawned.transform.position = Player.local.head.transform.position;
            spawned.transform.rotation = Player.local.head.transform.rotation;
            spawned.physicBody.rigidBody.useGravity = false;
            spawned.IgnoreRagdollCollision(Player.local.creature.ragdoll);
            var vectorRandom = Player.local.head.transform.position + Player.local.head.transform.forward +
                               (Random.insideUnitSphere * 0.4f);
            spawned.physicBody.rigidBody.AddForce(
                -(Player.local.head.transform.position - vectorRandom).normalized * 15f, ForceMode.Impulse);
            spawned.gameObject.AddComponent<Despawn>();

            IEnumerator StartDelay()
            {
                this.delay = true;
                yield return new WaitForSeconds(0.7f);
                this.delay = false;
            }
        }
    }
}