using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameplayNetworkBootstrapper : MonoBehaviour
{
    [SerializeField] private float clientTimeout = 0.65f;

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    private void Start()
    {
        TryStartClient();
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        // only react to our own client losing connection
        if (NetworkManager.Singleton.ConnectedClients.Count == 0)
        {
            Debug.LogWarning("Client disconnected—retrying connection...");
            TryStartClient();
        }
    }

    private void TryStartClient()
    {
        var net = NetworkManager.Singleton;

        Debug.Log("Attempting to start client...");
        bool started = net.StartClient();

        if (started)
        {
            StartCoroutine(WaitForConnectionOrBecomeHost());
        }
        else
        {
            Debug.LogError("Can't start client. Check your network configuration.");
        }
    }

    private IEnumerator WaitForConnectionOrBecomeHost()
    {
        var net = NetworkManager.Singleton;
        float elapsed = 0f;

        while (!net.IsConnectedClient && elapsed < clientTimeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (net.IsConnectedClient)
        {
            Debug.Log("Connected to Host successfully.");
        }
        else
        {
            Debug.LogWarning("Connection timeout. Becoming Host.");
            net.Shutdown();
            yield return new WaitForSeconds(0.2f);
            StartHost();
        }
    }

    private void StartHost()
    {
        var net = NetworkManager.Singleton;

        if (net.StartHost())
        {
            Debug.Log("Host started successfully.");
        }
        else
        {
            Debug.LogError("Failed to start Host. Critical failure.");
        }
    }
}
