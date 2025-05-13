using System.Linq;
using _Scripts.Gameplay.Enemy;
using _Scripts.Gameplay.Health;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Gameplay
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIUnitTargetMovementController : NetworkBehaviour
    {
        private NavMeshAgent navMeshAgent;
        public HealthController CurrentTarget { get; private set; }

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            // prevent auto-registration on spawn
            navMeshAgent.enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // snap to the nearest NavMesh point
            var pos = transform.position;
            if (NavMesh.SamplePosition(pos, out var hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }
            else
            {
                Debug.LogWarning($"[{name}] could not find NavMesh near {pos}");
            }

            // now that we're on valid NavMesh, enable agent
            navMeshAgent.enabled = true;
        }

        private void Update()
        {
            if (!IsOwner) return;

            if (!navMeshAgent.isOnNavMesh)
            {
                MoveToNavmesh();
                return;
            }

            if (!CurrentTarget || CurrentTarget.Health <= 0)
                CurrentTarget = FindNewTarget();

            if (CurrentTarget)
                navMeshAgent.SetDestination(CurrentTarget.transform.position);
        }

        private HealthController FindNewTarget()
        {
            var allTargets = FindObjectsByType<HealthController>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None)
                .Where(h => h.Health > 0)
                .Where(h =>
                {
                    if (!h.TryGetComponent<NetworkObject>(out var net)) return false;
                    return net.OwnerClientId != OwnerClientId;
                });

            return allTargets
                .OrderBy(t => (transform.position - t.transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        private void MoveToNavmesh()
        {
            if (NavMesh.SamplePosition(transform.position, out var hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                navMeshAgent.Warp(hit.position);
            }
        }
    }
}
