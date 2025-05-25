using FishNet;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : NetworkBehaviour
{

    public int spawnCount = 1;
    public float spawnRadius = 5f;
    public float spawnInterval = 10f;
    public GameObject enemyPrefab;

    public event Action<List<EnemyController>> OnEnemiesSpawned;
    void Start()
    {
        if(InstanceFinder.IsHostStarted)
            StartCoroutine(SpawnEnemies());
    }

    /// <summary>
    /// Instantiates enemy objects around the current position within the given radius.
    /// PRECONDITION:
    ///     'enemyPrefab' must be assigned in the inspector.
    ///     'spawnCount' should be 1 or greater.
    /// </summary>
    /// 
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            try
            {
                List<EnemyController> spawnedEnemies = new List<EnemyController>();
                for (int i = 0; i < spawnCount; i++)
                {
                    Vector3 randomPos = transform.position + UnityEngine.Random.insideUnitSphere * spawnRadius;
                    randomPos.y = transform.position.y; //Keeps enemies on same Y axis

                    NavMeshHit hit;
                    // Compensates for terrain elevation by snapping the spawn position to the nearest point on the NavMesh:
                    if (NavMesh.SamplePosition(randomPos, out hit, 2.0f, NavMesh.AllAreas))
                    {
                        GameObject enemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
                        if(enemy.IsPrefabInstance())
                        {
                            SpawnEnemyOnServer(enemy);
                        }
                        spawnedEnemies.Add(enemy.GetComponent<EnemyController>());
                    }
                }
                OnEnemiesSpawned?.Invoke(spawnedEnemies);
            }
            catch (Exception ex)
            {
                Debug.Log($"SpawnEnemies failed: {ex.Message}");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnEnemyOnServer(GameObject enemy)
    {
        try
        {
            ServerManager.Spawn(enemy);
        }
        catch (Exception ex)
        {
            Debug.Log($"SpawnEnemyOnServer failed: {ex.Message}");
        }
    }
}

