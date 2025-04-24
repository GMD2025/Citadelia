using _Scripts.Data;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Gameplay.ResourceSystem
{
    public class ResourceProducer : MonoBehaviour
    {
        [SerializeField] private ResourceProductionData resourceProdData;
        [SerializeField, Tooltip("Resource production animation")] private GameObject resourceProdUIPrefab;
        
        private ResourceProductionService resourceProdService;

        private void Start()
        {
            resourceProdService = DependencyContainer.Instance.Resolve<ResourceProductionService>();
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
            Debug.Log($"[{gameObject.name}] Produced {resourceProdData.productionAmount} {resourceProdData.resourceType.resourceName}. Total: {resourceProdService.GetResourceAmount(resourceProdData.resourceType)}");
        }
    }
}