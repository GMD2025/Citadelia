using System.Collections.Generic;
using System.Linq;
using _Scripts.Data;
using _Scripts.Gameplay.Units;
using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Gameplay.Buildings.Flame
{
    [RequireComponent(typeof(Team))]
    public class FlameTowerController : NetworkBehaviour
    {
        [SerializeField] private FlameTowerData flameTowerData;

        [SerializeField, Tooltip("Flames parent object")]
        private GameObject flameRing;

        [SerializeField] private LayerMask enemyLayer;

        private PolygonBuilder polygonBuilder;
        private Team myTeam;


        private void Start()
        {
            myTeam = GetComponent<Team>();
            polygonBuilder = flameRing.GetComponent<PolygonBuilder>();
            polygonBuilder.BuildPolygon();
            ActivateCooldownInAll();
            IntervalRunner.Start(this, () => flameTowerData.fireDelay, CheckAndFire);
            IntervalRunner.Start(this, () => flameTowerData.flameRestoreDelay, RestoreFlame);
        }

        private void OnDisable()
        {
            IntervalRunner.StopAll(this);
        }

        private void CheckAndFire()
        {
            Collider2D enemyCollider = Physics2D
                .OverlapCircleAll(transform.position, flameTowerData.detectionRadius, enemyLayer)
                .First(e => e.GetComponent<Team>().TeamId != myTeam.TeamId);
            if (!enemyCollider)
                return;

            Transform flame = GetClosestFlame(enemyCollider.transform.position);
            if (!flame)
                return;

            var flameProjectile = flame.GetComponent<FlameProjectile>();
            if (!flameProjectile)
                return;

            bool hasFired = flameProjectile.FireFlame(enemyCollider.transform.position);
            if (hasFired)
            {
                polygonBuilder.NotifyVertexInstanceRemoved(flame);
            }
        }

        private void RestoreFlame()
        {
            List<GameObject> newFlames = polygonBuilder.FillMissingInstances(1);
            if (newFlames.Count == 0)
                return;

            var flameProjectile = newFlames[0].GetComponent<FlameProjectile>();
            if (flameProjectile)
                flameProjectile.ActivateCooldown(this);
        }

        private Transform GetClosestFlame(Vector3 targetPos)
        {
            Transform closest = null;
            float minDist = float.MaxValue;
            foreach (Transform child in flameRing.transform)
            {
                float dist = Vector2.Distance(child.position, targetPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = child;
                }
            }

            return closest;
        }

        private void ActivateCooldownInAll()
        {
            foreach (var flame in polygonBuilder.Instances)
            {
                var flameProjectile = flame.GetComponent<FlameProjectile>();
                if (flameProjectile)
                    flameProjectile.ActivateCooldown(this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, flameTowerData.detectionRadius);
        }
    }
}