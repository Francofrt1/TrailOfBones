using Assets.Scripts.Interfaces;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableObject : NetworkBehaviour, IDamageable, IDeath
{
    private float health = 30f;
    [SerializeField]
    private GameObject item;
    [SerializeField]
    private ParticleSystem particles;

    public void TakeDamage(float damageAmout, string hittedById)
    {
        particles.Play();
        if (particles.isPlaying) Debug.Log("esta andando despues");
        health -= damageAmout;

        if(health <= 0) { 
            OnDeath(hittedById); 
        }
    }

    public void OnDeath(string killedById)
    {
        SpawnDrop();
        Destroy(gameObject);
    }

    public void SpawnDrop()
    {
        var loot = Instantiate(item, transform.position, transform.rotation);
        ServerManager.Spawn(loot);
    }

    public string GetTag() { return gameObject.tag; }
}
