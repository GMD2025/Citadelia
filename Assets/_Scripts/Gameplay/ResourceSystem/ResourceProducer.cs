using System.Collections.Generic;
using _Scripts.Data;
using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Gameplay.ResourceSystem
{
    public class ResourceProducer : NetworkBehaviour
    {
        [SerializeField] private ResourceProductionData resourceProdData;
        [SerializeField, Tooltip("Resource production animation")] private GameObject resourceProdUIPrefab;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            IntervalRunner.Start(this, () => resourceProdData.intervalSeconds, ProduceResourceServer);
        }


        private void OnDisable()
        {
            IntervalRunner.StopAll(this);
        }

        private void ProduceResourceServer()
        {
            if (!IsServer)
                return;

            ProduceResourceOnClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { OwnerClientId }
                }
            });
        }

        [ClientRpc]
        private void ProduceResourceOnClientRpc(ClientRpcParams rpcParams = default)
        {
            var container = global::_Scripts.Systems.Network.HostedDependencyContainerLocator.Get(OwnerClientId);
            if (!container)
            {
                Debug.LogError($"ResourceProducer: Failed to find DependencyContainer for client {OwnerClientId}");
                return;
            }

            var resourceService = container.Resolve<ResourceProductionService>();

            resourceService.AddResource(resourceProdData.resourceType, resourceProdData.productionAmount);

            CreatePopup();
        }

        private void CreatePopup()
        {
            var popupInstance = Instantiate(resourceProdUIPrefab, transform.position, Quaternion.identity, transform);

            var canvas = popupInstance.GetComponent<Canvas>();
            if (canvas)
            {
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = Camera.main;
            }

            var rectTransform = popupInstance.GetComponent<RectTransform>();
            if (rectTransform)
            {
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localRotation = Quaternion.identity;
            }

            popupInstance.GetComponent<ResourceProductionAnimation>().SetResourceData(resourceProdData);
        }
    }
}
