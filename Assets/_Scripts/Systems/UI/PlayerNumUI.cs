using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Systems.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PlayerNumUI: NetworkBehaviour
    {
        private TextMeshProUGUI playerCountText;

        public override void OnNetworkSpawn()
        {
            playerCountText = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (!IsServer || !playerCountText) return;

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