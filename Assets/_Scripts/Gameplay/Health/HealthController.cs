using System;
using UnityEngine;

namespace _Scripts.Gameplay.Health
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField] private int startingHealth = 100;

        public int StartingHealth => startingHealth;
        public int Health => health;

        private int health;

        public event Action OnDied;
        public event Action OnHealthChange;

        private void Awake()
        {
            health = startingHealth;
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            OnHealthChange?.Invoke();

            if (health <= 0)
            {
                Die();
            }
        }


        private void Die()
        {
            OnDied?.Invoke();
            Destroy(gameObject);
        }

        public void HealBy(int amount)
        {
            health = Mathf.Clamp(health + amount, 0, startingHealth);
            OnHealthChange?.Invoke();
        }
    }
}