using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private static GameInput instance;
    public static GameInput Instance => instance;

    private PlayerInputActions playerInputActions;

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlterAction;

    private void Awake()
    {
        if (instance == null) instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        //Player interaction input
        playerInputActions.Player.Interact.performed += Interact_Performed;
        playerInputActions.Player.InteractAlter.performed += InteractAlter_Performed;
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    private void Interact_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
    private void InteractAlter_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlterAction?.Invoke(this, EventArgs.Empty);
    }
}
