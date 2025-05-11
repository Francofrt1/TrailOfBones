using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public Slider playerHealthBar = null;
    public Slider wheelcartHealthBar = null;
    public TextMeshProUGUI playerHealthText = null;
    public TextMeshProUGUI wheelcartHealthText = null;
    public Slider progressBar = null;

    private void Start()
    {
        GameManager.Instance.onWheelcartTrailDuration += SetProgressBarMaxLimit;
        StartCoroutine("UpdateProgressBar");
    }
    private void OnEnable()
    {
        GameManager.Instance.OnWheelcartHealthUpdate += UpdateWheelcartHealthbar;
        GameManager.Instance.OnPlayerHealthUpdate += UpdatePlayerHealthbar;
        
    }

    private void OnDisable()
    {
        GameManager.Instance.OnWheelcartHealthUpdate -= UpdateWheelcartHealthbar;
        GameManager.Instance.OnPlayerHealthUpdate -= UpdatePlayerHealthbar;
    }
    public void UpdatePlayerHealthbar(float playerHealthAmount)
    {
        playerHealthBar.value = playerHealthAmount;
        playerHealthText.text = "HP: " + playerHealthAmount.ToString(); 
    }

    public void UpdateWheelcartHealthbar(float wheelcartHealthAmount)
    {
        wheelcartHealthBar.value = wheelcartHealthAmount;
        wheelcartHealthText.text = "Wheelcart HP: " + wheelcartHealthAmount.ToString();
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
}
