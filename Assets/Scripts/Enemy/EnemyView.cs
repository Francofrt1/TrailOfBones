using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class EnemyView : MonoBehaviour
{
    //sound
    public AudioClip stepSound;
    public AudioClip hitSound;
    private AudioSource audioSource;
    public AudioMixerGroup audioMixerGroup;
    public Slider healthBar;
    // animation 
    private Animator Animator;
    private Animator NetworkAnimator;

    // initializes animation
    private void Start()
    {
        Animator = GetComponent<Animator>();
        NetworkAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // triggers movement animation based on whether the enemy is moving
    public void SetMovingAnimation(bool isMoving)
    {
        if (IsDying()) return;
        Animator.SetBool("isMoving", isMoving);
        if (audioSource.clip != stepSound)
        {
            audioSource.clip = stepSound;
            audioSource.outputAudioMixerGroup = null;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // triggers attack animation
    public void SetAttackAnimation()
    {
        if (IsDying()) return;
        NetworkAnimator.SetTrigger("attack");
    }

    // triggers take-damage animation
    public void SetTakeDamageAnimation()
    {
        if (IsDying()) return;
        NetworkAnimator.SetTrigger("takeDamage");

        if (audioSource.clip != hitSound)
        {
            audioSource.Stop();
            audioSource.clip = hitSound;
            audioSource.outputAudioMixerGroup = null;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    // triggers death animation
    public void SetDieAnimation()
    {
        if (IsDying()) return;
        NetworkAnimator.SetTrigger("dieTrigger");
    }

    public bool IsDying()
    {
        return NetworkAnimator.GetCurrentAnimatorStateInfo(0).IsName("Death");
    }

    public float GetCurrentAnimationClipLength()
    {
        AnimatorClipInfo[] clipInfo = NetworkAnimator.GetCurrentAnimatorClipInfo(0);
        return clipInfo.Length > 0 ? clipInfo[0].clip.length : 0f;
    }

    public void SetMaxHealthBar(float maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void SetCurrentHealthBar(float currentHealth)
    {
        if (healthBar.GetComponent<CanvasGroup>().alpha == 0f) healthBar.GetComponent<CanvasGroup>().alpha = 1f;
        healthBar.value = currentHealth;
    }
}
