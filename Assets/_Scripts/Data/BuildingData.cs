using _Scripts.Gameplay.Buildings;
using UnityEngine;

namespace _Scripts.Data
{
    [System.Serializable]
    public struct Resource
    {
        public ResourceData resourceData;
        public int amount;
    }
    [CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Buildings/UI")]
    public class BuildingData: ScriptableObject
    {
        public Vector2Int cellsize;
        
        [Header("Resources")]
        public Resource[] resources;
    }
}
