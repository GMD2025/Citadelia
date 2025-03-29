using UnityEngine;
using UnityEngine.AI;

namespace _Scripts
{
    public class UnitMovementController : MonoBehaviour
    {
        [SerializeField] private Transform objectToFollow;
        private NavMeshAgent navMeshAgent;
        private Quaternion rotation;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if(!navMeshAgent.isOnNavMesh || !isActiveAndEnabled)
                Debug.LogError($"Either NavMeshAgent is not on the NavMesh or it is inactive");
            
            navMeshAgent.SetDestination(objectToFollow.transform.position);
        }
    }
}