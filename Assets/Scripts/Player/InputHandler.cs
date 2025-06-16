using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.5f;

    public event Action<Vector2> OnMovePerformed;
    public event Action<float> OnMouseMoveX;
    public event Action<float> OnMouseMoveY;
    public event Action OnMoveCanceled;
    public event Action OnJumpPerformed;
    public event Action OnPauseTogglePerformed;
    public event Action OnAttack;
    public event Action OnSprint;
    public event Action OnUseInventory;

    private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction lookAction;
    private InputAction pauseAction;
    private InputAction attackAction;
    private InputAction sprintAction;
    private InputAction useInventoryAction;

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
        lookAction = inputActions.FindAction("Player/Look");
        attackAction = inputActions.FindAction("Player/Attack");
        sprintAction = inputActions.FindAction("Player/Sprint");
        useInventoryAction = inputActions.FindAction("Player/UseInventory");
        pauseAction = inputActions.FindAction("UI/Pause");
    }

    private void AssignEvents()
    {
        moveAction.performed += HandleMove;
        moveAction.canceled += HandleMove;
        jumpAction.performed += HandleJump;
        lookAction.performed += HandleLook;
        pauseAction.performed += HandlePause;
        attackAction.performed += HandleAttack;
        sprintAction.performed += HandleSprint;
        useInventoryAction.performed += HandleUseInventory;
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
        lookAction.performed -= HandleLook;
        pauseAction.performed -= HandlePause;
        attackAction.performed -= HandleAttack;
        sprintAction.performed -= HandleSprint;
        useInventoryAction.performed -= HandleUseInventory;
        moveAction.Dispose();
        jumpAction.Dispose();
        lookAction.Dispose();
        pauseAction.Dispose();
        attackAction.Dispose();
        sprintAction.Dispose();
        useInventoryAction.Dispose();
    }

    public void OnPlayerInputEnabled()
    {
        moveAction.Enable();
        jumpAction.Enable();
        lookAction.Enable();
        attackAction.Enable();
        sprintAction.Enable();
        useInventoryAction.Enable();
    }

    public void OnPlayerInputDisabled()
    {
        moveAction.Disable();
        jumpAction.Disable();
        lookAction.Disable();
        attackAction.Disable();
        sprintAction.Disable();
        useInventoryAction.Disable();
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

    private void HandleLook(InputAction.CallbackContext context)
    {
        Vector2 mouseDelta = context.ReadValue<Vector2>();
        float mouseInputX = mouseDelta.x * mouseSensitivity;
        float mouseInputY = mouseDelta.y * mouseSensitivity;

        if (Mathf.Abs(mouseInputX) > Mathf.Epsilon)
        {
            OnMouseMoveX?.Invoke(mouseInputX);
        }

        if (Mathf.Abs(mouseInputY) > Mathf.Epsilon)
        {
            OnMouseMoveY?.Invoke(mouseInputY);
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
    private void HandleUseInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnUseInventory?.Invoke();
        }
    }
}