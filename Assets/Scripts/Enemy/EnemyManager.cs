using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : NetworkBehaviour
{
    [AllowMutableSyncType]
    private SyncList<EnemyController> enemies = new SyncList<EnemyController>();
    [AllowMutableSyncType]
    private SyncList<PlayerPresenter> playerControllers = new SyncList<PlayerPresenter>();
    [AllowMutableSyncType]
    private SyncDictionary<string, int> playerEnemies = new SyncDictionary<string, int>();
    private GameObject wheelcart;
    [SerializeField] private int maxEnemiesToPlayer = 4;
    [SerializeField] private int maxTotalEnemies = 10;
    private bool spawnersStopped = false;
    private void Awake()
    {
        PlayerPresenter.OnPlayerSpawned += SetPlayerSpawned;
    }

    void Start()
    {
        wheelcart = GameObject.FindGameObjectWithTag("DefendableObject");
        var spawners = new List<EnemySpawner>(FindObjectsOfType<EnemySpawner>());
        spawners.ForEach(spawner => spawner.OnEnemiesSpawned += HandleEnemySpawned);
    }

    public void SetPlayerSpawned(PlayerPresenter newPlayer)
    {
        playerControllers.Add(newPlayer);
        playerEnemies.Add(newPlayer.GetID(), 0);
    }

    public void HandleEnemySpawned(List<EnemyController> spawnedEnemies)
    {
        enemies.AddRange(spawnedEnemies);
        foreach (var enemy in spawnedEnemies)
        {
            enemy.OnEnemyKilled += HandleEnemyKilled;
        }
        AssignEnemies(spawnedEnemies);
        if (maxTotalEnemies <= enemies.Count)
        {
            StopSpawners();
            spawnersStopped = true;
        } 
    }

    public void HandleEnemyKilled(EnemyController enemyKilled, bool inPlayer, string playerId)
    {
        enemies.Remove(enemyKilled);
        if (inPlayer)
        {
            playerEnemies[playerId] -= 1;
            ReassignEnemiesToPlayer(playerId);
        }

        if (maxTotalEnemies > enemies.Count && spawnersStopped)
        {
            StartSpawners();
            spawnersStopped = false;
        }
    }

    public void AssignEnemies(List<EnemyController> spawnedEnemies)
    {
        if (playerEnemies.Any(x => x.Value != 4))
        {
            foreach (var player in playerControllers)
            {
                int currentEnemies = playerEnemies[player.GetID()];
                if (currentEnemies < maxEnemiesToPlayer)
                {
                    var enemiesToAssign = spawnedEnemies.Where(e => !e.GetIsEnemyOnPlayer()).Take(maxEnemiesToPlayer - currentEnemies).ToList();
                    foreach (var enemy in enemiesToAssign)
                    {
                        enemy.targetObject = player.gameObject;
                        enemy.SetIsEnemyOnPlayer(true);
                        playerEnemies[player.GetID()] += 1;
                    }
                }
            }
        }

        var enemiesToWheelcart = spawnedEnemies.Where(e => !e.GetIsEnemyOnPlayer()).ToList();
        foreach (var enemy in enemiesToWheelcart)
        {
            enemy.SetIsEnemyOnPlayer(false);
            enemy.targetObject = wheelcart;
        }
    }

    public void ReassignEnemiesToPlayer(string playerId)
    {
        PlayerPresenter player = playerControllers.FirstOrDefault(p => p.GetID() == playerId);
        int currentEnemies = playerEnemies[playerId];
        if (player == null) return;

        if (currentEnemies < maxEnemiesToPlayer)
        {
            var enemiesToAssign = enemies.Where(e => !e.GetIsEnemyOnPlayer()).Take(maxEnemiesToPlayer - currentEnemies).ToList();
            foreach (var enemy in enemiesToAssign)
            {
                enemy.targetObject = player.gameObject;
                enemy.SetIsEnemyOnPlayer(true);
                playerEnemies[playerId] += 1;
            }
        }
    }

    private void StopSpawners()
    {
        var spawners = FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in spawners)
        {
            spawner.SetCanSpawn(false);
        }
    }

    private void StartSpawners()
    {
        var spawners = FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in spawners)
        {
            spawner.SetCanSpawn(true);
        }
    }
}