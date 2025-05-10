using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    public int spawnCount = 1;
    public float spawnRadius = 5f;
    public float spawnInterval = 10f;
    public GameObject enemyPrefab;

    public event Action<List<EnemyController>> OnEnemiesSpawned;
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    /// <summary>
    /// Instantiates enemy objects around the current position within the given radius.
    /// PRECONDITION:
    ///     'enemyPrefab' must be assigned in the inspector.
    ///     'spawnCount' should be 1 or greater.
    /// </summary>
    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            List<EnemyController> spawnedEnemies = new List<EnemyController>();
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 randomPos = transform.position + UnityEngine.Random.insideUnitSphere * spawnRadius;
                randomPos.y = transform.position.y; //Keeps enemies on same Y axis

                var enemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity).GetComponent<EnemyController>();
                spawnedEnemies.Add(enemy);
            }
            OnEnemiesSpawned?.Invoke(spawnedEnemies);
        }
    }
}

