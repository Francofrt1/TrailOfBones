using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    public int spawnCount = 1;
    public float spawnRadius = 5f;
    public float spawnInterval = 0f;
    public GameObject enemyPrefab;
    private float timer;
     
    void Start()
    {
        InitializeTimer();
    }
    void Update()
    {
        SpawnInterval();
    }



    void InitializeTimer()
    {   /*PURPOSE:
            * Initializes the spawn timer based on the configured interval.
            PRECONDITION:
            * 'spawnInterval' must be set before this method is called.
        */
        timer = spawnInterval;

        if (spawnInterval == 0f)
        {
            SpawnEnemies();
            enabled = false; // Disables the script if it won't continue spawning
        }
    }

    void SpawnEnemies()
    {   /*PURPOSE:
            * Instantiates enemy objects around the current position within the given radius.
            PRECONDITION:
            * 'enemyPrefab' must be assigned in the inspector.
            * 'spawnCount' should be 1 or greater.
        */
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = transform.position.y; //Keeps enemies on same Y axis

            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }

    void SpawnInterval()
    {   /*PURPOSE:
            * Controls timed spawning of enemies at regular intervals.
            PRECONDITION:
            * 'spawnInterval' must be greater than 0 for this to take effect.
        */
        if (spawnInterval > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SpawnEnemies();
                timer = spawnInterval;
            }
        }

        // test manual spawn (disabled)
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    SpawnEnemies();
        //}
    }
}

