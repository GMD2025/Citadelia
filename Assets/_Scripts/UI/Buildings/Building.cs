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
    [CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Buildings")]
    public class Building : ScriptableObject
    {
        public Sprite sprite;
        public new string name;
        public Vector2Int cellsize;
        public Resource[] resources;
    }
}
