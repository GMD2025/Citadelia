using System.Linq;
using _Scripts.Gameplay.Enemy;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Gameplay
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIUnitTargetMovementController : NetworkBehaviour
    {
        private NavMeshAgent navMeshAgent;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (!navMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning("The unit is not on NavMesh. Will try to move it to the closest point on the navmesh.");
                MoveToNavmesh();
                return;
            }

            Transform target = FindClosestEnemyTarget();
            if (!target)
                return;

            navMeshAgent.SetDestination(target.position);
        }

        private Transform FindClosestEnemyTarget()
        {
            var enemies = FindObjectsByType<CastleController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Where(c => c.OwnerClientId != OwnerClientId)
                .Select(c => c.transform);

            return enemies
                .OrderBy(t => (transform.position - t.position).sqrMagnitude)
                .FirstOrDefault();
        }

        private void MoveToNavmesh()
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 100f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                navMeshAgent.Warp(hit.position);
            }
        }
    }
}