using _Scripts.TilemapGrid;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Tilemaps;

namespace _Scripts
{
    public enum InputMode
    {
        Keyboard,
        Mouse
    }
    public class DependencyContainer : MonoBehaviour
    {
        [SerializeField] public InputMode inputMode = InputMode.Keyboard;

        [SerializeField] private InputActionAsset[] actionAssets = new InputActionAsset[2];
        
        public static DependencyContainer Instance;
        
        public IGridInput GridInput { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            switch (inputMode)
            {
                case InputMode.Keyboard:
                {
                    GridInput = new GridInputKeyboard();
                    InputSystemUIInputModule inputModule = FindFirstObjectByType<InputSystemUIInputModule>();
                    inputModule.actionsAsset = actionAssets[1];
                    inputModule.move = InputActionReference.Create(actionAssets[1].FindActionMap("UI").FindAction("Horizontal"));
                    inputModule.submit = InputActionReference.Create(actionAssets[1].FindActionMap("Global").FindAction("Confirm"));
                    
                    KeyboardManager keyboardManager = gameObject.AddComponent<KeyboardManager>();
                    break;
                }
                case InputMode.Mouse:
                {
                    GridInput = new GridInputTouch();
                    InputSystemUIInputModule inputModule = FindFirstObjectByType<InputSystemUIInputModule>();
                    inputModule.actionsAsset = actionAssets[0];
                    InputActionMap inputActionMap = actionAssets[1].FindActionMap("UI");
                    break;
                }
            }
        }
    }
}