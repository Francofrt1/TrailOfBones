using Assets.Scripts.Interfaces;
using FishNet.Object;
using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerPresenter : NetworkBehaviour, IDamageable, IAttack, IDeath, IHealthVariation
{
    private Rigidbody rigidBody;
    private Vector2 movementInput;
    private InputHandler inputHandler;
    private InventoryController inventoryController;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wheelcartFloorLayer;
    [SerializeField] private LayerMask wheelcartBodyLayer;

    private float currentYRotation = 0f;

    private Vector3 _cartPushbackNormal;

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
    public static Action<PlayerPresenter> OnPlayerSpawned;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        var cameraPivot = GetComponentInChildren<CameraPivot>();
        inputHandler = GetComponent<InputHandler>();
        playerModel = GetComponent<PlayerModel>();
        playerView = GetComponent<PlayerView>();
        attackArea = GetComponentInChildren<AttackArea>();
        inventoryController = GetComponentInChildren<InventoryController>();
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
        RepairWheelcart();
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
        Vector3 localVelocity = playerModel.CalculateLocalVelocity(movementInput);

        Vector3 horizontalVelocity = new Vector3(localVelocity.x, 0f, localVelocity.z);
        float verticalVelocity = rigidBody.velocity.y;

        horizontalVelocity = ApplyPushbackPlaneIfNeeded(horizontalVelocity);

        localVelocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);

        playerView.SetMovementAnimation(localVelocity);

        Vector3 nextPos = NextGlobalPosition(localVelocity * Time.fixedDeltaTime);
        rigidBody.MovePosition(nextPos);
    }

    private Vector3 ApplyPushbackPlaneIfNeeded(Vector3 horizontalVelocity)
    {
        if (_cartPushbackNormal == Vector3.zero)
            return horizontalVelocity;

        Vector3 horizontalNormal = new Vector3(_cartPushbackNormal.x, 0f, _cartPushbackNormal.z);

        // Verify if horizontalNormal is not too small
        if (horizontalNormal.sqrMagnitude < 0.0001f)
            return horizontalVelocity;


        horizontalNormal.Normalize(); // Now is safe to .Normalize()

        float dot = Vector3.Dot(horizontalVelocity, -horizontalNormal);
        if (dot > 0f)
        {
            horizontalVelocity = Vector3.ProjectOnPlane(horizontalVelocity, horizontalNormal);
        }

        return horizontalVelocity;
    }

    private Vector3 NextGlobalPosition(Vector3 localDisplacement)
    {
        Vector3 worldMove = transform.TransformDirection(localDisplacement);
        Vector3 combined = worldMove + carrierDelta;
        return rigidBody.position + combined;
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
        bool isWheelcartBody = IsWheelcartBodyLayer(layer);

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

        if (isWheelcartBody)
        {
            _cartPushbackNormal = CalculateCartPushbackNormal(collision);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        int layer = collision.gameObject.layer;

        bool isWheelcartBody = IsWheelcartBodyLayer(layer);

        if (isWheelcartBody)
        {
            _cartPushbackNormal = CalculateCartPushbackNormal(collision);
            print(_cartPushbackNormal);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        int layer = collision.gameObject.layer;

        bool isGround = IsGroundLayer(layer);
        bool isCarrier = IsWheelcartFloorLayer(layer);
        bool isWheelcartBody = IsWheelcartBodyLayer(layer);

        if (isGround || isCarrier)
        {
            isGrounded = false;
        }

        if (isCarrier && collision.transform == carrierTransform)
        {
            carrierTransform = null;
            carrierDelta = Vector3.zero;
        }

        if (isWheelcartBody)
        {
            _cartPushbackNormal = Vector3.zero;
        }
    }

    private Vector3 CalculateCartPushbackNormal(Collision collision)
    {
        Vector3 totalNormal = Vector3.zero;

        foreach (var contact in collision.contacts)
        {
            // Convertimos la normal a espacio local
            Vector3 localNormal = transform.InverseTransformDirection(contact.normal);
            totalNormal += localNormal;
        }

       return totalNormal.normalized;
    }

    private bool IsGroundLayer(int layer)
    {
        return (groundLayer.value & (1 << layer)) != 0;
    }

    private bool IsWheelcartFloorLayer(int layer)
    {
        return (wheelcartFloorLayer.value & (1 << layer)) != 0;
    }

    private bool IsWheelcartBodyLayer(int layer)
    {
        return (wheelcartBodyLayer.value & (1 << layer)) != 0;
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

    public void saveItem(Item item)
    {
        inventoryController.HandleAddItem(item);
    }

    public bool canBeSaved(Item item)
    {
        return inventoryController.canBeSaved(item);
    }

    public void RepairWheelcart()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            /*TO DO: 
             * cambiar para hacer que no dependa de la referencia a la carreta (enviar evento de reparacion?). 
             * Usar lo de santi con una variable boolena para detectar si esta cerca. 
             * Cambiar el input de lugar y enviarlo donde corresponde*/
            GameObject wheelcart = GameObject.FindGameObjectWithTag("DefendableObject");

            if (wheelcart == null) return;
            if (Vector3.Distance(this.transform.position, wheelcart.transform.position) < 10 && wheelcart.GetComponent<WheelcartController>().NeedRepair())
            {
                int logsToSent = wheelcart.GetComponent<WheelcartController>().NeededLogsToRepair();
                int logsInInventory = inventoryController.GetItemQuantity(ItemType.WoodLog);
                if (logsToSent >= logsInInventory)
                {
                    inventoryController.HandleUseItem(ItemType.WoodLog, logsInInventory);
                    wheelcart.GetComponent<WheelcartController>().StorageLog(logsInInventory);
                }
                else
                {
                    inventoryController.HandleUseItem(ItemType.WoodLog, logsToSent);
                    wheelcart.GetComponent<WheelcartController>().StorageLog(logsToSent);
                }

            }
        }
    }
}