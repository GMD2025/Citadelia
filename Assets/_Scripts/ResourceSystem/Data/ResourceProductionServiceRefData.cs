using UnityEngine;

namespace _Scripts.ResourceSystem.Data
{
    [CreateAssetMenu(fileName = "ResourceControllerRef", menuName = "Data/Service References/Resource Production")]
    public class ResourceProductionServiceRefData : ScriptableObject
    {
        [SerializeField, Tooltip("This runtime reference is set automatically at runtime by the ResourceProductionService. Do not assign manually."), Header("Don't assign")]
        private ResourceProductionService runtimeProductionService;

        public void SetRuntime(ResourceProductionService productionService)
        {
            runtimeProductionService = productionService;
        }

        public ResourceProductionService Get => runtimeProductionService;
    }
}