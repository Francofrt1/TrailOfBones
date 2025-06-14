using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelcartBuff : MonoBehaviour
{
    [SerializeField] private float healAmountPerPlayer = 1f;
    [SerializeField] private float buffCooldown = 3f;
    private List<PlayerPresenter> playersInZone = new List<PlayerPresenter>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playersInZone.Add(other.GetComponent<PlayerPresenter>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playersInZone.Remove(other.GetComponent<PlayerPresenter>()); ;
        }
    }
    

    private void HealPlayersInZone()
    {
        foreach(PlayerPresenter player in playersInZone)
        {
            player.Heal(healAmountPerPlayer * playersInZone.Count);
        }
    }

    private IEnumerator HealAfterSeconds(float amount)
    {
        while (true)
        {
            yield return new WaitForSeconds(amount);
            HealPlayersInZone();
        }
    }

    private void Start()
    {
        StartCoroutine(HealAfterSeconds(buffCooldown));
    }

}
