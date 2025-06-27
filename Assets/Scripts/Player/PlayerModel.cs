using FishNet.Object;
using System;
using UnityEngine;

public class PlayerModel : NetworkBehaviour
{
    public string ID { get; private set; }
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }
    public float baseAttackSpeed = 1f;
    public float baseDamage = 10f;
    public float currentAttackSpeed;
    private bool isPerformingAttack = false;
    public float currentShield = 0;
    //If moveSpeed default changed, walk and run animation transitions must be changed too
    public float moveSpeed = 5f;
    public float acceleration = 1f;
    private bool isGrounded;
    private float fallingTime = 0f;
    public float jumpForce = 7f;
    [SerializeField] private float defaultAcceleration = 1f;
    [SerializeField] private float sprintAcceleration = 2f;
    private float yawRotation = 0f;
    public bool isDead { get; set; } = false;
    private void Awake()
    {
        ID = Guid.NewGuid().ToString();
        currentHealth = maxHealth;
        isDead = false;
        currentAttackSpeed = baseAttackSpeed;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            this.enabled = false;
            return;
        }
    }

    public Vector3 CalculateLocalVelocity(Vector2 input)
    {
        Vector3 direction = new Vector3(input.x, 0f, input.y);
        return direction * moveSpeed * acceleration;
    }

    public void ToggleSprint()
    {
        acceleration = acceleration == defaultAcceleration ? sprintAcceleration : defaultAcceleration;
    }

    public Quaternion UpdateYawRotation(float delta)
    {
        yawRotation += delta;
        return Quaternion.Euler(0f, yawRotation, 0f);
    }

    public void SetHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (currentHealth <= 0)
        {
            isDead = true;
        }
    }

    public void SetAttackSpeed(float multiplier, float duration)
    {
        currentAttackSpeed = baseAttackSpeed * multiplier;
    }

    public void SetAttackState(bool state)
    {
        isPerformingAttack = state;
    }

    public bool CanAttack()
    {
        return !isDead && !isPerformingAttack;
    }

    public void SetShield(int amount, float duration)
    {
        currentShield = amount;
    }

    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }

    public float GetFallingTime() { return fallingTime; }
    public void ResetFallingTime() { fallingTime = 0f; }
    public void UpdateFallingTime(float time) { fallingTime += time; }
}