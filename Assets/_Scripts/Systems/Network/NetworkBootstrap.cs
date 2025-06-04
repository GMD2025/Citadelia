using System.Collections;
using System.IO;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace _Scripts.Systems.Network
{
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
            LoadIPConfig();
            TryStartClient();
        }

        private void HandleClientDisconnect(ulong clientId)
        {
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

        private void LoadIPConfig()
        {
            string exeDir = Directory.GetParent(Application.dataPath)!.FullName;
            string configPath = Path.Combine(exeDir, "config.txt");

            if (!File.Exists(configPath)) return;

            string[] lines = File.ReadAllLines(configPath);
            if (lines.Length < 2) return;

            string serverIP = lines[0].Trim();
            if (!int.TryParse(lines[1].Trim(), out int port)) return;

            Debug.Log($"Overriding connection config: IP={serverIP}, Port={port}");

            var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (utp != null)
            {
                utp.ConnectionData.Address = serverIP;
                utp.ConnectionData.Port = (ushort)port;
            }
            else
            {
                Debug.LogError("UnityTransport component missing on NetworkManager.");
            }
        }

    }
}
