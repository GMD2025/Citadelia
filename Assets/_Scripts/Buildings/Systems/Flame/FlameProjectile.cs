using System;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Buildings.Systems.Flame
{
    public class FlameProjectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;

        public void FireFlame(Vector3 targetPos)
        {
            var rb = GetComponent<Rigidbody2D>();
            transform.parent = null;

            if (!rb)
            {
                return;
            }

            rb.gravityScale = 0f;
            Vector2 direction = ((Vector2)(targetPos - transform.position)).normalized;
            rb.linearVelocity = direction * speed;
            Destroy(gameObject, 5f);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                // TODO: damage! copy health system from shooter game
                Destroy(other.gameObject);
                Destroy(gameObject, 0.2f);
            }
        }
    }
}