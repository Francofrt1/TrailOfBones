using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class EnemyBase : MonoBehaviour
{   //
    public int maxHealth = 100;
    private int currentHealth;
    public int attackDamage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0;
    //
    public GameObject targetObject;
    public MovementTest[] players;
    public GameObject defendableObject;
    private NavMeshAgent agent;
    public Animator animator;
    public float agentSpeedDefault = 3;
    private Vector3 currentPosition;
    private Vector3 target;
    public AudioClip attackSound;
    public AudioClip chaseSound;
    private AudioSource audioSource;
    public AudioMixerGroup mixerGroup;
    private bool inPlayer = false;
    void Start()
    {
        Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();
        players = GameObject.FindObjectsOfType<MovementTest>();
        defendableObject = GameObject.Find("DefendableObject");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = true;
        agent.updateUpAxis = true;
        agent.updateRotation = true;
        agent.speed = agentSpeedDefault;
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth; // 
    }

    // Update is called once per frame
    void Update()
    {
        
        if (players.Any(p => p.GetEnemyCount() < 2))
        {
            MovementTest player = players.First(p => p.GetEnemyCount() < 2);
            player.AddEnemy(gameObject);
            targetObject = player.gameObject;
            inPlayer = true;
        }
        else if (!inPlayer)
        {
            targetObject = defendableObject;
        }

        Debug.Log("Chasing");
        target = targetObject.transform.position;
        agent.SetDestination(target);
        bool isMoving = agent.velocity.magnitude > 0.1f; 
        animator.SetBool("isMoving", isMoving); 

        // check the distance to attack
        if (targetObject != null && Vector3.Distance(transform.position, targetObject.transform.position) <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

    }

    private void Attack() { 
        animator.SetTrigger("attack");
        // Check if it's attacking the player
        MovementTest player = targetObject.GetComponent<MovementTest>();
        if (player != null) {
            Debug.Log($"Dealt {attackDamage} damage to player {player.name}");
            /* TO DO when the Player class is ready :)
             * playerHealth.TakeDamage(attackDamage);
             */
        }
        else if (targetObject.name == "DefendableObject")
        {
            Debug.Log($"Dealt {attackDamage} to DefendableObject");


            /* TO DO 
             * defendableHealth.TakeDamage(attackDamage);
             */

            
        }
    }


    public void TakeDamage(int amount) { // TO DO, is provisory
        animator.SetTrigger("takeDamage");
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die() 
    {
        
        Debug.Log("Enemy died.");
        animator.SetTrigger("dieTrigger");
        Destroy(gameObject);
    }
}
