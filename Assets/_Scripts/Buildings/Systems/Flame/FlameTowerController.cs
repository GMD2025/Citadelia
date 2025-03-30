using System;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Buildings.Systems.Flame
{
    public class FlameTowerController : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 5f;
        [SerializeField] private float fireDelay = 2f;
        [SerializeField] private float flameRestoreDelay = 10f;
        [SerializeField, Tooltip("Flames parent object")] private GameObject flameRing;
        [SerializeField] private LayerMask enemyLayer;
        
        private PolygonBuilder triangleBuilder;
        private void Start()
        {
            triangleBuilder = flameRing.GetComponent<PolygonBuilder>();
            IntervalRunner.Start(this, () => fireDelay, CheckAndFire);
            IntervalRunner.Start(this, () => flameRestoreDelay, RestoreFlame);
        }

        private void OnDisable()
        {
            IntervalRunner.Stop(this, CheckAndFire);
            IntervalRunner.Stop(this, RestoreFlame);
        }

        private void CheckAndFire()
        {
            Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, enemyLayer);
            if (!enemyCollider)
                return;

            Transform flame = GetClosestFlame(enemyCollider.transform.position);
            if (!flame)
                return;

            var flameProjectile = flame.GetComponent<FlameProjectile>();
            flameProjectile.FireFlame(enemyCollider.transform.position);
            triangleBuilder.NotifyVertexInstanceRemoved(flame);
        }

        private void RestoreFlame()
        {
            triangleBuilder.FillMissingInstances(1);
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
