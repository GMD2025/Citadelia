using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class NetworkBootstrapper : MonoBehaviour
{
    [Header("Network Settings")]
    [SerializeField] private float connectionTimeout = 5f;
    [SerializeField] private string connectionAddress = "127.0.0.1";
    [SerializeField] private ushort connectionPort = 7777;

    private NetworkManager networkManager;
    private bool isConnected = false;

    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager component missing!");
            enabled = false;
        }
    }

    private void Start()
    {
        // Configure transport
        if (networkManager.NetworkConfig.NetworkTransport is UnityTransport utp)
        {
            utp.ConnectionData.Address = connectionAddress;
            utp.ConnectionData.Port    = connectionPort;
        }

        // Subscribe to connection events
        networkManager.OnClientConnectedCallback += HandleClientConnected;
        networkManager.OnClientDisconnectCallback += HandleClientDisconnected;
        networkManager.OnTransportFailure       += HandleTransportFailure;

        // Begin connection attempt
        AttemptClientConnection();
    }

    private void OnDestroy()
    {
        if (networkManager != null)
        {
            networkManager.OnClientConnectedCallback -= HandleClientConnected;
            networkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
            networkManager.OnTransportFailure       -= HandleTransportFailure;
        }
    }

    private void AttemptClientConnection()
    {
        Debug.Log("Attempting to start client...");
        if (!networkManager.StartClient())
        {
            Debug.LogWarning("Client startup failed. Falling back to host.");
            BecomeHost();
            return;
        }
        // Schedule fallback if no connection event
        Invoke(nameof(FallbackToHost), connectionTimeout);
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == networkManager.LocalClientId)
        {
            Debug.Log("Connected as client.");
            isConnected = true;
            CancelInvoke(nameof(FallbackToHost));
        }
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        if (clientId == networkManager.LocalClientId && !isConnected)
        {
            Debug.LogWarning("Client failed to connect. Falling back to host.");
            FallbackToHost();
        }
    }

    private void HandleTransportFailure()
    {
        Debug.LogError("Transport failure detected. Shutting down and retrying as host.");
        FallbackToHost();
    }

    private void FallbackToHost()
    {
        if (isConnected || networkManager.IsListening) return;
        BecomeHost();
    }

    private void BecomeHost()
    {
        // Cleanly shut down any client/server
        networkManager.Shutdown();  
        StartCoroutine(DelayedHostStart());
    }

    private IEnumerator DelayedHostStart()
    {
        yield return new WaitForSeconds(0.5f);
        if (!networkManager.IsListening && networkManager.StartHost())
        {
            Debug.Log("Started as host successfully.");
        }
        else
        {
            Debug.LogError("Host startup failed.");
        }
    }
}
