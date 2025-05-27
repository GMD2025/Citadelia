using _Scripts.Data;
using _Scripts.Gameplay.Health;
using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Gameplay
{
    [RequireComponent(typeof(NetworkObject))]
    public class UnitAttackController : NetworkBehaviour
    {
        [SerializeField] private UnitAttackData data;

        private AIUnitTargetMovementController movement;
        private HealthController currentTarget;
        private NetworkObject selfNetworkObject;

        public override void OnNetworkSpawn()
        {
            movement = GetComponent<AIUnitTargetMovementController>();
            selfNetworkObject = GetComponent<NetworkObject>();
            IntervalRunner.Start(this, () => data.AttackInterval, TryAttack);
        }

        private void OnDisable()
        {
            IntervalRunner.StopAll(this);
        }

        private void TryAttack()
        {
            if (!IsServer)
            {
                return;
            }
            currentTarget = movement.CurrentTarget;
            if (!currentTarget || currentTarget.Health.Value <= 0)
                return;

            if (!currentTarget.TryGetComponent<NetworkObject>(out var netObj))
                return;
            if (netObj.OwnerClientId == selfNetworkObject.OwnerClientId)
                return;

            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (dist > data.AttackRadius)
                return;

            currentTarget.TakeDamage(data.DamagePerHit);
            Debug.Log($"{name} attacked {currentTarget.name} for {data.DamagePerHit}. Health is {currentTarget.Health}");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data.AttackRadius);
        }
    }
}