using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    public static GameInput Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    public event EventHandler OnSkipDialogueAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;

    private void Awake () {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        Instance = this;

        playerInputActions.Player.Pause.performed += Pause_performed;
        playerInputActions.Player.SkipDialogue.performed += SkipDialogue_performed;
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;

        playerInputActions.Player.SkipDialogue.Disable();
        playerInputActions.Player.InteractAlternate.Disable(); // Enable when on minigame state
    }

    private void InteractAlternate_performed (UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed (UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void Pause_performed (UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void SkipDialogue_performed (UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnSkipDialogueAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized () {
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }

    private void OnDestroy () {
        playerInputActions.Player.Pause.performed -= Pause_performed;
        playerInputActions.Player.SkipDialogue.performed -= SkipDialogue_performed;
        playerInputActions.Player.Interact.performed -= Interact_performed;
    }

    public void EnablePlayerMovementAndInteract () {
        playerInputActions.Player.Movement.Enable();
        playerInputActions.Player.Interact.Enable();
    }

    public void DisablePlayerMovementAndInteract () {
        playerInputActions.Player.Movement.Disable();
        playerInputActions.Player.Interact.Disable();
    }

    public void EnableInteractAlternate () {
        playerInputActions.Player.InteractAlternate.Enable(); 
    }

    public void DisableInteractAlternate () {
        playerInputActions.Player.InteractAlternate.Disable();
    }

    public void EnableSkipDialogue () {
        playerInputActions.Player.SkipDialogue.Enable();
    }

    public void DisableSkipDialogue () {
        playerInputActions.Player.SkipDialogue.Disable();
    }
}
