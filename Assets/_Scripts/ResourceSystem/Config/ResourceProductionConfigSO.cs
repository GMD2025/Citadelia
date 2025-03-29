using UnityEngine;

namespace _Scripts.ResourceSystem.Config
{
    [CreateAssetMenu(fileName = "NewResourceProduction", menuName = "Data/Resources/Production Config", order = 2)]
    public class ResourceProductionConfigSO : ScriptableObject
    {
        public ResourceSO resourceType;
        public int productionAmount;
        public float intervalSeconds;
    }
}