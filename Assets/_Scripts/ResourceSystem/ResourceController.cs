using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.ResourceSystem
{
    public class ResourceController : MonoBehaviour
    {
        private Dictionary<Resource, int> resourceStorage = new Dictionary<Resource, int>();

        public int GetResourceAmount(Resource resource)
        {
            return resourceStorage.GetValueOrDefault(resource, resource.initialAmount);
        }

        public void AddResource(Resource resource, int amount)
        {
            resourceStorage.TryAdd(resource, resource.initialAmount); // if no key-value in dictionary - create
            resourceStorage[resource] = Math.Min(resourceStorage[resource] + amount, resource.maxAmount);
        }

        public bool SpendResource(Resource resource, int amount)
        {
            if (!resourceStorage.ContainsKey(resource) || resourceStorage[resource] < amount)
                return false; // Not enough resources
            resourceStorage[resource] -= amount;
            return true;
        }
    }
}