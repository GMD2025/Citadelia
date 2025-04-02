using _Scripts.Gameplay.Buildings.Systems.Flame.Data;
using _Scripts.Gameplay.Health;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Gameplay.Buildings.Systems.Flame
{
    public enum FlameState
    {
        Cooldown,
        Ready,
        Fired
    }

    public class FlameProjectile : MonoBehaviour
    {
        [SerializeField] private FlameTowerData flameTowerData;
        
        private FlameState state;
        private bool hasHit;

        private void Awake()
        {
            state = FlameState.Cooldown;
            hasHit = false;
        }

        public bool FireFlame(Vector3 targetPos)
        {
            if (state != FlameState.Ready)
                return false;

            state = FlameState.Fired;

            var rb = GetComponent<Rigidbody2D>();
            transform.parent = null;
            rb.gravityScale = 0f;
            Vector2 direction = ((Vector2)(targetPos - transform.position)).normalized;
            rb.linearVelocity = direction * flameTowerData.speed;

            Destroy(gameObject, 5f);
            return true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (state == FlameState.Fired && !hasHit && collision.gameObject.CompareTag("Enemy"))
            {
                hasHit = true;
                var healthController = collision.gameObject.GetComponent<HealthController>();
                if (healthController != null)
                {
                    healthController.TakeDamage(flameTowerData.damageOnHit);
                }
                Destroy(gameObject);
            }
        }

        public void ActivateCooldown(MonoBehaviour host)
        {
            state = FlameState.Cooldown;
            IntervalRunner.RunOnce(host, () => flameTowerData.cooldownAfterRestore, () =>
            {
                state = FlameState.Ready;
            });
        }
    }
}