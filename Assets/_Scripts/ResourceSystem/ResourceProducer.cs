using System.Collections;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.ResourceSystem
{
    public class ResourceProducer : MonoBehaviour
    {
        [SerializeField] private ResourceController resourceController;
        [SerializeField] private Resource resourceType;
        [SerializeField] private int productionAmount;
        [SerializeField] private float productionInterval;

        private IntervalExecutor executor;

        private void Start()
        {
            executor = new IntervalExecutor(this, productionInterval, ProduceResource);
            executor.Start();
        }

        private void ProduceResource()
        {
            resourceController.AddResource(resourceType, productionAmount);
            Debug.Log($"Resource: {resourceType.resourceName}. Produced {productionAmount}; Total {resourceController.GetResourceAmount(resourceType)}");
        }
    }
}