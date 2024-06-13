using System;
using ThunderRoad;
using UnityEngine;

namespace Jutsu
{
    public class SubstitutionJutsu : ThunderBehaviour
    {
        private Vector3 spawnPosition;
        private Vector3 teleportPosition;
        private ItemData logData;
        private bool eventTriggered = false;
        private void Start()
        {
            spawnPosition = Player.currentCreature.ragdoll.headPart.transform.position;
            teleportPosition = new Vector3(
                Player.currentCreature.transform.position.x + UnityEngine.Random.Range(-10f,10f),
                Player.currentCreature.transform.position.y,
                Player.currentCreature.transform.position.z + UnityEngine.Random.Range(-10f, 10f));

            Player.currentCreature.OnDamageEvent += OnDamaged;
        }

        private void OnDamaged(CollisionInstance collisioninstance, EventTime eventtime)
        {
            float health = Player.local.creature.currentHealth;
                if (eventtime == EventTime.OnStart) return;

                Player.local.creature.currentHealth = health;
                JutsuEntry.local.logData.SpawnAsync(callback => {}, spawnPosition);
                Player.local.Teleport(teleportPosition, Player.currentCreature.transform.rotation);
                UnityEngine.Object.Destroy(this);
        }

        private void OnDestroy()
        {
            Player.currentCreature.OnDamageEvent -= OnDamaged;
        }
    }
}