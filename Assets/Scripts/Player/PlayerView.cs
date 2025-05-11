using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerView : MonoBehaviour
{
    private Animator animator;
    public AudioClip stepSound;
    public AudioClip attackSound;
    public AudioClip hitSound;
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
        if (attackSound != null)
            AudioSource.PlayClipAtPoint(attackSound, this.transform.position);
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

    public void PlayHitSound()
    {
        if (IsDying()) return;
        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, this.transform.position);
    }

    public void PlayStepSound()
    {
        if (IsDying()) return;
        if (stepSound != null)
            AudioSource.PlayClipAtPoint(stepSound, this.transform.position);
    }

    private void OnDestroy()
    {
        Destroy(animator);
    }
}
