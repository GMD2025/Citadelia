using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class PlayerConnectionHandler : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => StartCoroutine(StartGame()));
        }

        private IEnumerator StartGame()
        {
            var net = NetworkManager.Singleton;
            net.OnClientConnectedCallback += OnClientConnected;

            if (!net.StartClient())
            {
                Debug.Log("Client failed to start immediately. Becoming host.");
                StartHostAndLoad();
                yield break;
            }

            float timeout = 3f;
            float elapsed = 0f;
            while (!net.IsConnectedClient && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!net.IsConnectedClient)
            {
                Debug.Log("Timed out connecting. Becoming host.");
                net.Shutdown();
                yield return new WaitForSeconds(0.2f);
                StartHostAndLoad();
            }
        }

        private void StartHostAndLoad()
        {
            var net = NetworkManager.Singleton;
            if (!net.StartHost())
            {
                Debug.LogError("Failed to start host");
                return;
            }
            net.SceneManager.LoadScene("Sevastian", LoadSceneMode.Single);
        }

        private void OnClientConnected(ulong clientId)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log($"Client {clientId} connected. Host loading 'Sevastian'.");
                NetworkManager.Singleton.SceneManager.LoadScene("Sevastian", LoadSceneMode.Single);
            }
        }


    }
}