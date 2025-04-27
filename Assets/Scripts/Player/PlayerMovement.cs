using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    PlayerInputActions inputActions;
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
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        cameraPivot = GameObject.Find("CameraPivot").transform;
        groundCollider = GameObject.Find("GroundCheck").GetComponent<Collider>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
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
    }

    private void FixedUpdate()
    {
        PerformMovement();
    }

    private void PerformMovement()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);

        // Transforma el movimiento local a global (según la rotación del jugador)
        Vector3 move = transform.TransformDirection(moveDirection) * moveSpeed;

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
        // Chequea la layer por bits (mas eficiente que el tag)
        return (groundLayer.value & (1 << obj.layer)) != 0;
    }

    private void ManageRotation()
    {
        currentYRotation = cameraPivot.rotation.eulerAngles.y;

        transform.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }
}
