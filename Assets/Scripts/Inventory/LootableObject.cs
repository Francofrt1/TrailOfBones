using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableObject : MonoBehaviour, IDamageable, IDeath
{
    private float health = 30f;
    [SerializeField]
    private GameObject item;

    public void TakeDamage(float damageAmout, string hittedById)
    {
        health -= damageAmout;

        if(health <= 0) { 
            OnDeath(hittedById); 
        }
    }

    public void OnDeath(string killedById)
    {

        Instantiate(item,transform.position,transform.rotation);
        Destroy(gameObject);
    }

    public string GetTag() { return gameObject.tag; }
}
