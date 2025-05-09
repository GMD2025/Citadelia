using _Scripts.Data;
using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class TrainUnitController : NetworkBehaviour
    {
        [SerializeField] private UnitSpawnerData spawnerData;
        [SerializeField] private Transform spawnPointTransform;


        private uint unitsAlive = 0;

        private void Start()
        {
            if (!IsServer) return;
            IntervalRunner.Start(this, () => spawnerData.TrainInterval, TrainWarrior);
        }

        private void OnDisable()
        {
            if (!IsServer) return;
            IntervalRunner.StopAll(this);
        }

        private void TrainWarrior()
        {
            if(unitsAlive >= spawnerData.AliveUnitsNumber)
                return;
            
            var go = Instantiate(spawnerData.UnitPrefab, spawnPointTransform.position,
                spawnPointTransform.rotation);
            go.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
            go.transform.SetParent(transform, worldPositionStays: true);
            unitsAlive++;
            LifecycleHooks.OnDestroy(go) += () => unitsAlive--;
        }
    }
}