using System.Collections;
using UnityEngine;

namespace _Scripts.ResourceSystem
{
    public class ResourceProducer : MonoBehaviour
    {
        [SerializeField] private ResourceController resourceController;
        [SerializeField] private Resource resourceType;
        [SerializeField] private int productionAmount;
        [SerializeField] private float productionInterval;

        private Coroutine productionCoroutine;

        private void Start()
        {
            StartProduction();
        }

        public void StartProduction()
        {
            productionCoroutine ??= StartCoroutine(Produce());
        }

        public void StopProduction()
        {
            if (productionCoroutine == null) return;
            StopCoroutine(productionCoroutine);
            productionCoroutine = null;
        }

        private IEnumerator Produce()
        {
            while (true) // Keeps running until stopped
            {
                yield return new WaitForSeconds(productionInterval);
                resourceController.AddResource(resourceType, productionAmount);
                Debug.Log($"Resource: {resourceType.resourceName}. Produced {productionAmount}; Total {resourceController.GetResourceAmount(resourceType)}");
            }
        }
    }
}