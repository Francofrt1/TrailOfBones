using FishNet;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : NetworkBehaviour
{

    public int spawnCount = 1;
    public float spawnRadius = 5f;
    public float spawnInterval = 10f;
    public GameObject enemyPrefab;
    [SerializeField] private bool canSpawn = true;

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
                if (canSpawn)
                {
                    for (int i = 0; i < spawnCount; i++)
                    {
                        Vector3 randomPos = transform.position + UnityEngine.Random.insideUnitSphere * spawnRadius;
                        randomPos.y = transform.position.y; //Keeps enemies on same Y axis

                        SpawnEnemyOnServer(randomPos);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"SpawnEnemies failed: {ex.Message}");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnEnemyOnServer(Vector3 randomPos)
    {
        try
        {
            List<EnemyController> spawnedEnemies = new List<EnemyController>();
            NavMeshHit hit;
            // Compensates for terrain elevation by snapping the spawn position to the nearest point on the NavMesh:
            if (NavMesh.SamplePosition(randomPos, out hit, 2.0f, NavMesh.AllAreas))
            {
                GameObject enemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);

                ServerManager.Spawn(enemy);
                spawnedEnemies.Add(enemy.GetComponent<EnemyController>());
            }
            OnEnemiesSpawned?.Invoke(spawnedEnemies);
        }
        catch (Exception ex)
        {
            Debug.Log($"SpawnEnemyOnServer failed: {ex.Message}");
        }
    }

    public void SetCanSpawn(bool value)
    {
        canSpawn = value;
    }
}

