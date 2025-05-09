using System;
using _Scripts.Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.UI
{
    [RequireComponent(typeof(TextMeshPro))]
    public class PlayerNumUI: NetworkBehaviour
    {
        private TextMeshProUGUI playerCountText;

        public override void OnNetworkSpawn()
        {
            playerCountText = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (!IsServer) return;

            int playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            UpdatePlayerCountClientRpc(playerCount);
        }

        [ClientRpc]
        private void UpdatePlayerCountClientRpc(int count)
        {
            if(playerCountText)
                playerCountText.text = $"Players: {count}";
        }
    }
}