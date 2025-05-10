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
        if(IsDying()) return;
        animator.SetFloat("Speed", speed);
    }

    public void SetJumpAnimation()
    {
        if (IsDying()) return;
        animator.SetTrigger("Jump");
    }

    public void SetAttackAnimation()
    {
        if (IsDying()) return;
        animator.SetTrigger("Attack");
    }

    public void SetIsFallingAnimation(bool isFalling, float fallingTime = 0f)
    {
        if (IsDying()) return;
        animator.SetBool("IsFalling", isFalling);
        animator.SetFloat("FallingTime", fallingTime);
    }

    public void SetIsDeadAnimation()
    {
        if (IsDying()) return;
        animator.SetTrigger("Dead");
    }

    public bool IsAttacking()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    }

    public bool IsDying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Death");
    }

    private void OnDestroy()
    {
        Destroy(animator);
    }
}
