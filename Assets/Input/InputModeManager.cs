using UnityEngine;
using UnityEngine.InputSystem;

public enum InputMode { Gameplay, UI }
public class InputModeManager : MonoBehaviour
{
    public static InputModeManager Instance;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionAsset inputActions;

    public InputMode CurrentMode { get; private set; }
    
    private InputActionMap gameplayMap;
    private InputActionMap uiMap;
    private InputActionMap globalMap;
    
    private void Awake()
    {
        gameplayMap = inputActions.FindActionMap("Gameplay");
        uiMap = inputActions.FindActionMap("UI");
        globalMap = inputActions.FindActionMap("Global");
        
        globalMap.Enable();
        
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playerInput.actions["Switch"].performed += ctx => ToggleMode();
    }
    
    public void ToggleMode()
    {
        if (CurrentMode == InputMode.Gameplay)
        {
            uiMap.Disable();
            gameplayMap.Enable();
        }
        else if (CurrentMode == InputMode.UI)
        {
            uiMap.Enable();
            gameplayMap.Disable();
        }
    }
    
    public void SwitchMode(InputMode mode)
    {
        CurrentMode = mode;
        switch (mode)
        {
            case InputMode.Gameplay:
                playerInput.SwitchCurrentActionMap("Gameplay");
                break;
            case InputMode.UI:
                playerInput.SwitchCurrentActionMap("UI");
                break;
        }
    }
}