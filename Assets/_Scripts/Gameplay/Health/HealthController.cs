using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Gameplay.Health
{
    [RequireComponent(typeof(Team))]
    public class HealthController : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        public int MaxHealth => maxHealth;

        public NetworkVariable<int> Health = new(
            value: 100,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        private Team team;

        public event Action OnDied;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            team = GetComponent<Team>();

            if (IsServer)
                Health.Value = maxHealth;

            Health.OnValueChanged += OnHealthChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Health.OnValueChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(int previous, int current)
        {
            Debug.Log($"[HealthController] {gameObject.name} health changed: {previous} -> {current}");
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