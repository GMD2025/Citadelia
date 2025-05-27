using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Gameplay.Health
{
    public class HealthController : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        public int MaxHealth => maxHealth;

        public NetworkVariable<int> Health = new(
            value: 100,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        public event Action<int, int> OnHealthChange;
        public event Action OnDied;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
                Health.Value = maxHealth;

            Health.OnValueChanged += OnHealthChanged;

            if (IsClient)
                OnHealthChange?.Invoke(Health.Value, maxHealth);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Health.OnValueChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(int previous, int current)
        {
            Debug.Log($"[HealthController] {gameObject.name} health changed: {previous} -> {current}");
            OnHealthChange?.Invoke(current, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (!IsServer) return;

            int newHealth = Mathf.Max(Health.Value - amount, 0);
            Health.Value = newHealth;

            if (newHealth <= 0)
                Die();
        }

        private void Die()
        {
            Debug.Log($"{gameObject.name} died.");
            OnDied?.Invoke();

            if (IsServer)
                NetworkObject.Despawn();
        }
    }
}