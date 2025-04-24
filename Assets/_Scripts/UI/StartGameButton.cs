using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class StartGameButton: MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => SceneManager.LoadScene("Sevastian"));
        }
    }
}