using Assets.Scripts.Interfaces;
using FishNet.Object;
using System;
using System.Linq;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour, IDamageable, IAttack, IDeath, IHealthVariation, IPowerUpApplicable
{
    private Rigidbody rigidBody;
    private Vector2 movementInput;
    private InputHandler inputHandler;

    [SerializeField] private LayerMask groundLayer;

    private float currentYRotation = 0f;

    private bool isGrounded;
    private bool isJumping = false;
    private float fallingTime = 0f;

    private PlayerModel playerModel;
    private PlayerView playerView;
    private AttackArea attackArea;

    public event Action OnDie;
    public event Action<float, float> OnHealthVariation;
    public static Action<PlayerController> OnPlayerSpawned;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        var cameraPivot = GetComponentInChildren<CameraPivot>();
        inputHandler = GetComponent<InputHandler>();
        playerModel = GetComponent<PlayerModel>();
        playerView = GetComponent<PlayerView>();
        attackArea = GetComponentInChildren<AttackArea>();

        cameraPivot.SetInputHandler(inputHandler);
        groundLayer = LayerMask.GetMask("groundLayer");
        AssignEvents();
        OnPlayerSpawned?.Invoke(this);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            this.enabled = false;
        }
    }

    private void Start()
    {
        OnHealthVariation?.Invoke(playerModel.currentHealth, playerModel.maxHealth);
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
        if (!isJumping && !isGrounded)
        {
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

        Vector3 nextPosition = rigidBody.position + move * Time.fixedDeltaTime;
        rigidBody.MovePosition(nextPosition);

        Vector3 flatMove = new Vector3(move.x, 0f, move.z);
        float horizontalVelocity = flatMove.magnitude;
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
        if (playerView.IsAttacking()) return;
        playerView.SetAttackAnimation();

        attackArea.DamageablesInRange.RemoveAll(x => x == null || (x as MonoBehaviour) == null);
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

        OnHealthVariation?.Invoke(playerModel.currentHealth, playerModel.maxHealth);
    }

    public void OnDeath(string killedById)
    {
        OnDie?.Invoke();
    }

    public string GetTag()
    {
        return gameObject.tag;
    }

    public string GetID()
    {
        return playerModel.ID;
    }

    // Propósito: Aplica un aumento temporal al daño de ataque.
    // Precondición: multiplier > 0, duration > 0
    public void ApplyAttackBoost(float multiplier, float duration)
    {
        playerModel.SetAttackSpeed(multiplier, duration);
        StartCoroutine(ResetAttackAfter(duration));
    }

    // Propósito: Restaura una cantidad de salud al jugador.
    // Precondición: amount > 0
    public void ApplyHealing(float amount)
    {
        playerModel.SetHealth(amount);
        OnHealthVariation?.Invoke(playerModel.currentHealth, playerModel.maxHealth);
    }

    // Propósito: Aumenta la velocidad de movimiento del jugador temporalmente.
    // Precondición: bonus > 0, duration > 0
    public void ApplySpeedBoost(float bonus, float duration)
    {
        playerModel.moveSpeed += bonus;
        StartCoroutine(ResetSpeedAfter(duration, bonus));
    }

    // Propósito: Reinicia la velocidad de ataque después del tiempo del PowerUp.
    // Precondición: tiempo > 0
    private IEnumerator ResetAttackAfter(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        playerModel.currentAttackSpeed = playerModel.baseAttackSpeed;
    }

    // Propósito: Reinicia la velocidad de movimiento después del tiempo del PowerUp.
    // Precondición: tiempo > 0, bonus > 0
    private IEnumerator ResetSpeedAfter(float tiempo, float bonus)
    {
        yield return new WaitForSeconds(tiempo);
        playerModel.moveSpeed -= bonus;
    }





}