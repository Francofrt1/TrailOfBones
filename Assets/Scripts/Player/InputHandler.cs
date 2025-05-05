using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
    public event Action<Vector2> OnMovePerformed;
    public event Action OnMoveCanceled;
    public event Action OnJumpPerformed;
    public event Action OnPauseTogglePerformed;
    public event Action OnAttack;
    public event Action OnSprint;

    private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction pauseAction;
    private InputAction attackAction;
    private InputAction sprintAction;

    private void Awake()
    {
        AssignActions();
        AssignEvents();
    }

    private void AssignActions()
    {
        var playerInput = GetComponent<PlayerInput>();
        inputActions = playerInput.actions;

        moveAction = inputActions.FindAction("Player/Move");
        jumpAction = inputActions.FindAction("Player/Jump");
        attackAction = inputActions.FindAction("Player/Attack");
        sprintAction = inputActions.FindAction("Player/Sprint");
        pauseAction = inputActions.FindAction("UI/Pause");
    }

    private void AssignEvents()
    {
        moveAction.performed += HandleMove;
        moveAction.canceled += HandleMove;
        jumpAction.performed += HandleJump;
        pauseAction.performed += HandlePause;
        attackAction.performed += HandleAttack;
        sprintAction.performed += HandleSprint;
    }

    private void OnEnable()
    {
        OnPlayerInputEnabled();
        OnUIInputEnabled();
    }

    private void OnDisable()
    {
        OnPlayerInputDisabled();
        OnUIInputDisabled();
    }

    private void OnDestroy()
    {
        moveAction.performed -= HandleMove;
        moveAction.canceled -= HandleMove;
        jumpAction.performed -= HandleJump;
        pauseAction.performed -= HandlePause;
        attackAction.performed -= HandleAttack;
        sprintAction.performed -= HandleSprint;
        moveAction.Dispose();
        jumpAction.Dispose();
        pauseAction.Dispose();
        attackAction.Dispose();
        sprintAction.Dispose();
    }

    public void OnPlayerInputEnabled()
    {
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
        sprintAction.Enable();
    }

    public void OnPlayerInputDisabled()
    {
        moveAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
        sprintAction.Disable();
    }

    public void OnUIInputEnabled()
    {
        pauseAction.Enable();
    }

    public void OnUIInputDisabled()
    {
        pauseAction.Enable();
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
            OnMoveCanceled?.Invoke();
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

    private void HandleAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnAttack?.Invoke();
        }
    }

    private void HandleSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSprint?.Invoke();
        }
    }
}