using _Scripts.Utils;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Server.Network
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkSpawnerCommands : NetworkBehaviour
    {
        [ServerRpc(RequireOwnership = false)]
        public void SpawnNetworkObjectServerRpc(int prefabId, Vector3 pos)
        {
            var prefabRegistry = DependencyContainer.Instance.Resolve<NetworkPrefabRegistry>();
            var prefab = prefabRegistry.Get(prefabId);
            if (prefab == null) return;

            var go = Instantiate(prefab.gameObject, pos, Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn();
        }
    }
}