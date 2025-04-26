using System.Collections;
using TMPro;
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
        private TextMeshProUGUI buttonText;
        private const string GameSceneName = "Sevastian";

        private void Awake()
        {
            button = GetComponent<Button>();
            buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            button.onClick.AddListener(HandleConnection);
        }

        private void HandleConnection()
        {
            button.interactable = false;
            buttonText.text = "Loading...";

            var net = NetworkManager.Singleton;

            Debug.Log("Trying to start client...");
            if (net.StartClient())
            {
                StartCoroutine(CheckConnectionTimeout());
            }
            else
            {
                Debug.LogWarning("Client failed immediately. Starting Host.");
                StartHost();
            }
        }

        private IEnumerator CheckConnectionTimeout()
        {
            var net = NetworkManager.Singleton;
            float timeout = 0.75f;
            float elapsed = 0f;

            while (!net.IsConnectedClient && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (net.IsConnectedClient)
            {
                Debug.Log("Connected to Host successfully.");
                net.OnClientConnectedCallback += OnClientConnected;
                button.interactable = false;
                buttonText.text = "Connected";
            }
            else
            {
                Debug.LogWarning("No Host found quickly. Becoming Host.");
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
                Debug.Log("Started Host.");
                LoadGameScene();
            }
            else
            {
                Debug.LogError("Failed to start Host.");
                button.interactable = true;
                buttonText.text = "Retry";
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            if (NetworkManager.Singleton.IsHost && clientId == NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log("Host loading the game scene.");
                LoadGameScene();
            }
            else
            {
                Debug.Log($"Client {clientId} connected. Waiting for scene sync.");
            }
        }

        private void LoadGameScene()
        {
            NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        }
    }
}
