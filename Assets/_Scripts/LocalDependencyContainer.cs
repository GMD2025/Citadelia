using System;
using System.Collections.Generic;
using _Scripts.Data;
using _Scripts.Gameplay.ResourceSystem;
using _Scripts.Gameplay.UserInput;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace _Scripts
{
    public class LocalDependencyContainer : MonoBehaviour
    {
        [SerializeField] public DependencyContainerData Data;
        
        
        private Dictionary<Type, object> services = new();
        private static LocalDependencyContainer cached;

        public void Awake()
        {
            RegisterDependencies();
        }

        private void Register<T, K>(K instance) where K : T
        {
            services[typeof(T)] = instance;
        }

        private void Register<T>(T instance)
        {
            services[typeof(T)] = instance;
        }

        public T Resolve<T>() where T : class
        {
            services.TryGetValue(typeof(T), out var service);
            return service as T;
        }

        private void RegisterDependencies()
        {
            RegisterGridInputSystem();
            RegisterResourceService();
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
                    inputModule.move =
                        InputActionReference.Create(Data.ActionAssets[1].FindActionMap("UI").FindAction("Horizontal"));
                    inputModule.submit =
                        InputActionReference.Create(Data.ActionAssets[1].FindActionMap("Global").FindAction("Confirm"));

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

        public static LocalDependencyContainer Instance
        {
            get
            {
                if (cached) 
                    return cached;

                cached = FindFirstObjectByType<LocalDependencyContainer>();
                if (!cached)
                {
                    Debug.LogError("LocalDependencyContainerAccessor: No LocalDependencyContainer found in scene.");
                }
                return cached;
            }
        }
    }
}