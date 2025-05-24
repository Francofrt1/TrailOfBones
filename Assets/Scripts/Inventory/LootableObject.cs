using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableObject : MonoBehaviour, IDamageable, IDeath
{
    private float health = 30f;
    [SerializeField]
    private Item item;

    public void TakeDamage(float damageAmout, string hittedById)
    {
        health -= damageAmout;

        if(health <= 0) { 
            OnDeath(hittedById); 
        }
    }

    public void OnDeath(string killedById)
    {
        var position = transform;
        Destroy(gameObject);
        Instantiate(item, position);
    }

    public string GetTag() { return gameObject.tag; }
}
