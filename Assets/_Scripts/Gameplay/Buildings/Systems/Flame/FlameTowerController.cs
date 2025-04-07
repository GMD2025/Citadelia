using System.Collections.Generic;
using _Scripts.Gameplay.Buildings.Systems.Flame.Data;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Gameplay.Buildings.Systems.Flame
{
    public class FlameTowerController : MonoBehaviour
    {
        [SerializeField]
        private FlameTowerData flameTowerData;

        [SerializeField, Tooltip("Flames parent object")]
        private GameObject flameRing;

        [SerializeField] private LayerMask enemyLayer;
        
        private PolygonBuilder polygonBuilder;

        private void Start()
        {
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
            Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, flameTowerData.detectionRadius, enemyLayer);
            Transform flame = GetClosestFlame(enemyCollider.transform.position);
            var flameProjectile = flame.GetComponent<FlameProjectile>();
            if (!flameProjectile || !flame || !enemyCollider)
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
            flameProjectile?.ActivateCooldown(this);
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
                flameProjectile?.ActivateCooldown(this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, flameTowerData.detectionRadius);
        }
    }
}
