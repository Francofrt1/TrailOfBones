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
    public Animator Animator { get; private set; }

    // initializes animation
    private void Start()
    {
        Animator = GetComponent<Animator>();
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
            audioSource.Play();
        }
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
        
        if (audioSource.clip != hitSound)
        {
            audioSource.Stop();
            audioSource.clip = hitSound;
            audioSource.outputAudioMixerGroup = null;
            audioSource.Play();
        }
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

    public void SetMaxHealthBar(float maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void SetCurrentHealthBar(float currentHealth)
    {
        if(healthBar.GetComponent<CanvasGroup>().alpha == 0f) healthBar.GetComponent<CanvasGroup>().alpha = 1f;
        healthBar.value = currentHealth;
    }
}
