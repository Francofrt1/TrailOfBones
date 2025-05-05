using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerModel))]
[RequireComponent(typeof(PlayerView))]
[RequireComponent(typeof(InputHandler))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidBody;
    private Vector2 movementInput;
    private InputHandler inputHandler;

    [SerializeField] private LayerMask groundLayer;

    private Transform cameraPivot;
    private float currentYRotation = 0f;

    private bool isGrounded;
    private bool isJumping = false;

    private PlayerModel playerModel;
    private PlayerView playerView;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        GameObject camera = GameObject.Find("CameraPivot");
        cameraPivot = camera != null ? camera.transform : this.transform;
        
        inputHandler = GetComponent<InputHandler>();
        playerModel = GetComponent<PlayerModel>();
        playerView = GetComponent<PlayerView>();

        AssignEvents();
    }

    private void AssignEvents()
    {
        inputHandler.OnMovePerformed += OnMovePerformed;
        inputHandler.OnMoveCanceled += OnMoveCanceled;
        inputHandler.OnJumpPerformed += OnJumpPerformed;
        inputHandler.OnAttack += OnAttack;
        inputHandler.OnSprint += OnSprint;
    }

    private void OnMovePerformed(Vector2 direction)
    {
        movementInput = direction;
    }

    private void OnMoveCanceled()
    {
        movementInput = Vector2.zero;
    }

    private void OnJumpPerformed()
    {
        if (isGrounded)
        {
            rigidBody.AddForce(Vector3.up * playerModel.jumpForce, ForceMode.Impulse);
            playerView.SetJumpAnimation();
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

        Vector3 move = transform.TransformDirection(moveDirection) * playerModel.moveSpeed * playerModel.acceleration; // Transforma el movimiento local a global (según la rotación del jugador)

        move.y = rigidBody.velocity.y;

        rigidBody.velocity = move;

        float horizontalVelocity = Vector2.Dot(rigidBody.velocity, Vector2.right);
        playerView.SetMovementAnimation(horizontalVelocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsGroundLayer(collision.gameObject.layer))
        {
            isGrounded = true;
            playerView.SetIsFallingAnimation(false);
        }
    }

    private void OnCollisionExit(Collision collision)
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

    private void OnAttack()
    {
        if(playerView.IsAttacking()) return;
        playerView.SetAttackAnimation();
    }

    private void OnSprint()
    {
        playerModel.acceleration = playerModel.acceleration == 1f ? 2f : 1f;
    }

    private void OnDestroy()
    {
        inputHandler.OnMovePerformed -= OnMovePerformed;
        inputHandler.OnMoveCanceled -= OnMoveCanceled;
        inputHandler.OnJumpPerformed -= OnJumpPerformed;
        inputHandler.OnAttack -= OnAttack;
        inputHandler.OnSprint -= OnSprint;
    }
}