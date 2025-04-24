using System.Collections.Generic;
using _Scripts.Data;
using Unity.Netcode;
using UnityEngine;

public class NetworkPrefabRegistry
{
    private readonly Dictionary<int, GameObject> registry = new();

    private bool initialized = false;

    public void Init()
    {
        if (initialized) return;

        var netManager = NetworkManager.Singleton;
        if (netManager == null)
        {
            Debug.LogError("NetworkManager not yet available.");
            return;
        }

        foreach (var np in netManager.NetworkConfig.Prefabs.Prefabs)
        {
            var prefab = np.Prefab.gameObject;
            var components = prefab.GetComponents<NetworkBehaviour>();

            IHaveId idHolder = null;
            foreach (var comp in components)
            {
                if (comp is IHaveId candidate)
                {
                    idHolder = candidate;
                    break;
                }
            }

            if (idHolder == null) continue;

            int id = ComputeStableHash(prefab.name);
            idHolder.Id = id;

            registry.TryAdd(id, prefab);
        }

        initialized = true;
    }

    public GameObject Get(int id)
    {
        Init(); // Lazy init
        return registry.GetValueOrDefault(id);
    }
    
    private static int ComputeStableHash(string s)
    {
        unchecked
        {
            const uint fnvOffset = 2166136261;
            const uint fnvPrime = 16777619;
            uint hash = fnvOffset;
            foreach (var c in s)
                hash = (hash ^ c) * fnvPrime;
            return (int)hash;
        }
    }
}