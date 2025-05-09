using Assets.Scripts.Interfaces;
using System;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerModel))]
[RequireComponent(typeof(PlayerView))]
[RequireComponent(typeof(InputHandler))]
public class PlayerController : MonoBehaviour, IDamageable, IAttack, IDeath, IAttackRangeProvider
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
    private AttackArea attackArea;

    public event Action playerDie;
    public event Action<float> OnPlayerHealthVariation;

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
            rigidBody.AddForce(Vector3.up * playerModel.jumpForce, ForceMode.Impulse);
            playerView.SetJumpAnimation();
        }
    }

    private void Update()
    {
        if (!isJumping && !isGrounded) {
            playerView.SetIsFallingAnimation(true);
        }

        if(playerModel.isDead && !playerView.IsDying())
        {
            OnDeath();
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

    public int GetEnemyCount()
    {
        return playerModel.enemies.Count;
    }

    public void AddEnemy(GameObject newEnemy)
    {
        playerModel.enemies.Add(newEnemy);
    }

    public void OnAttack()
    {
        if(playerView.IsAttacking()) return;
        playerView.SetAttackAnimation();
        foreach (IDamageable damageable in attackArea.DamageablesInRange)
        {
            damageable.TakeDamage(playerModel.baseDamage);
            Debug.Log($"{playerModel.baseDamage} done to {damageable.GetTag()}");
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

    public void TakeDamage(float damageAmout)
    {
        playerModel.SetHealth(-damageAmout);

        if (playerModel.currentHealth <= 0)
        {
            playerView.SetIsDeadAnimation();
            playerModel.isDead = true;
        }

        OnPlayerHealthVariation?.Invoke(playerModel.currentHealth);
    }

    public void OnDeath()
    {
        playerDie?.Invoke();
        Destroy(playerModel);
        Destroy(playerView);
        Destroy(gameObject);

    }

    public string GetTag()
    {
        return gameObject.tag;
    }
    public float RangeToBeAttacked()
    {
        return playerModel.distToBeAttacked;
    }
}