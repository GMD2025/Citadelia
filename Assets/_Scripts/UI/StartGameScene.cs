using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class StartGameScene : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName;
        
        private Button button;
        private TextMeshProUGUI buttonText;

        private void Awake()
        {
            button = GetComponent<Button>();
            buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            button.onClick.AddListener(LoadGameplayScene);
        }

        private void LoadGameplayScene()
        {
            button.interactable = false;
            buttonText.text = "Loading...";
            SceneManager.LoadScene(gameplaySceneName);
        }
    }
}
