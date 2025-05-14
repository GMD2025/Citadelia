using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Systems.Network
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkSpawnerCommands : NetworkBehaviour
    {
        [ServerRpc(RequireOwnership = false)]
        public void SpawnNetworkObjectServerRpc(int prefabId, Vector3 pos, ServerRpcParams rpcParams = default)
        {
            var prefab = NetworkPrefabRegistry.Instance.Get(prefabId);
            var str = !prefab ? "null" : "not null (OK)";
            Debug.LogWarning($"Prefab is {str}");
            if (prefab == null) return;

            var go = Instantiate(prefab.gameObject, pos, Quaternion.identity);
            ulong requester = rpcParams.Receive.SenderClientId;
            go.GetComponent<NetworkObject>().SpawnWithOwnership(requester);
            Debug.Log($"Spawned '{prefab.name}' at {pos}, owned by client {requester}");
        }
    }
}