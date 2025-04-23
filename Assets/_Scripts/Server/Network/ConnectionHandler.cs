using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Server.Network
{
    [DisallowMultipleComponent]
    public class ConnectionHandler : MonoBehaviour
    {
        [SerializeField] private int maxConnections = 2;

        private NetworkManager net;

        private void Start()
        {
            net = NetworkManager.Singleton;
            if (net == null)
            {
                Debug.LogError("ConnectionHandler: no NetworkManager.Singleton in scene!");
                enabled = false;
                return;
            }

            // Make sure you've ticked "Connection Approval" in the inspector
            net.ConnectionApprovalCallback += ApprovalCheck;
        }

        private void OnDestroy()
        {
            if (net != null)
                net.ConnectionApprovalCallback -= ApprovalCheck;
        }

        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            int current = net.ConnectedClientsIds.Count;

            if (current >= maxConnections)
            {
                response.Approved = false;
                response.Reason = "Server is full";
            }
            else
            {
                response.Approved = true;
                response.CreatePlayerObject = true;
            }
        }
    }
}