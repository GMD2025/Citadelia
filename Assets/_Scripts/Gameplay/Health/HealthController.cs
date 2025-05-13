using System;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Gameplay.Health
{
    public class HealthController : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        public int MaxHealth => maxHealth;
        public int Health
        {
            get => health;
            set
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                OnHealthChange?.Invoke(health, maxHealth);
            }
        }

        private int health;

        public event Action<int, int> OnHealthChange;
        public event Action OnDied;

        private void Awake()
        {
            health = maxHealth;
            OnHealthChange?.Invoke(health, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            OnHealthChange?.Invoke(health, maxHealth);
            if (health <= 0)
                Die();
        }

        public void HealBy(int amount)
        {
            health = Mathf.Clamp(health + amount, 0, maxHealth);
            OnHealthChange?.Invoke(health, maxHealth);
        }

        private void Die()
        {
            OnDied?.Invoke();
            if (IsServer)
                NetworkObject.Despawn();
        }
    }
}