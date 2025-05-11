using Assets.Scripts.Interfaces;
using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IDamageable, IAttack, IDeath
{
    private Rigidbody rigidBody;
    private Vector2 movementInput;
    private InputHandler inputHandler;

    [SerializeField] private LayerMask groundLayer;

    private Transform cameraPivot;
    private float currentYRotation = 0f;

    private bool isGrounded;
    private bool isJumping = false;
    private float fallingTime = 0f;

    private PlayerModel playerModel;
    private PlayerView playerView;
    private AttackArea attackArea;

    public event Action playerDie;
    public event Action<float, float> OnPlayerHealthVariation;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        GameObject camera = GameObject.Find("CameraPivot");
        cameraPivot = camera != null ? camera.transform : this.transform;
        
        inputHandler = GetComponent<InputHandler>();
        playerModel = GetComponent<PlayerModel>();
        playerView = GetComponent<PlayerView>();
        attackArea = GetComponentInChildren<AttackArea>();

        AssignEvents();
    }

    private void Start()
    {
        OnPlayerHealthVariation?.Invoke(playerModel.currentHealth, playerModel.maxHealth);
    }

    private void AssignEvents()
    {
        inputHandler.OnMovePerformed += OnMovePerformed;
        inputHandler.OnMoveCanceled += OnMoveCanceled;
        inputHandler.OnJumpPerformed += OnJumpPerformed;
        inputHandler.OnMouseMoveX += ManageRotation;
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
            fallingTime = 0f;
            rigidBody.AddForce(Vector3.up * playerModel.jumpForce, ForceMode.Impulse);
            playerView.SetJumpAnimation();
        }
    }

    private void Update()
    {
        if (!isJumping && !isGrounded) {
            fallingTime += Time.deltaTime;
            playerView.SetIsFallingAnimation(true, fallingTime);
        }
    }

    private void FixedUpdate()
    {
        PerformMovement();
    }

    private void PerformMovement()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);

        Vector3 move = transform.TransformDirection(moveDirection) * playerModel.moveSpeed * playerModel.acceleration;

        move.y = rigidBody.velocity.y;

        rigidBody.velocity = move;

        float horizontalVelocity = Math.Abs(Vector2.Dot(rigidBody.velocity, Vector2.right));
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
        return (groundLayer.value & (1 << layer)) != 0;
    }

    private void ManageRotation(float amount)
    {
        currentYRotation += amount;
        transform.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }

    public void OnAttack()
    {
        if(playerView.IsAttacking()) return;
        playerView.SetAttackAnimation();

        var damageables = attackArea.DamageablesInRange.Where(x => x.GetTag() != "DefendableObject");
        if (!damageables.Any()) return;
        foreach (IDamageable damageable in damageables)
        {
            if (damageable.GetTag() == "DefendableObject") continue; // ignore wheelcart
            damageable.TakeDamage(playerModel.baseDamage, playerModel.ID);
            Debug.Log($"Player did {playerModel.baseDamage} damage to {damageable.GetTag()}");
        }
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
        inputHandler.OnMouseMoveX -= ManageRotation;
        inputHandler.OnAttack -= OnAttack;
        inputHandler.OnSprint -= OnSprint;
    }

    public void TakeDamage(float damageAmout, string hittedById)
    {
        playerView.PlayHitSound();
        playerModel.SetHealth(-damageAmout);

        if (playerModel.currentHealth <= 0)
        {
            playerView.SetIsDeadAnimation();
            playerModel.isDead = true;
            OnDeath(hittedById);
        }

        OnPlayerHealthVariation?.Invoke(playerModel.currentHealth, playerModel.maxHealth);
    }

    public void OnDeath(string killedById)
    {
        playerDie?.Invoke();
    }

    public string GetTag()
    {
        return gameObject.tag;
    }

    public string GetID()
    {
        return playerModel.ID;
    }
}