using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class EnemyBase : MonoBehaviour
{
    public GameObject targetObject;
    public MovementTest[] players;
    public GameObject defendableObject;
    private NavMeshAgent agent;
    private Animator animator;
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
    }

    // Update is called once per frame
    void Update()
    {
        if(players.Any(p => p.GetEnemyCount() < 2))
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
    }
}
