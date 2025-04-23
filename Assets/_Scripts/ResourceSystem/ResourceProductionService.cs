using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using _Scripts.ResourceSystem.Data;
using _Scripts.UI.Buildings;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ResourceSystem
{
    public class ResourceProductionService
    {
        [SerializeField] private ResourceProductionServiceRefData resourceProdServiceRefData;

        private Dictionary<ResourceData, int> resourceStorage = new Dictionary<ResourceData, int>();
        public event Action<ResourceData, int> OnResourceChanged;

        private void Awake()
        {
            resourceProdServiceRefData.SetRuntime(this);
        }

        public int GetResourceAmount(ResourceData resourceData)
        {
            return resourceStorage.GetValueOrDefault(resourceData, resourceData.initialAmount);
        }

        public void AddResource(ResourceData resourceData, int amount)
        {
            resourceStorage.TryAdd(resourceData, resourceData.initialAmount);
            resourceStorage[resourceData] = Math.Min(resourceStorage[resourceData] + amount, resourceData.maxAmount);
            OnResourceChanged?.Invoke(resourceData, resourceStorage[resourceData]);
        }

        public bool SpendResource(ResourceData resourceData, int amount)
        {
            if (!resourceStorage.ContainsKey(resourceData) || resourceStorage[resourceData] < amount)
                return false;

            resourceStorage[resourceData] -= amount;
            OnResourceChanged?.Invoke(resourceData, resourceStorage[resourceData]);
            return true;
        }

        public bool SpendResources(Resource[] resources)
        {
            foreach (var resource in resources)
            {
                if (!resourceStorage.ContainsKey(resource.resourceData) ||
                    resourceStorage[resource.resourceData] < resource.amount)
                    return false;
            }

            foreach (var resource in resources)
            {
                resourceStorage[resource.resourceData] -= resource.amount;
                OnResourceChanged?.Invoke(resource.resourceData, resourceStorage[resource.resourceData]);
            }

            return true;
        }
    }
}