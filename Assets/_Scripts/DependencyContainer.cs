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
    /// <summary>
    /// Any non MonoBehaviour, Singleton class can be registered in DependencyContainer. 
    /// When doing so, it is safe to remove all the Singleton features.
    /// </summary>
    public class DependencyContainer : NetworkBehaviour
    {
        [SerializeField] public DependencyContainerData Data;

        private Dictionary<Type, object> services = new();

        private bool dependenciesInitialized = false;

        public override void OnNetworkSpawn()
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

        public void RegisterDependencies()
        {
            if (!IsOwner || dependenciesInitialized) return;
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

        public static DependencyContainer LocalInstance
        {
            get
            {
                if (!NetworkManager.Singleton || 
                    NetworkManager.Singleton.LocalClient == null || 
                    !NetworkManager.Singleton.LocalClient.PlayerObject)
                {
                    Debug.LogWarning("DependencyContainer: Client is not connected to the server. Dependency retrieval failed. Functionality will break. Try to reconnect.");
                    return null;
                }

                return NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<DependencyContainer>();
            }
        }

        
        public static DependencyContainer Instance(ulong clientId)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
            {
                return networkClient.PlayerObject.GetComponent<DependencyContainer>();
            }

            Debug.LogError($"DependencyContainer: No player found for clientId {clientId}");
            return null;
        }
    }
}