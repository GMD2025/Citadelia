using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Buildings
{
    public class WarriorBuildingController : MonoBehaviour
    {
        [SerializeField] private float trainInterval = 4f;
        [SerializeField] private GameObject warriorPrefab;
        [SerializeField] private Transform spawnPoint;

        private IntervalExecutor intervalExecutor;

        private void Start()
        {
            intervalExecutor = new IntervalExecutor(this, trainInterval, TrainWarrior);
            intervalExecutor.Start();
        }

        private void TrainWarrior()
        {
            Instantiate(warriorPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}