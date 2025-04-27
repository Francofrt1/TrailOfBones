using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    
    Vector2 movementInput;
    Rigidbody rb;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;

    private Transform cameraPivot;
    private float currentYRotation = 0f;
    private Collider groundCollider;

    private bool isGrounded;
    private bool isJumping;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraPivot = GameObject.Find("CameraPivot").transform;
        groundCollider = GameObject.Find("GroundCheck").GetComponent<Collider>();
        
        var playerInput = GetComponent<PlayerInput>();
        inputActions = playerInput.actions;
        
        moveAction = inputActions.FindAction("Player/Move");
        jumpAction = inputActions.FindAction("Player/Jump");
    }

    private void OnEnable()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        jumpAction.performed += OnJump;
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        jumpAction.performed -= OnJump;
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }
    }

    private void Update()
    {
        ManageRotation();
        if (!isJumping && !isGrounded) {} //animación de caída
    }

    private void FixedUpdate()
    {
        PerformMovement();
    }

    private void PerformMovement()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);

        Vector3 move = transform.TransformDirection(moveDirection) * moveSpeed; // Transforma el movimiento local a global (según la rotación del jugador)

        move.y = rb.velocity.y;

        rb.velocity = move;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (IsGroundLayer(collision.gameObject))
        {
            isGrounded = true;
            isJumping = false;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (IsGroundLayer(collision.gameObject))
        {
            isGrounded = false;
        }
    }

    private bool IsGroundLayer(GameObject obj)
    {
        return (groundLayer.value & (1 << obj.layer)) != 0; // Chequea la layer por bits (mas eficiente que el tag)
    }

    private void ManageRotation()
    {
        currentYRotation = cameraPivot.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }
}