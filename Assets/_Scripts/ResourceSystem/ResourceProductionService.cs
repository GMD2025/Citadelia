using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ResourceSystem
{
    public class ResourceProductionService : MonoBehaviour
    {
        [SerializeField] private ResourceProductionServiceSO resourceProdService;

        private Dictionary<ResourceSO, int> resourceStorage = new Dictionary<ResourceSO, int>();
        public event Action<ResourceSO, int> OnResourceChanged;

        private void Awake()
        {
            resourceProdService.SetRuntime(this);
        }

        public int GetResourceAmount(ResourceSO resourceSo)
        {
            return resourceStorage.GetValueOrDefault(resourceSo, resourceSo.initialAmount);
        }

        public void AddResource(ResourceSO resourceSo, int amount)
        {
            resourceStorage.TryAdd(resourceSo, resourceSo.initialAmount);
            resourceStorage[resourceSo] = Math.Min(resourceStorage[resourceSo] + amount, resourceSo.maxAmount);
            
            if (OnResourceChanged == null)
                Debug.Log("Pizda");
            OnResourceChanged?.Invoke(resourceSo, resourceStorage[resourceSo]);
        }

        public bool SpendResource(ResourceSO resourceSo, int amount)
        {
            if (!resourceStorage.ContainsKey(resourceSo) || resourceStorage[resourceSo] < amount)
                return false;

            resourceStorage[resourceSo] -= amount;
            OnResourceChanged?.Invoke(resourceSo, resourceStorage[resourceSo]);
            return true;
        }
    }
}