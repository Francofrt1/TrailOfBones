using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
    public event Action<Vector2> OnMovePerformed;
    public event Action<Vector2> OnMoveCanceled;
    public event Action OnJumpPerformed;
    public event Action OnPauseTogglePerformed;

    private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction pauseAction;

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        inputActions = playerInput.actions;

        moveAction = inputActions.FindAction("Player/Move");
        jumpAction = inputActions.FindAction("Player/Jump");
        pauseAction = inputActions.FindAction("UI/Pause");
    }

    private void OnEnable()
    {
        moveAction.performed += HandleMove;
        moveAction.canceled += HandleMove;
        jumpAction.performed += HandleJump;
        pauseAction.performed += HandlePause;

        moveAction.Enable();
        jumpAction.Enable();
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.performed -= HandleMove;
        moveAction.canceled -= HandleMove;
        jumpAction.performed -= HandleJump;
        pauseAction.performed -= HandlePause;

        moveAction.Disable();
        jumpAction.Disable();
        pauseAction.Disable();
    }

    private void HandleMove(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();

        if (context.phase == InputActionPhase.Performed)
        {
            OnMovePerformed?.Invoke(inputValue);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            OnMoveCanceled?.Invoke(inputValue);
        }
    }

    private void HandleJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJumpPerformed?.Invoke();
        }
    }

    private void HandlePause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnPauseTogglePerformed?.Invoke();
        }
    }

}