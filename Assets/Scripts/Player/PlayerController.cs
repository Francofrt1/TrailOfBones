using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerModel))]
[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 movementInput;
    private InputHandler inputHandler;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;

    private Transform cameraPivot;
    private float currentYRotation = 0f;
    private Collider groundCollider;

    private bool isGrounded;
    private bool isJumping;

    private PlayerModel playerModel;
    private PlayerView playerView;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraPivot = GameObject.Find("CameraPivot").transform;
        groundCollider = GameObject.Find("GroundCheck").GetComponent<Collider>();
        
        inputHandler = GetComponent<InputHandler>();
        playerModel = GetComponent<PlayerModel>();
        playerView = GetComponent<PlayerView>();
    }

    private void OnEnable()
    {
        if (inputHandler != null)
        {
            inputHandler.OnMovePerformed += OnMovePerformed;
            inputHandler.OnMoveCanceled += OnMoveCanceled;
            inputHandler.OnJumpPerformed += OnJumpPerformed;
        }
    }

    private void OnDisable()
    {
        if (inputHandler != null)
        {
            inputHandler.OnMovePerformed -= OnMovePerformed;
            inputHandler.OnMoveCanceled -= OnMoveCanceled;
            inputHandler.OnJumpPerformed -= OnJumpPerformed;
        }
    }

    private void OnMovePerformed(Vector2 direction)
    {
        movementInput = direction;
    }

    private void OnMoveCanceled(Vector2 direction)
    {
        movementInput = Vector2.zero;
    }

    private void OnJumpPerformed()
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
        if (!isJumping && !isGrounded) {
            playerView.SetIsFallingAnimation(true);
        }
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
        if (IsGroundLayer(collision.gameObject.layer))
        {
            isGrounded = true;
            isJumping = false;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (IsGroundLayer(collision.gameObject.layer))
        {
            isGrounded = false;
        }
    }

    private bool IsGroundLayer(int layer)
    {
        return (groundLayer.value & (1 << layer)) != 0; // Chequea la layer por bits (mas eficiente que el tag)
    }

    private void ManageRotation()
    {
        currentYRotation = cameraPivot.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }

    public int GetEnemyCount()
    {
        return playerModel.enemies.Count;
    }

    public void AddEnemy(GameObject newEnemy)
    {
        playerModel.enemies.Add(newEnemy);
    }

    public void Attack()
    {
        playerView.SetAttackAnimation(true);
    }
}