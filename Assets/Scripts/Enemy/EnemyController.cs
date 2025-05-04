using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    // References to the current target, player list, and defendable object
    public GameObject targetObject { get; private set; }
    public MovementTest[] players { get; private set; }
    public GameObject defendableObject { get; private set; }

    // Component and logic references
    private NavMeshAgent agent;
    private EnemyModel model;
    private EnemyView view;
    private bool inPlayer = false;

    void Start()
    {
        // finds players and target on the scene
        players = GameObject.FindObjectsOfType<MovementTest>();
        defendableObject = GameObject.Find("DefendableObject"); 

        // gets navigation and view components
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = true;
        agent.updateUpAxis = true;
        agent.updateRotation = true;
        agent.speed = 3;

        view = GetComponent<EnemyView>();

        // initializes combat logic model
        model = new EnemyModel(100, 10, 2f); 
    }

    void Update()
    {
        // check if any player can receive more enemies
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

        // move toward the target
        Vector3 target = targetObject.transform.position;
        agent.SetDestination(target);
        bool isMoving = agent.velocity.magnitude > 0.1f;
        view.SetMovingAnimation(isMoving);

        // check the distance to attack
        if (targetObject != null && Vector3.Distance(transform.position, targetObject.transform.position) <= 2f)
        {
            if (model.CanAttack(Time.time))
            {
                Attack();           
            }
        }
    }

    private void Attack()
    {
        view.SetAttackAnimation();

        // check if it's attacking the player
        MovementTest player = targetObject.GetComponent<MovementTest>();
        if (player != null)
        {
            Debug.Log($"Dealt {model.attackDamage} damage to player {player.name}");
            /* TO DO when the Player class is ready :)
             * playerHealth.TakeDamage(model.AttackDamage);
             */
        }
        else if (targetObject.name == "DefendableObject")
        {
            Debug.Log($"Dealt {model.attackDamage} to DefendableObject");
            /* TO DO 
             * defendableHealth.TakeDamage(model.AttackDamage);
             */
        }
    }

    public void TakeDamage(int amount)
    {
        view.SetTakeDamageAnimation();
        model.TakeDamage(amount);
    }

    private void Die()
    {
        Debug.Log("Enemy died.");
        view.SetDieAnimation();
        Destroy(gameObject);
    }
}
