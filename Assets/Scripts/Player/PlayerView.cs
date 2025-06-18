using System;
using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerView : MonoBehaviour
{
    private Animator animator;

    public AudioClip attackSound;
    public AudioClip hitSound;

    public AudioClip[] stepSoundsArray;
    public event Action<bool> OnAttackStateChanged;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on PlayerView.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMovementAnimation(Vector3 localMove)
    {
        if (IsDying()) return;

        float speed = CalculateAnimationSpeed(localMove);
        animator.SetFloat("Speed", speed);
    }

    private float CalculateAnimationSpeed(Vector3 localMove)
    {
        Vector3 flat = new Vector3(localMove.x, 0f, localMove.z);
        return flat.magnitude;
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
        animator.SetBool("IsAttacking", true);
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

    public void CheckIsAttacking()
    {
        OnAttackStateChanged?.Invoke(animator.GetBool("IsAttacking"));
    }

    public bool IsDying()
    {
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on PlayerView.");
            return false;
        }
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

        if (stepSoundsArray != null && stepSoundsArray.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, stepSoundsArray.Length);
            AudioClip selectedStep = stepSoundsArray[index];

            if (selectedStep != null)
                AudioSource.PlayClipAtPoint(selectedStep, this.transform.position);
        }
    }

    private void OnDestroy()
    {
        Destroy(animator);
    }
}
