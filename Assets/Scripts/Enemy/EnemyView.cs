using UnityEngine;

public class EnemyView : MonoBehaviour
{
    // animation 
    public Animator Animator { get; private set; }

    // initializes animation
    private void Start()
    {
        Animator = GetComponent<Animator>();
    }

    // triggers movement animation based on whether the enemy is moving
    public void SetMovingAnimation(bool isMoving)
    {
        if (IsDying()) return;
        Animator.SetBool("isMoving", isMoving);
    }

    // triggers attack animation
    public void SetAttackAnimation()
    {
        if (IsDying()) return;
        Animator.SetTrigger("attack");
    }

    // triggers take-damage animation
    public void SetTakeDamageAnimation()
    {
        if (IsDying()) return;
        Animator.SetTrigger("takeDamage");
    }

    // triggers death animation
    public void SetDieAnimation()
    {
        if (IsDying()) return;
        Animator.SetTrigger("dieTrigger");
    }

    public bool IsDying()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).IsName("Death");
    }
}
