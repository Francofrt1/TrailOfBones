using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public float duration = 5f; // Default duration of the PowerUp
    public abstract void Activate(GameObject player); // Method to be implemented by each PowerUp child
}

public class Health : PowerUp
{
    public int healAmount = 25;

    public override void Activate(GameObject player)
    {
        var stats = player.GetComponent<PlayerModel>();
        if (stats != null)
        {
            stats.SetHealth(healAmount);
            Debug.Log("Health PowerUp applied");
        }

        Destroy(gameObject);
    }
}

public class AttackSpeed : PowerUp
{
    public float multiplier = 1.5f;

    public override void Activate(GameObject player)
    {
        var stats = player.GetComponent<PlayerModel>();
        if (stats != null)
        {
            stats.SetAttackSpeed(multiplier, duration);
            Debug.Log("Attack Speed PowerUp applied");
        }

        Destroy(gameObject);
    }
}

public class Shield : PowerUp
{
    public int shieldAmount = 30;

    public override void Activate(GameObject player)
    {
        var stats = player.GetComponent<PlayerModel>();
        if (stats != null)
        {
            stats.SetShield(shieldAmount, duration);
            Debug.Log("Shield PowerUp applied");
        }

        Destroy(gameObject);
    }
}
