using System;
using _Scripts.ResourceSystem.Data;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ResourceSystem
{
    public class ResourceProducer : MonoBehaviour
    {
        [SerializeField] private ResourceProductionServiceRefData resourceProdServiceRefData;
        [SerializeField] private ResourceProductionData resourceProdData;
        [SerializeField, Tooltip("Resource production animation")] private GameObject resourceProdUIPrefab;
        
        private ResourceProductionService resourceProdService;

        private void Start()
        {
            resourceProdService = resourceProdServiceRefData.Get;
            IntervalRunner.Start(this, () => resourceProdData.intervalSeconds, ProduceResource);
        }

        private void OnDisable()
        {
            IntervalRunner.StopAll(this);
        }

        private void ProduceResource()
        {
            resourceProdService.AddResource(resourceProdData.resourceType, resourceProdData.productionAmount);
            var popupInstance = Instantiate(resourceProdUIPrefab, transform.position, Quaternion.identity, transform);
            popupInstance.GetComponent<ResourceProductionAnimation>().SetResourceData(resourceProdData);
        }
    }
}