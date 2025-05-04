using System.Collections;
using System.Collections.Generic;
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
        Animator.SetBool("isMoving", isMoving);
    }

    // triggers attack animation
    public void SetAttackAnimation()
    {
        Animator.SetTrigger("attack");
    }

    // triggers take-damage animation
    public void SetTakeDamageAnimation()
    {
        Animator.SetTrigger("takeDamage");
    }

    // triggers death animation
    public void SetDieAnimation()
    {
        Animator.SetTrigger("dieTrigger");
    }

    
}
