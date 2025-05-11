using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Interfaces;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EnemyModel))]
[RequireComponent(typeof(EnemyView))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IDamageable, IAttack, IDeath
{
    // References to the current target, player list, and defendable object
    public GameObject targetObject;
    public GameObject defendableObject { get; private set; }

    // Component and logic references
    private NavMeshAgent agent;
    private EnemyModel model;
    private EnemyView view;

    private AttackArea attackArea;

    public event Action<EnemyController, bool, string> OnEnemyKilled;

    private void Awake()
    {
        // gets model and view components
        model = GetComponent<EnemyModel>();
        view = GetComponent<EnemyView>();
        attackArea = GetComponentInChildren<AttackArea>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.Log("No NavMesh found, destroyed.");
            Destroy(gameObject);
        }

        agent.updatePosition = true;
        agent.updateUpAxis = true;
        agent.updateRotation = true;
        agent.speed = 3;

        StartCoroutine(AttackCheck());
        if (targetObject == null) return;
        Vector3 target = targetObject.transform.position;
        agent.SetDestination(target);
    }

    void Update()
    {
        if (targetObject == null) return;

        bool isMoving = false;
        // move to target 
        Debug.Log($"Distance to target: {agent.remainingDistance}");
        if (agent.remainingDistance >= agent.stoppingDistance)
        {
            agent.isStopped = false;
            Vector3 target = targetObject.transform.position;
            agent.SetDestination(target);

            isMoving = agent.velocity.magnitude > 0.1f;
        }
        else
        {
            isMoving = false;
        }

        view.SetMovingAnimation(isMoving);
    }
    public void OnAttack()
    {
        view.SetAttackAnimation();

        foreach (IDamageable damageable in attackArea.DamageablesInRange)
        {
            if (damageable.GetTag() == "Enemy") continue; // ignore other enemies
            damageable.TakeDamage(model.baseDamage, model.ID);

            Debug.Log($"{model.baseDamage} done to {damageable.GetTag()}");
        }
    }

    private IEnumerator AttackCheck()
    {
        while (true)
        {
            if (targetObject != null && attackArea.DamageablesInRange.Count > 0)
            {
                OnAttack();
            }
            yield return new WaitForSeconds(model.attackCooldown);
        }
    }

    public void OnDeath(string killedById)
    {
        OnEnemyKilled?.Invoke(this, model.inPlayer, killedById);
        Debug.Log("Enemy died.");
        view.SetDieAnimation();
    }

    public void TakeDamage(float damageAmout, string killedById)
    {
        model.SetHealth(model.currentHealth - damageAmout);
        view.SetTakeDamageAnimation();
        if (model.currentHealth <= 0)
        {
            OnDeath(killedById);
        }
    }

    public string GetTag()
    {
        return gameObject.tag;
    }

    public void SetIsEnemyOnPlayer(bool isEnemyOnPlayer)
    {
        model.inPlayer = isEnemyOnPlayer;
    }

    public bool GetIsEnemyOnPlayer()
    {
        return model.inPlayer;
    }
}