using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;

public class EnemyMockTestProjectile : MonoBehaviour, IDamageable
{
    [SerializeField] float lifePoints = 100;

    public string GetTag()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float damageAmout, string hittedById)
    {
        lifePoints -= damageAmout;
        Debug.Log("Vida: " + lifePoints);
    }
}
