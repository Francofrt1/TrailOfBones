using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public List<IDamageable> DamageablesInRange { get; private set; } = new List<IDamageable>();

    public void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            DamageablesInRange.Add(damageable);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && DamageablesInRange.Contains(damageable))
        {
            DamageablesInRange.Remove(damageable);
        }
    }
}
