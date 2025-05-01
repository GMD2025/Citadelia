using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class TrainUnitController : NetworkBehaviour
    {
        [SerializeField] private float trainInterval = 4f;
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform spawnPoint;

        private static GameObject unitParentCachedInstance;

        public static GameObject UnitParentInstance
        {
            get
            {
                if (!unitParentCachedInstance)
                {
                    unitParentCachedInstance = new GameObject("Troops");
                }

                return unitParentCachedInstance;
            }
        }

        private void Start()
        {
            if (!IsServer) return;
            IntervalRunner.Start(this, () => trainInterval, TrainWarrior);
        }

        private void OnDisable()
        {
            if (!IsServer) return;
            IntervalRunner.StopAll(this);
        }

        private void TrainWarrior()
        {
            var go = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation, UnitParentInstance.transform);
            go.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        }
    }
}