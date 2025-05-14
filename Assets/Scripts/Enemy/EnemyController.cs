using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Interfaces;
using System.Collections;
using System;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EnemyModel))]
[RequireComponent(typeof(EnemyView))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IDamageable, IAttack, IDeath
{
    private enum State
    {
        Attack,
        Hit,
        Move,
        Idle,
        Death
    }
    // References to the current target, player list, and defendable object
    public GameObject targetObject;
    // Component and logic references
    private NavMeshAgent agent;
    private EnemyModel model;
    private EnemyView view;

    private AttackArea attackArea;

    public event Action<EnemyController, bool, string> OnEnemyKilled;

    private State currentState = State.Idle;

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
        view.SetMaxHealthBar(model.maxHealth);
    }

    void Update()
    {
        HandleMovement();
    }

    public void HandleMovement()
    {
        State[] cantMoveStates = { State.Attack, State.Hit, State.Death };
        if (targetObject == null || cantMoveStates.Contains(currentState)) return;

        // move to target 
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            currentState = State.Idle;
        }
        else
        {
            agent.isStopped = false;
            currentState = State.Move;
        }

        Vector3 target = targetObject.transform.position;
        agent.SetDestination(target);

        view.SetMovingAnimation(currentState == State.Move);
    }

    public void OnAttack()
    {
        // ignore other enemies
        var damageables = attackArea.DamageablesInRange.Where(x => x.GetTag() != "Enemy");
        if (!damageables.Any()) return;
        currentState = State.Attack;
        view.SetAttackAnimation();
        foreach (IDamageable damageable in damageables)
        {
            damageable.TakeDamage(model.baseDamage, model.ID);

            Debug.Log($"Enemy did {model.baseDamage} damage to {damageable.GetTag()}");
        }
        currentState = State.Idle;
    }

    private IEnumerator AttackCheck()
    {
        while (true)
        {
            if (targetObject != null && currentState != State.Death)
            {
                OnAttack();
            }
            yield return new WaitForSeconds(model.attackCooldown);
        }
    }

    public void OnDeath(string killedById)
    {
        currentState = State.Death;
        OnEnemyKilled?.Invoke(this, model.inPlayer, killedById);
        Debug.Log("Enemy died.");
        view.SetDieAnimation();
        Destroy(gameObject, 5f);
    }

    public void TakeDamage(float damageAmout, string hittedById)
    {
        if (currentState == State.Death) return;
        model.SetHealth(model.currentHealth - damageAmout);
        currentState = State.Hit;
        view.SetTakeDamageAnimation();
        currentState = State.Idle;
        view.SetCurrentHealthBar(model.currentHealth);
        if (model.currentHealth <= 0)
        {
            OnDeath(hittedById);
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