using System.Collections.Generic;
using _Scripts.ResourceSystem.Data;
using UnityEngine;

namespace _Scripts.UI.Buildings
{
    [System.Serializable]
    public struct Resource
    {
        public ResourceData resourceData;
        public int amount;
    }
    [CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Buildings/UI")]
    public class BuildingData : ScriptableObject
    {
        public GameObject buildingPrefab;
        public Vector2Int cellsize;
        
        [Header("Resources")]
        public Resource[] resources;

        public Sprite Sprite => buildingPrefab.GetComponent<SpriteRenderer>().sprite;
    }
}
