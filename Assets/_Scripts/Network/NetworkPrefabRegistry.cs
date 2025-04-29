using System.Collections.Generic;
using _Scripts.Data;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Network
{
    public class NetworkPrefabRegistry : MonoBehaviour
    {
        public static NetworkPrefabRegistry Instance { get; private set; }

        private readonly Dictionary<int, GameObject> registry = new();
        private bool initialized;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Init(); // Force eager init at scene load
        }

        public void Init()
        {
            if (initialized) return;

            var netManager = NetworkManager.Singleton;
            if (netManager == null || netManager.NetworkConfig == null)
            {
                Debug.LogError("NetworkManager or NetworkConfig not available during Init.");
                return;
            }

            foreach (var netPrefab in netManager.NetworkConfig.Prefabs.Prefabs)
            {
                if (netPrefab?.Prefab == null)
                {
                    Debug.LogWarning("Null prefab reference found in NetworkConfig.");
                    continue;
                }

                var prefab = netPrefab.Prefab.gameObject;
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

                if (idHolder == null)
                {
                    Debug.LogWarning($"Prefab '{prefab.name}' has no component implementing IHaveId.");
                    continue;
                }

                int id = ComputeStableHash(prefab.name);
                idHolder.Id = id;

                if (!registry.TryAdd(id, prefab))
                {
                    Debug.LogWarning($"Duplicate prefab ID detected for '{prefab.name}' (ID: {id}).");
                }
            }

            initialized = true;
            Debug.Log($"NetworkPrefabRegistry initialized with {registry.Count} prefabs.");
        }

        public GameObject Get(int id)
        {
            if (!initialized)
            {
                Debug.LogWarning("Registry was not initialized. Forcing Init().");
                Init();
            }

            if (!registry.TryGetValue(id, out var go))
            {
                Debug.LogError($"Prefab with ID {id} not found in registry.");
            }

            return go;
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
}
