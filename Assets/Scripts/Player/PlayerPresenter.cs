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

    private Vector3 cartPushbackNormal;

    private PlayerModel playerModel;
    private PlayerView playerView;
    private AttackArea attackArea;
    [SerializeField] private GameObject cameraPivotGameObject;
    [SerializeField] private GameObject attackAreaGameObject;
    [SerializeField] private GameObject inventoryGameObject;

    private Transform carrierTransform = null;
    private Vector3 lastCarrierPosition = Vector3.zero;
    private Vector3 carrierDelta = Vector3.zero;

    public event Action OnDie;
    public event Action<float, float> OnHealthVariation;
    public static Action<PlayerPresenter> OnPlayerSpawned;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        var cameraPivot = cameraPivotGameObject.GetComponent<CameraPivot>();
        inputHandler = GetComponent<InputHandler>();
        playerModel = GetComponent<PlayerModel>();
        playerView = GetComponent<PlayerView>();
        attackArea = attackAreaGameObject.GetComponent<AttackArea>();
        inventoryController = inventoryGameObject.GetComponent<InventoryController>();
        if (cameraPivot != null && inputHandler != null)
        {
            cameraPivot.SetInputHandler(inputHandler);
        }
        if (cameraPivot == null)
        {
            Debug.LogError("CameraPivot component is missing on the player.");
        }
        if (inputHandler == null)
        {
            Debug.LogError("InputHandler component is missing on the player.");
        }
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
        inputHandler.OnUseInventory += UseItems;
        playerView.OnAttackStateChanged += OnAttackStateChanged;
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
        if (playerModel.IsGrounded)
        {
            playerModel.ResetFallingTime();
            rigidBody.AddForce(Vector3.up * playerModel.jumpForce, ForceMode.Impulse);
            playerView.SetJumpAnimation();
        }
    }

    private void Update()
    {

        if (!playerModel.IsGrounded)
        {
            playerModel.UpdateFallingTime(Time.deltaTime);
            playerView.SetIsFallingAnimation(true, playerModel.GetFallingTime());
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
        if (cartPushbackNormal == Vector3.zero)
            return horizontalVelocity;

        Vector3 horizontalNormal = new Vector3(cartPushbackNormal.x, 0f, cartPushbackNormal.z);

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
        // Converts local input to world direction: W moves in player's forward, not global Z+
        Vector3 worldMove = transform.TransformDirection(localDisplacement);
        // Adds carrier location variation
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
            playerModel.IsGrounded = true;
            playerView.SetIsFallingAnimation(false);
        }

        if (isCarrier)
        {
            carrierTransform = collision.transform;
            lastCarrierPosition = carrierTransform.position;
        }

        if (isWheelcartBody)
        {
            cartPushbackNormal = CalculateCartPushbackNormal(collision);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        int layer = collision.gameObject.layer;

        bool isWheelcartBody = IsWheelcartBodyLayer(layer);

        if (isWheelcartBody)
        {
            cartPushbackNormal = CalculateCartPushbackNormal(collision);
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
            playerModel.IsGrounded = false;
        }

        if (isCarrier && collision.transform == carrierTransform)
        {
            carrierTransform = null;
            carrierDelta = Vector3.zero;
        }

        if (isWheelcartBody)
        {
            cartPushbackNormal = Vector3.zero;
        }
    }

    private Vector3 CalculateCartPushbackNormal(Collision collision)
    {
        Vector3 totalNormal = Vector3.zero;

        foreach (var contact in collision.contacts)
        {
            // Convert normal to local space
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
        Quaternion newRotation = playerModel.UpdateYawRotation(amount);

        transform.rotation = newRotation;
    }

    private void OnAttackStateChanged(bool isAttacking)
    {
        playerModel.SetAttackState(isAttacking);
    }
    
    public void OnAttack()
    {
        playerView.CheckIsAttacking();
        if (playerModel.CanAttack() == false) return;
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
        playerModel.ToggleSprint();
    }

    private void OnDestroy()
    {
        inputHandler.OnMovePerformed -= OnMovePerformed;
        inputHandler.OnMoveCanceled -= OnMoveCanceled;
        inputHandler.OnJumpPerformed -= OnJumpPerformed;
        inputHandler.OnMouseMoveX -= ManageRotation;
        inputHandler.OnAttack -= OnAttack;
        inputHandler.OnSprint -= OnSprint;
        playerView.OnAttackStateChanged -= OnAttackStateChanged;
    }

    public void TakeDamage(float damageAmout, string hittedById)
    {
        playerView.PlayHitSound();
        playerModel.SetHealth(-damageAmout);

        if (playerModel.isDead)
        {
            playerView.SetIsDeadAnimation();
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

    public void SaveItem(Item item)
    {
        inventoryController.HandleAddItem(item);
    }

    public bool CanBeSaved(Item item)
    {
        return inventoryController.canBeSaved(item);
    }

    public void UseItems()
    {
        /*TO DO: 
             * cambiar para hacer que no dependa de la referencia a la carreta (enviar evento de reparacion?).
             * Hacer una interfaz interactable y hacer que funcione a partir de ahi sin necesidad de ver que tipo de objeto es*/

        
           
            GameObject wheelcart = GameObject.FindGameObjectWithTag("DefendableObject");
            
            if (wheelcart == null) return;
            if (Vector3.Distance(this.transform.position, wheelcart.transform.position) < 10 && wheelcart.GetComponent<WheelcartController>().NeedRepair())
            {
                int logsToSend = wheelcart.GetComponent<WheelcartController>().NeededLogsToRepair();
                int logsInInventory = inventoryController.GetItemQuantity(ItemType.WoodLog);
                if (logsToSend >= logsInInventory)
                {
                    inventoryController.HandleUseItem(ItemType.WoodLog, logsInInventory);
                    wheelcart.GetComponent<WheelcartController>().StorageLog(logsInInventory);
                }
                else
                {
                    inventoryController.HandleUseItem(ItemType.WoodLog, logsToSend);
                    wheelcart.GetComponent<WheelcartController>().StorageLog(logsToSend);
                }
                return;
            }

            GameObject paymentEvent = GameObject.FindGameObjectWithTag("PaymentEvent");
            if (paymentEvent == null) return;
            
            if(Vector3.Distance(this.transform.position, paymentEvent.transform.position) < 5.5f)
            {
                PaymentEvent paymentEventController = paymentEvent.GetComponent<PaymentEvent>();
                if (paymentEventController == null) return;
                
                int bonesToSend = paymentEventController.NeededBonesToPay();
                int bonesInInventory = inventoryController.GetItemQuantity(ItemType.Bone);
                
                if (bonesToSend >= bonesInInventory)
                {
                    inventoryController.HandleUseItem(ItemType.Bone, bonesInInventory);
                    paymentEventController.StorageBones(bonesInInventory);
                }
                else
                {
                    inventoryController.HandleUseItem(ItemType.Bone, bonesToSend);
                    paymentEventController.StorageBones(bonesToSend);
                }
            }
    }
}