using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts
{
    public class UnitMovementController : MonoBehaviour
    {
        private Transform currentTarget;
        private NavMeshAgent navMeshAgent;
        
        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (!navMeshAgent.isOnNavMesh || !isActiveAndEnabled)
            {
                Debug.LogError("Either NavMeshAgent is not on the NavMesh or it is inactive");
                return;
            }

            Transform newTarget = FindClosestFollowPoint();
            if (newTarget && newTarget != currentTarget)
            {
                currentTarget = newTarget;
                navMeshAgent.SetDestination(currentTarget.position);
            }
        }
        
        private Transform FindClosestFollowPoint()
        {
            GameObject[] followPoints = GameObject.FindGameObjectsWithTag("FollowPoint");
            if (followPoints == null || followPoints.Length == 0)
                return null;
            
            return followPoints
                .OrderBy(go => Vector3.Distance(transform.position, go.transform.position))
                .First().transform;
        }
    }
}