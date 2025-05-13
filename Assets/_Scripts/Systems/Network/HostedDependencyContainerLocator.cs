using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Systems.Network
{
    public sealed class HostedDependencyContainerLocator : NetworkBehaviour
    {
        private static readonly Dictionary<ulong, LocalDependencyContainer> containersByClientId = new();

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                containersByClientId[OwnerClientId] = LocalDependencyContainer.Instance;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
            {
                containersByClientId.Remove(OwnerClientId);
            }
        }

        public static LocalDependencyContainer Get(ulong clientId)
        {
            if (containersByClientId.TryGetValue(clientId, out var container))
            {
                return container;
            }

            Debug.LogError($"HostedDependencyContainerLocator: No container for client {clientId}");
            return null;
        }
    }
}