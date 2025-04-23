using System.Collections.Generic;
using _Scripts.ResourceSystem;
using _Scripts.TilemapGrid;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Tilemaps;

namespace _Scripts
{
    // use SO to store all the additional setup data like below
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
        private Dictionary<System.Type, object> services;
        private void Register<T, K>(K instance) where K : T
        {
            services[typeof(T)] = instance;
        }
        
        public T Resolve<T>() where T : class => services[typeof(T)] as T;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            RegisterGridInputSystem();
            RegisterResourceService();
        }

        private void RegisterGridInputSystem()
        {
            switch (inputMode)
            {
                case InputMode.Keyboard:
                {
                    Register<IGridInput, GridInputKeyboard>(new GridInputKeyboard());

                    InputSystemUIInputModule inputModule = FindFirstObjectByType<InputSystemUIInputModule>();
                    inputModule.actionsAsset = actionAssets[1];
                    inputModule.move = InputActionReference.Create(actionAssets[1].FindActionMap("UI").FindAction("Horizontal"));
                    inputModule.submit = InputActionReference.Create(actionAssets[1].FindActionMap("Global").FindAction("Confirm"));
                    
                    KeyboardManager keyboardManager = gameObject.AddComponent<KeyboardManager>();
                    break;
                }
                case InputMode.Mouse:
                {
                    Register<IGridInput, GridInputTouch>(new GridInputTouch());
                    
                    InputSystemUIInputModule inputModule = FindFirstObjectByType<InputSystemUIInputModule>();
                    inputModule.actionsAsset = actionAssets[0];
                    InputActionMap inputActionMap = actionAssets[1].FindActionMap("UI");
                    break;
                }
            }
        }

        private void RegisterResourceService()
        {
            Register<ResourceProductionService, ResourceProductionService>(new ResourceProductionService());
        }
    }
}