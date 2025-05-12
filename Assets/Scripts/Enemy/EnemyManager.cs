using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<EnemyController> enemies = new List<EnemyController>();
    private List<PlayerController> playerControllers = new List<PlayerController>();
    private Dictionary<string, int> playerEnemies = new Dictionary<string, int>();
    private GameObject wheelcart;
    private int maxEnemiesToPlayer = 4;
    // Start is called before the first frame update
    void Start()
    {
        wheelcart = GameObject.Find("Wheelcart");
        var spawners = new List<EnemySpawner>(FindObjectsOfType<EnemySpawner>());
        spawners.ForEach(spawner => spawner.OnEnemiesSpawned += HandleEnemySpawned);
        playerControllers = new List<PlayerController>(FindObjectsOfType<PlayerController>());
        foreach (var player in playerControllers)
        {
            playerEnemies.Add(player.GetID(), 0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleEnemySpawned(List<EnemyController> spawnedEnemies)
    {
        enemies.AddRange(spawnedEnemies);
        foreach (var enemy in spawnedEnemies)
        {
            enemy.OnEnemyKilled += HandleEnemyKilled;
        }
        AssignEnemies(spawnedEnemies);
    }

    public void HandleEnemyKilled(EnemyController enemyKilled, bool inPlayer, string playerId)
    {
        enemies.Remove(enemyKilled);
        if (inPlayer)
        {
            playerEnemies[playerId] -= 1;
            ReassignEnemiesToPlayer(playerId);
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
        PlayerController player = playerControllers.FirstOrDefault(p => p.GetID() == playerId);
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
}