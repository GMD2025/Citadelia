using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Buildings
{
    public class TrainUnitController : MonoBehaviour
    {
        // TODO think if we can benefit from introducing SO for interval and unit prefab
        [SerializeField] private float trainInterval = 4f;
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform spawnPoint;

        private IntervalExecutor intervalExecutor;

        private void Start()
        {
            intervalExecutor = new IntervalExecutor(this, trainInterval, TrainWarrior);
            intervalExecutor.Start();
        }

        private void TrainWarrior()
        {
            Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}