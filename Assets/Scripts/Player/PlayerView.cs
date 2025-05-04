using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerView : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMovementAnimation(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void SetJumpAnimation(bool isJumping)
    {
        animator.SetBool("IsJumping", isJumping);
    }

    public void SetAttackAnimation(bool isAttacking)
    {
        animator.SetBool("IsAttacking", isAttacking);
    }

    public void SetIsFallingAnimation(bool isFalling)
    {
        animator.SetBool("IsFalling", isFalling);
    }

    public void SetIsDeadAnimation(bool isDead)
    {
        animator.SetBool("IsDead", isDead);
    }
}
