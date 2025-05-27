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
    public enum InputMode
    {
        Keyboard,
        Pointer,
        Gamepad
    }
    public class LocalDependencyContainer : MonoBehaviour
    {
        [SerializeField] public InputMode SelectedInputMode = InputMode.Keyboard;
        [SerializeField] public ResourceData[] resources;

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
            Register<InputActions>(new InputActions());
            RegisterGridInputSystem();
            RegisterResourceService();
        }

        public void RegisterGridInputSystem()
        {
            switch (SelectedInputMode)
            {
                case InputMode.Keyboard:
                case InputMode.Gamepad:
                    Register<IGridInput>(new GridInputKeyboardGamepad());
                    gameObject.AddComponent<KeyboardGamepadManager>();
                    break;
                case InputMode.Pointer:
                    Register<IGridInput>(new GridInputTouch());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RegisterResourceService()
        {
            Register(new ResourceProductionService(resources));
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