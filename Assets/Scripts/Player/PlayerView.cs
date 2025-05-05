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

    public void SetJumpAnimation()
    {
        animator.SetTrigger("Jump");
    }

    public void SetAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

    public void SetIsFallingAnimation(bool isFalling)
    {
        animator.SetBool("IsFalling", isFalling);
    }

    public void SetIsDeadAnimation()
    {
        animator.SetTrigger("Dead");
    }

    public bool IsAttacking()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    }   
}
