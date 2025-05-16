using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Interfaces;

public class HUD : MonoBehaviour
{
    public Slider playerHealthBar = null;
    public Slider wheelcartHealthBar = null;
    public TextMeshProUGUI playerHealthText = null;
    public TextMeshProUGUI wheelcartHealthText = null;
    public Slider progressBar = null;

    private IHealthVariation playerHealthEvents;
    private IHealthVariation wheelcartHealthEvents;
    private IWheelcartDuration wheelcartDurationEvents;

    private void Start()
    {
        StartCoroutine("UpdateProgressBar");
    }

    private void OnEnable()
    {
    }

    public void UpdatePlayerHealthbar(float playerHealthAmount, float maxHealth)
    {
        playerHealthBar.maxValue = maxHealth;
        playerHealthBar.value = playerHealthAmount;
        playerHealthText.text = "HP: " + playerHealthAmount.ToString(); 
    }

    public void UpdateWheelcartHealthbar(float wheelcartHealthAmount, float maxHealth)
    {
        wheelcartHealthBar.maxValue = maxHealth;
        wheelcartHealthBar.value = wheelcartHealthAmount;
        wheelcartHealthText.text = "HP: " + wheelcartHealthAmount.ToString();
    }

    IEnumerator UpdateProgressBar()
    {
        for(int i = 0; i<= progressBar.maxValue; i++)
        {
            yield return new WaitForSeconds(1f);
            progressBar.value = progressBar.value+1f;
        }
    }

    private void SetProgressBarMaxLimit(float limit)
    {
        progressBar.maxValue = limit;
    }

    public void SetPlayerHealthEvent(IHealthVariation playerHealthVariation)
    {
        playerHealthEvents = playerHealthVariation;
        playerHealthEvents.OnHealthVariation += UpdatePlayerHealthbar;
    }

    public void SetWheelcartHealthEvent(IHealthVariation wheelcartHealthVariation)
    {
        wheelcartHealthEvents = wheelcartHealthVariation;
        wheelcartHealthEvents.OnHealthVariation += UpdateWheelcartHealthbar;
    }

    public void SetWheelcartDuration(IWheelcartDuration wheelcartDuration)
    {
        wheelcartDurationEvents = wheelcartDuration;
        wheelcartDurationEvents.OnWheelcartDuration += SetProgressBarMaxLimit;
    }

    private void OnDestroy()
    {
    }
}
