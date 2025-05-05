using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Assets.Scripts.Interfaces;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EnemyModel))]
[RequireComponent(typeof(EnemyView))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IDamageable, IAttack, IDeath
{
    // References to the current target, player list, and defendable object
    public GameObject targetObject { get; private set; }
    public List<PlayerController> players { get; private set; }
    public GameObject defendableObject { get; private set; }

    // Component and logic references
    private NavMeshAgent agent;
    private EnemyModel model;
    private EnemyView view;

    private AttackArea attackArea;
    private int maxAssignedEnemiesToPlayer = 4;

    void Start()
    {
        // finds players and target on the scene
        players = GameObject.FindGameObjectsWithTag("Player").Select(x => x.GetComponent<PlayerController>()).ToList();
        defendableObject = GameObject.Find("Wheelcart");

        // gets navigation and view components
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = true;
        agent.updateUpAxis = true;
        agent.updateRotation = true;
        agent.speed = 3;

        view = GetComponent<EnemyView>();

        // initializes combat logic model
        model = GetComponent<EnemyModel>();

        attackArea = GetComponentInChildren<AttackArea>();
    }

    void Update()
    {
        // check if any player can receive more enemies
        if (players.Any(p => p.GetEnemyCount() < maxAssignedEnemiesToPlayer))
        {
            PlayerController player = players.First(p => p.GetEnemyCount() < maxAssignedEnemiesToPlayer);
            player.AddEnemy(gameObject);
            targetObject = player.gameObject;
            model.inPlayer = true;
        }
        else if (!model.inPlayer)
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
                OnAttack();           
            }
        }
    }

    public void OnAttack()
    {
        view.SetAttackAnimation();

        foreach (IDamageable damageable in attackArea.DamageablesInRange)
        {
            if (damageable.GetTag() == "Enemy") return;
            damageable.TakeDamage(model.baseDamage);
            Debug.Log($"{model.baseDamage} done to {damageable.GetTag()}");
        }
    }

    public void OnDeath()
    {
        Debug.Log("Enemy died.");
        view.SetDieAnimation();
        Destroy(gameObject);
    }

    public void TakeDamage(float damageAmout)
    {
        model.SetHealth(model.currentHealth - damageAmout);
        view.SetTakeDamageAnimation();
        if (model.currentHealth <= 0)
        {
            OnDeath();
        }
    }

    public string GetTag()
    {
        return gameObject.tag;
    }
}
