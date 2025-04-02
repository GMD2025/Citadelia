using System;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Gameplay.Buildings
{
    public class TrainUnitController : MonoBehaviour
    {
        // TODO think if we can benefit from introducing SO for interval and unit prefab
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
            IntervalRunner.Start(this, () => trainInterval, TrainWarrior);
        }

        private void OnDisable()
        {
            IntervalRunner.StopAll(this);
        }

        private void TrainWarrior()
        {
            Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation, UnitParentInstance.transform);
        }
    }
}