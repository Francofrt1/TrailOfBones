using Assets.Scripts.Interfaces;
using FishNet.Object;
using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour, IDamageable, IAttack, IDeath, IHealthVariation
{
    private Rigidbody rigidBody;
    private Vector2 movementInput;
    private InputHandler inputHandler;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wheelcartFloorLayer;

    private float currentYRotation = 0f;

    private bool isGrounded;
    private bool isJumping = false;
    private float fallingTime = 0f;

    private PlayerModel playerModel;
    private PlayerView playerView;
    private AttackArea attackArea;

    private Transform carrierTransform = null;
    private Vector3 lastCarrierPosition = Vector3.zero;
    private Vector3 carrierDelta = Vector3.zero;

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
        wheelcartFloorLayer = LayerMask.GetMask("WheelcartFloorLayer");
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
        UpdateCarrierDelta();
        PerformMovement();
    }

    private void PerformMovement()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);

        Vector3 move = transform.TransformDirection(moveDirection) * playerModel.moveSpeed * playerModel.acceleration;

        move.y = rigidBody.velocity.y;

        Vector3 playerMovement = move * Time.fixedDeltaTime;
        Vector3 platformMovement = carrierDelta;
        Vector3 combinedMovement = playerMovement + platformMovement;
        Vector3 nextPosition = rigidBody.position + combinedMovement;
        rigidBody.MovePosition(nextPosition);

        Vector3 flatMove = new Vector3(move.x, 0f, move.z);
        float horizontalVelocity = flatMove.magnitude;
        playerView.SetMovementAnimation(horizontalVelocity);
    }

    private void UpdateCarrierDelta()
    {
        if (carrierTransform != null)
        {
            carrierDelta = carrierTransform.position - lastCarrierPosition;
            lastCarrierPosition = carrierTransform.position;
        }
        else
        {
            carrierDelta = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int layer = collision.gameObject.layer;

        bool isGround = IsGroundLayer(layer);
        bool isCarrier = IsWheelcartFloorLayer(layer);

        if (isGround || isCarrier)
        {
            isGrounded = true;
            playerView.SetIsFallingAnimation(false);
        }

        if (isCarrier)
        {
            carrierTransform = collision.transform;
            lastCarrierPosition = carrierTransform.position;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        int layer = collision.gameObject.layer;
        
        bool isGround = IsGroundLayer(layer);
        bool isCarrier = IsWheelcartFloorLayer(layer);

        if (isGround || isCarrier)
        {
            isGrounded = false;
        }

        if (isCarrier && collision.transform == carrierTransform)
        {
            carrierTransform = null;
            carrierDelta = Vector3.zero;
        }
    }

    private bool IsGroundLayer(int layer)
    {
        return (groundLayer.value & (1 << layer)) != 0;
    }

    private bool IsWheelcartFloorLayer(int layer)
    {
        return (wheelcartFloorLayer.value & (1 << layer)) != 0;
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

    public void Heal(float healAmount)
    {
        playerModel.SetHealth(healAmount);
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
}