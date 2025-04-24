using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Server.Network
{
    public class ConnectionHandlerUI : NetworkBehaviour
    {
        [SerializeField]
        private Button hostButton;
        [SerializeField]
        private Button clientButton;

        [SerializeField] private TextMeshProUGUI playersCountText;

        private NetworkVariable<int> playersNum = new NetworkVariable<int>();

    
    
        private void Awake()
        {
            hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
            clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
        }

        private void Update()
        {
            if(!IsServer) return;
            playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
            playersCountText.text = $"Players: {playersNum.Value.ToString()}";
        }
    }
}
