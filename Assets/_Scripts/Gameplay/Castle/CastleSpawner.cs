using System;
using System.Linq;
using _Scripts.Gameplay.Health;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.Gameplay.Enemy
{
    [RequireComponent(typeof(Grid))]
    public class CastleSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject castlePrefab;
        private Grid grid;

        private void Awake()
        {
            grid = GetComponent<Grid>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    
            TrySpawnCastles();
        }

        private void OnClientConnected(ulong clientId)
        {
            TrySpawnCastles();
        }

        private void TrySpawnCastles()
        {
            if (NetworkManager.Singleton.ConnectedClients.Count < 2)
                return;

            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;

            float offset = GetTallestTilemapHeight();

            var clients = NetworkManager.Singleton.ConnectedClientsList;
            SpawnFor(clients[0].ClientId, Vector3.down * offset);
            SpawnFor(clients[1].ClientId, Vector3.up * offset);
        }

        private void SpawnFor(ulong ownerId, Vector3 worldPos)
        {
            var castle = Instantiate(castlePrefab, worldPos, Quaternion.identity);
            var netObj = castle.GetComponent<NetworkObject>();
            netObj.Spawn();
            netObj.GetComponent<Team>().SetTeam((int) ownerId);

        }

        private int GetTallestTilemapHeight()
        {
            return grid.GetComponentsInChildren<Tilemap>().Max(tm => tm.cellBounds.yMax);
        }
    }
}