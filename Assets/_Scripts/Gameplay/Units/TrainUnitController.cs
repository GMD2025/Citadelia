using _Scripts.Data;
using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

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
            if (unitsAlive >= spawnerData.AliveUnitsNumber)
                return;

            Vector3 spawnPos = spawnPointTransform.position;
            Quaternion spawnRot = spawnPointTransform.rotation;

            // Ensure the spawn point is on or near the NavMesh
            if (!NavMesh.SamplePosition(spawnPos, out var hit, 2f, NavMesh.AllAreas))
            {
                Debug.LogWarning($"[{nameof(TrainUnitController)}] Could not find valid NavMesh near spawn point.");
                return;
            }

            spawnPos = hit.position;

            var go = Instantiate(spawnerData.UnitPrefab, spawnPos, spawnRot);
            go.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
            go.transform.SetParent(transform, worldPositionStays: true);
            unitsAlive++;
            LifecycleHooks.OnDestroy(go) += () => unitsAlive--;
        }

    }
}