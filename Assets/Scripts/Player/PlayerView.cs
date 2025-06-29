using System;
using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerView : NetworkBehaviour
{
    private Animator animator;
    private NetworkAnimator networkAnimator;

    public AudioClip attackSound;
    public AudioClip hitSound;

    public AudioClip[] stepSoundsArray;
    public event Action<bool> OnAttackStateChanged;
    private bool isDying = false;
    private bool showPause = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on PlayerView.");
            return;
        }
        networkAnimator = GetComponent<NetworkAnimator>();

        PauseView.OnPauseMenuHide += TogglePauseMenu;
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

    public void SetMovementAnimation(Vector3 localMove)
    {
        if (isDying) return;

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
        if (isDying) return;
        networkAnimator.SetTrigger("Jump");
    }

    public void SetAttackAnimation()
    {
        if (isDying) return;
        networkAnimator.SetTrigger("Attack");
        animator.SetBool("IsAttacking", true);
        if (attackSound != null)
            AudioSource.PlayClipAtPoint(attackSound, this.transform.position);
    }

    public void SetIsFallingAnimation(bool isFalling, float fallingTime = 0f)
    {
        if (isDying) return;
        animator.SetBool("IsFalling", isFalling);
        animator.SetFloat("FallingTime", fallingTime);
    }

    public void SetIsDeadAnimation()
    {
        if (isDying) return;
        networkAnimator.SetTrigger("Dead");
        isDying = true;
    }

    public void CheckIsAttacking()
    {
        OnAttackStateChanged?.Invoke(animator.GetBool("IsAttacking"));
    }

    public void PlayHitSound()
    {
        if (isDying) return;
        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, this.transform.position);
    }

    public void PlayStepSound()
    {
        if (isDying) return;

        if (stepSoundsArray != null && stepSoundsArray.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, stepSoundsArray.Length);
            AudioClip selectedStep = stepSoundsArray[index];

            if (selectedStep != null)
                AudioSource.PlayClipAtPoint(selectedStep, this.transform.position);
        }
    }

    public void TogglePauseMenu()
    {
        showPause = !showPause;
        Cursor.visible = showPause;
        Cursor.lockState = showPause ? CursorLockMode.None : CursorLockMode.Locked;
        if (showPause)
        {
            ViewManager.Instance.Show<PauseView>();
        }
        else
        {
            ViewManager.Instance.Show<HUDView>();
        }
    }
}
