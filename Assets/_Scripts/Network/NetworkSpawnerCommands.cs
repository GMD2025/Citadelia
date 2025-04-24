using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Network
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkSpawnerCommands : NetworkBehaviour
    {
        [ServerRpc(RequireOwnership = false)]
        public void SpawnNetworkObjectServerRpc(int prefabId, Vector3 pos)
        {
            var prefab = NetworkPrefabRegistry.Instance.Get(prefabId);
            var str = !prefab ? "null" : "not null (OK)";
            Debug.LogWarning($"Prefab is {str}");
            if (prefab == null) return;

            var go = Instantiate(prefab.gameObject, pos, Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn();
        }
    }
}