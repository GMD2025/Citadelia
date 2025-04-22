// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.InputSystem;
//
// public enum FocusState { Gameplay, UI }
// public class InputModeManager2 : MonoBehaviour
// {
//     public static InputModeManager2 Instance { get; private set; }
//     public FocusState CurrentFocus { get; private set; } = FocusState.Gameplay;
//
//     public UnityEvent<FocusState> OnFocusChanged;
//
//     private void Awake()
//     {
//         if (Instance != null) Destroy(gameObject);
//         Instance = this;
//     }
//
//     public void SwitchFocus()
//     {
//         CurrentFocus = CurrentFocus == FocusState.Gameplay ? FocusState.UI : FocusState.Gameplay;
//         OnFocusChanged?.Invoke(CurrentFocus);
//     }
// }