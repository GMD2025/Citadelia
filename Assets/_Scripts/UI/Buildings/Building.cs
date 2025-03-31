using UnityEngine;

namespace _Scripts.UI.Buildings
{
    [CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Buildings")]
    public class Building : ScriptableObject
    {
        public Sprite sprite;
        public string name;
        public Vector2Int cellsize;
    }
}
