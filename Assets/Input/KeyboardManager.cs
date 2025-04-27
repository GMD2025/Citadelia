using System;
using _Scripts;
using _Scripts.Gameplay.UserInput;
using _Scripts.TilemapGrid;
using _Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum FocusState { Gameplay, UI }
public class KeyboardManager : MonoBehaviour
{
    [SerializeField] private GameObject firstUIButton;
    public static KeyboardManager Instance;

    public FocusState CurrentFocus { get; private set; }
    
    private KeyboardActions keyboardActions;
    private GameObject currentFocusButton;
    private EventSystem eventSystem;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        eventSystem = EventSystem.current;
        keyboardActions = new KeyboardActions();
        firstUIButton = GameObject.Find("ArrowLeft");
        
        // Gameplay Move
        keyboardActions.Gameplay.Move.Enable();
        
        // Global
        keyboardActions.Global.SwitchFocus.performed += ctx => ToggleMode();
        keyboardActions.Global.Confirm.performed += ctx => Confirm();
        keyboardActions.Global.Cancel.performed += ctx => Cancel();
        keyboardActions.Global.Enable();
        
        // UI
        keyboardActions.UI.Disable();

        
        CurrentFocus = FocusState.Gameplay;
    }

    private void Confirm()
    {
        switch (CurrentFocus)
        {
            case FocusState.UI:
            {
                ToggleMode();
                currentFocusButton.GetComponent<BuildingPlacer>().PlaceBuildingPlaceholder();
                break;
            }
            case FocusState.Gameplay:
            {
                ToggleMode();
                currentFocusButton.GetComponent<BuildingPlacer>().PlaceBuilding(); 
                break;
            }
        }
    }
    
    private void Cancel()
    {
        switch (CurrentFocus)
        {
            case FocusState.Gameplay:
            {
                ToggleMode();
                break;
            }
        }
    }

    private float moveCooldown = 0.1f;
    private float moveTimer = 0f;

    private void Update()
    {
        moveTimer -= Time.deltaTime;
        Vector2 moveInput = keyboardActions.Gameplay.Move.ReadValue<Vector2>();
        
        if (moveInput != Vector2.zero && moveTimer <= 0f)
        {
            IGridInput grid = DependencyContainer.LocalInstance.Resolve<IGridInput>();
            if (grid is GridInputKeyboard)
            {
                ((GridInputKeyboard)grid).MoveCurrentPosition(moveInput);
                moveTimer = moveCooldown;
            }
        }
    }
    
    public void ToggleMode()
    {
        Debug.Log("toggle mode");
        if (CurrentFocus == FocusState.Gameplay)
        {
            if (currentFocusButton && currentFocusButton.GetComponent<BuildingPlacer>() is not null)
            {
                currentFocusButton.GetComponent<BuildingPlacer>().Cancel();
            }
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

    private void EnableUI()
    {
        eventSystem.sendNavigationEvents = true;
        eventSystem.SetSelectedGameObject(currentFocusButton ?? firstUIButton);
    }
    private void DisableUI()
    {
        currentFocusButton = eventSystem.currentSelectedGameObject;
        eventSystem.sendNavigationEvents = false;
        eventSystem.SetSelectedGameObject(null);
    }
}