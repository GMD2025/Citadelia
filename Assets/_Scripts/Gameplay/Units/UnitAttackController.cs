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

        private void Start()
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
            currentTarget = movement.CurrentTarget;
            if (!currentTarget || currentTarget.Health <= 0)
                return;

            if (!currentTarget.TryGetComponent<NetworkObject>(out var netObj))
                return;
            if (netObj.OwnerClientId == selfNetworkObject.OwnerClientId)
                return;

            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (dist > data.AttackRadius)
                return;

            currentTarget.TakeDamage(data.DamagePerHit);
            Debug.Log($"{name} attacked {currentTarget.name} for {data.DamagePerHit}");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data.AttackRadius);
        }
    }
}