using UnityEngine;

namespace _Scripts.ResourceSystem.Data
{
    [CreateAssetMenu(fileName = "NewResource", menuName = "Data/Resources/Resource", order = 1)]
    public class ResourceData : ScriptableObject
    {
        public string resourceName;
        public Sprite icon;
        public int initialAmount;
        public int maxAmount;
    }
}