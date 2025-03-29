using UnityEngine;
using _Scripts.Utils;

namespace _Scripts.Buildings
{
    public class FlameRingController : MonoBehaviour
    {
        public float detectionRadius = 5f;
        public float flameSpeed = 10f;
        public float fireDelay = 1f;
        public LayerMask enemyLayer;

        private IntervalExecutor intervalExecutor;

        private void Start()
        {
            intervalExecutor = new IntervalExecutor(this, fireDelay, CheckAndFire);
            intervalExecutor.Start();
        }

        private void OnDestroy() =>
            intervalExecutor?.Stop();

        private void CheckAndFire()
        {
            Collider2D enemy = Physics2D.OverlapCircle(transform.position, detectionRadius, enemyLayer);
            if (!enemy) return;
            Transform flame = GetClosestFlame(enemy.transform.position);
            if (!flame) return;
            FireFlame(flame, enemy.transform.position);
        }

        private Transform GetClosestFlame(Vector3 targetPos)
        {
            Transform closest = null;
            float minDist = float.MaxValue;
            foreach (Transform child in transform)
            {
                if (!child.gameObject.activeInHierarchy) continue;
                float dist = Vector2.Distance(child.position, targetPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = child;
                }
            }
            return closest;
        }

        private void FireFlame(Transform flame, Vector3 targetPos)
        {
            flame.parent = null;
            Rigidbody2D rb = flame.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearVelocity = ((Vector2)(targetPos - flame.position)).normalized * flameSpeed;
            Destroy(flame.gameObject, 5f);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}