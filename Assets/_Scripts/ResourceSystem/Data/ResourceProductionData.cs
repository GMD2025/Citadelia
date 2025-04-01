using UnityEngine;

namespace _Scripts.ResourceSystem.Data
{
    [CreateAssetMenu(fileName = "NewResourceProduction", menuName = "Data/Resources/Production Config", order = 2)]
    public class ResourceProductionData : ScriptableObject
    {
        public ResourceData resourceType;
        public int productionAmount;
        public float intervalSeconds;
    }
}