using _Scripts.ResourceSystem.Config;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ResourceSystem
{
    public class ResourceProducer : MonoBehaviour
    {
        [SerializeField] private ResourceProductionServiceSO resourceProdServiceSo;
        [SerializeField] private ResourceProductionConfigSO productionConfig;

        private IntervalExecutor executor;
        private ResourceProductionService resourceProdService;

        private void Start()
        {
            resourceProdService = resourceProdServiceSo.Get;
            executor = new IntervalExecutor(this, productionConfig.intervalSeconds, ProduceResource);
            executor.Start();
        }

        private void ProduceResource()
        {
            resourceProdService.AddResource(productionConfig.resourceType, productionConfig.productionAmount);
            Debug.Log($"[{gameObject.name}] Produced {productionConfig.productionAmount} {productionConfig.resourceType.resourceName}. Total: {resourceProdService.GetResourceAmount(productionConfig.resourceType)}");
        }
    }
}