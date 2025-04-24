using System;
using System.Collections.Generic;
using _Scripts.Data;
using _Scripts.Gameplay.ResourceSystem;
using _Scripts.Gameplay.UserInput;
using _Scripts.TilemapGrid;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Tilemaps;

namespace _Scripts
{
    public class DependencyContainer : MonoBehaviour
    {
        [SerializeField] public DependencyContainerData Data;

        public static DependencyContainer Instance;

        private Dictionary<Type, object> services = new();
        private void Register<T, K>(K instance) where K : T
        {
            services[typeof(T)] = instance;
        }
        
        private void Register<T>(T instance)
        {
            services[typeof(T)] = instance;
        }
        
        public T Resolve<T>() where T : class => services[typeof(T)] as T;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                RegisterDependencies();
            }
            else Destroy(gameObject);
        }

        private void RegisterDependencies()
        {
            RegisterGridInputSystem();
            RegisterResourceService();
            RegisterNetworkPrefabRegistry();
        }
        
        private void RegisterGridInputSystem()
        {
            switch (Data.InputMode)
            {
                case InputMode.Keyboard:
                {
                    Register<IGridInput, GridInputKeyboard>(new GridInputKeyboard());

                    InputSystemUIInputModule inputModule = FindFirstObjectByType<InputSystemUIInputModule>();
                    inputModule.actionsAsset = Data.ActionAssets[1];
                    inputModule.move = InputActionReference.Create(Data.ActionAssets[1].FindActionMap("UI").FindAction("Horizontal"));
                    inputModule.submit = InputActionReference.Create(Data.ActionAssets[1].FindActionMap("Global").FindAction("Confirm"));
                    
                    KeyboardManager keyboardManager = gameObject.AddComponent<KeyboardManager>();
                    break;
                }
                case InputMode.Mouse:
                {
                    Register<IGridInput, GridInputTouch>(new GridInputTouch());
                    
                    InputSystemUIInputModule inputModule = FindFirstObjectByType<InputSystemUIInputModule>();
                    inputModule.actionsAsset = Data.ActionAssets[0];
                    InputActionMap inputActionMap = Data.ActionAssets[1].FindActionMap("UI");
                    break;
                }
            }
        }

        private void RegisterResourceService()
        {
            Register(new ResourceProductionService());
        }

        private void RegisterNetworkPrefabRegistry()
        {
            Register(new NetworkPrefabRegistry());
        }
    }
}