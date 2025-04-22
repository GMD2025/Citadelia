using _Scripts;
using _Scripts.TilemapGrid;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum FocusState { Gameplay, UI }
public class InputModeManager : MonoBehaviour
{
    [SerializeField] private GameObject firstUIButton;
    public static InputModeManager Instance;

    [SerializeField] private PlayerInput playerInput;
    public FocusState CurrentFocus { get; private set; }
    public IGridInput InputMode { get; private set; }
    
    private KeyboardActions keyboardActions;
    private GameObject currentFocusButton;
    private EventSystem eventSystem;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        eventSystem = EventSystem.current;
        keyboardActions = new KeyboardActions();
        
        // Gameplay Move
        keyboardActions.Gameplay.Move.performed += OnMove;
        keyboardActions.Gameplay.Move.canceled += OnStopMove;
        keyboardActions.Gameplay.Move.Enable();
        
        // Global
        keyboardActions.Global.SwitchFocus.performed += ctx => ToggleMode();
        keyboardActions.Global.Enable();
        
        // UI
        keyboardActions.UI.Confirm.performed += ctx =>
        {
            Debug.Log("Confirm");
        };
        keyboardActions.UI.Confirm.Enable();
        
        CurrentFocus = FocusState.Gameplay;
    }

    private void Start()
    {
        // keyboardActions.Gameplay.Disable();
        // keyboardActions.UI.Disable();
    }
    
    public void ToggleMode()
    {
        Debug.Log("toggle mode");
        if (CurrentFocus == FocusState.Gameplay)
        {
            CurrentFocus = FocusState.UI;
            keyboardActions.UI.Enable();
            EnableUI();
            keyboardActions.Gameplay.Disable();
        }
        else if (CurrentFocus == FocusState.UI)
        {
            CurrentFocus = FocusState.Gameplay;
            keyboardActions.UI.Disable();
            DisableUI();
            keyboardActions.Gameplay.Enable();
        }
    }
    
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        // Apply value to movement, etc.
        IGridInput grid = DependencyContainer.Instance.GridInput;
        if (grid is GridInputKeyboard)
        {
            ((GridInputKeyboard)grid).MoveCurrentPosition(value);
        }
        Debug.Log(value.ToString());
    }

    private void OnStopMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        // Apply value to movement, etc.
        IGridInput grid = DependencyContainer.Instance.GridInput;
        if (grid is GridInputKeyboard)
        {
            ((GridInputKeyboard)grid).MoveCurrentPosition(value);
        }
        Debug.Log(value.ToString());
    }

    
    private void EnableUI()
    {
        eventSystem.sendNavigationEvents = true;
        eventSystem.SetSelectedGameObject(currentFocusButton ?? firstUIButton);
    }
    private void DisableUI()
    {
        currentFocusButton = EventSystem.current.currentSelectedGameObject;
        eventSystem.sendNavigationEvents = false;
        eventSystem.SetSelectedGameObject(null);
    }
}