using System.Collections.Generic;
using System.Linq;
using _Scripts.Data;
using _Scripts.Gameplay.Health;
using _Scripts.Gameplay.Units;
using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Gameplay.Buildings.Lightning
{
    public class LightningTowerController : NetworkBehaviour
    {
        [SerializeField] private GameObject lightningPrefab;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LightningTowerData lightningData;

        private int activeStrikes = 0;
        private readonly HashSet<GameObject> recentlyStruck = new();
        private Team myTeam;

        private void Start()
        {
            IntervalRunner.Start(this,
                () => lightningData.fireDelay,
                PrepareLightningStrike);

            myTeam = GetComponent<Team>();
        }

        private void PrepareLightningStrike()
        {
            if (activeStrikes >= lightningData.maxActiveStrikes)
                return;

            var enemies = Physics2D.OverlapCircleAll(
                transform.position,
                lightningData.detectionRadius,
                enemyLayer).Where(e => e.GetComponent<Team>().TeamId != myTeam.TeamId);

            foreach (var hit in enemies)
            {
                if (activeStrikes >= lightningData.maxActiveStrikes)
                    break;

                var networkObject = hit.GetComponent<NetworkObject>();
                if (!networkObject || networkObject.OwnerClientId == OwnerClientId)
                    continue;

                var enemy = hit.gameObject;
                if (recentlyStruck.Contains(enemy))
                    continue;

                Strike(enemy);
            }
        }

        private void Strike(GameObject enemy)
        {
            var verticalOffset = lightningData.biggestFrame.bounds.size.y / 2f;
            var spawnPos = enemy.transform.position + Vector3.up * verticalOffset;

            var instance = Instantiate(
                lightningPrefab,
                spawnPos,
                Quaternion.identity,
                transform);

            activeStrikes++;
            recentlyStruck.Add(enemy);

            // destroy VFX
            IntervalRunner.RunOnce(this,
                () => lightningData.destroyDelay,
                () => RemoveLightning(instance));

            // allow re-strike after cooldown
            IntervalRunner.RunOnce(this,
                () => lightningData.strikeCooldownPerTarget,
                () => recentlyStruck.Remove(enemy));

            if (enemy.TryGetComponent<NavMeshAgent>(out var agent) &&
                enemy.TryGetComponent<HealthController>(out var health))
            {
                Stun(agent);
                health.TakeDamage(lightningData.damageOnHit);
            }
        }

        private void RemoveLightning(GameObject instance)
        {
            if (instance) Destroy(instance);
            activeStrikes = Mathf.Max(0, activeStrikes - 1);
        }

        private void Stun(NavMeshAgent agent)
        {
            if (agent == null) return;
            agent.isStopped = true;
            IntervalRunner.RunOnce(this,
                () => lightningData.stunDuration,
                () => agent.isStopped = false);
        }
    }
}
