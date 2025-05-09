using UnityEngine;

namespace _Scripts.Data
{
    [CreateAssetMenu(fileName = "SpawnerData", menuName = "Scriptable Objects/Buildings/Data/Unit Spawner Data")]
    public class UnitSpawnerData: ScriptableObject
    {
        [SerializeField] public float TrainInterval = 4f;
        [SerializeField] public GameObject UnitPrefab;
        [SerializeField, Tooltip("Number of total alive units that can be produced before taking the pause until at least on of the units dye.")] 
        public uint AliveUnitsNumber;
    }
}