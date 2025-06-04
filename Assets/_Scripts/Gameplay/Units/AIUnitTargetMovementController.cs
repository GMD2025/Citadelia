using System.Linq;
using _Scripts.Gameplay.Health;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Gameplay.Units
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
            // only the server drives AI movement & targeting
            if (!IsServer) return;

            if (!navMeshAgent.isOnNavMesh)
            {
                MoveToNavmesh();
                return;
            }

            if (!CurrentTarget || CurrentTarget.Health.Value <= 0)
                CurrentTarget = FindNewTarget();

            if (CurrentTarget)
                navMeshAgent.SetDestination(CurrentTarget.transform.position);
        }

        private HealthController FindNewTarget()
        {
            int myTeam = GetComponent<Team>().TeamId.Value;

            return FindObjectsByType<HealthController>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None)
                .Where(h => h.Health.Value > 0)
                .Where(h =>
                {
                    var t = h.GetComponent<Team>();
                    return t != null && t.TeamId.Value != myTeam;
                })
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
