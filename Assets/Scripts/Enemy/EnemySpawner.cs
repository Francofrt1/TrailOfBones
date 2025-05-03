using System.Collections;
using System.Collections.Generic;
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
    {
        timer = spawnInterval;

        if (spawnInterval == 0f)
        {
            SpawnEnemies();
            enabled = false; // Desactiva el script si no va a seguir spawneando
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = transform.position.y; // Mantiene altura fija en horizontal

            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }

    void SpawnInterval()
    {
        if (spawnInterval > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SpawnEnemies();
                timer = spawnInterval;
            }
        }

        //prueba
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    SpawnEnemies();
        //}
    }
}

