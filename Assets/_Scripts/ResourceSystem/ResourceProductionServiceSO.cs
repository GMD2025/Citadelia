using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.ResourceSystem
{
    [CreateAssetMenu(fileName = "ResourceControllerRef", menuName = "Data/Service References/Resource Production")]
    public class ResourceProductionServiceSO : ScriptableObject
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