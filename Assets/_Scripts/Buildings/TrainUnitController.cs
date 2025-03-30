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

        private void Start()
        {
            InvokeRepeating(nameof(TrainWarrior), trainInterval, trainInterval);
        }

        private void TrainWarrior()
        {
            Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}