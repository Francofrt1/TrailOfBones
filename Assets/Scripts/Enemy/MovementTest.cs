using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public List<GameObject> enemies;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetEnemyCount()
    {
        return enemies.Count();
    }

    public void AddEnemy(GameObject newEnemy)
    {
        enemies.Add(newEnemy);
    }
}
