using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Data
{
    [CreateAssetMenu(fileName = "DependencyContainerData", menuName = "Scriptable Objects/Dependency Container Data", order = 1)]
    public class DependencyContainerData : ScriptableObject
    {
        [Header("Grid Input Setup")]
        [SerializeField] private InputMode inputMode = InputMode.Keyboard;
        [SerializeField] private InputActionAsset[] actionAssets = new InputActionAsset[2];

        public InputMode InputMode => inputMode;
        public InputActionAsset[] ActionAssets => actionAssets;
    }

    
    public enum InputMode
    {
        Keyboard,
        Mouse
    }
}