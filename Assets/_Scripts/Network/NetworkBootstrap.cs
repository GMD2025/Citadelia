using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Network
{
    public class GameplayNetworkBootstrapper : MonoBehaviour
    {
        [SerializeField] private float clientTimeout = 0.65f;

        private void Start()
        {
            TryStartClient();
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
                Debug.LogError("Can't start neither client or host. Check your network configuration.");
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
}