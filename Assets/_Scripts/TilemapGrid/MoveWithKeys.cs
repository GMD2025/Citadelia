using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.TilemapGrid
{
    public class MoveWithKeys : MonoBehaviour
    {
        private InputAction moveHorizontal;
        [SerializeField]
        private InputActionAsset inputActionAsset;

        void Awake()
        {
            moveHorizontal = inputActionAsset.FindActionMap("Gameplay").FindAction("Vertical");
            Vector2 test = inputActionAsset.FindActionMap("Gameplay").FindAction("Vertical").ReadValue<Vector2>();
            moveHorizontal.performed += OnMove;
            moveHorizontal.canceled += OnMove;
            moveHorizontal.Enable();
        }

        void OnDisable()
        {
            moveHorizontal.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            // Apply value to movement, etc.
            Debug.Log(value.ToString());
        }
    }
}