using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Data;
using UnityEngine;

namespace _Scripts.Gameplay.ResourceSystem
{
    public class ResourceProductionService
    {
        private Dictionary<ResourceData, int> resourceStorage = new Dictionary<ResourceData, int>();

        public ResourceProductionService(ResourceData[] resources)
        {
            foreach (var resource in resources)
            {
                if (!resourceStorage.ContainsKey(resource))
                    resourceStorage[resource] = resource.initialAmount;
            }
        }

        public event Action<ResourceData, int> OnResourceChanged;

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
            Debug.Log(string.Join("\n", resourceStorage.Select((key, val) => $"{key.Key.name} : {val}")));

            Debug.Log("Attempting to spend resources...");

            foreach (var resource in resources)
            {
                if (!resourceStorage.ContainsKey(resource.resourceData))
                {
                    Debug.LogWarning($"Missing resource: {resource.resourceData.name}");
                    return false;
                }

                float currentAmount = resourceStorage[resource.resourceData];
                if (currentAmount < resource.amount)
                {
                    Debug.LogWarning(
                        $"Not enough of resource: {resource.resourceData.name}. Needed: {resource.amount}, Available: {currentAmount}");
                    return false;
                }
            }

            foreach (var resource in resources)
            {
                resourceStorage[resource.resourceData] -= resource.amount;
                Debug.Log(
                    $"Spent {resource.amount} of {resource.resourceData.name}. Remaining: {resourceStorage[resource.resourceData]}");
                OnResourceChanged?.Invoke(resource.resourceData, resourceStorage[resource.resourceData]);
            }

            return true;
        }
    }
}