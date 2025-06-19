using Assets.Scripts.Interfaces;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDView : View
{
    [SerializeField] private Slider playerHealthBar = null;
    [SerializeField] private Slider wheelcartHealthBar = null;
    [SerializeField] private TextMeshProUGUI playerHealthText = null;
    [SerializeField] private TextMeshProUGUI wheelcartHealthText = null;
    [SerializeField] private Slider progressBar = null;
    [SerializeField] private GameObject inventoryMenu = null;

    private IHealthVariation playerHealthEvents;
    private IHealthVariation wheelcartHealthEvents;
    private IWheelcartDuration wheelcartDurationEvents;

    private void Start()
    {
        StartCoroutine("UpdateProgressBar");
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
        for (int i = 0; i <= progressBar.maxValue; i++)
        {
            yield return new WaitForSeconds(1f);
            progressBar.value++;
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

    public GameObject GetInvetoryMenu()
    {
        return inventoryMenu;
    }
}
