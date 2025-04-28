using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Gameplay
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIUnitTargetMovementController : MonoBehaviour
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
                Debug.LogError("Either NavMeshAgent is not on the NavMesh or gameobject is inactive");
                return;
            }

            Transform target = FindClosestFollowPoint();
            navMeshAgent.SetDestination(target.position);
        }

        private Transform FindClosestFollowPoint()
        {
            var followPoints = GameObject.FindGameObjectsWithTag("FollowPoint");
            if (followPoints == null || followPoints.Length == 0)
                return null;

            NavMeshHit hit;
            return followPoints
                .Where(go => NavMesh.SamplePosition(go.transform.position, out hit, 1.0f, NavMesh.AllAreas))
                .OrderBy(go => Vector3.SqrMagnitude(transform.position - go.transform.position))
                .FirstOrDefault()?.transform;
        }

    }
}