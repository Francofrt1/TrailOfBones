using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Slider playerHealthBar = null;
    public Slider wheelcartHealthBar = null;

    public event Action UpdateHUD;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void UpdateHUDTrigger()
    {
        UpdateHUD?.Invoke();
    }

    public void UpdateHealthbars(int playerHealthAmount, int wheelcartHealthAmount)
    {
        playerHealthBar.value = playerHealthAmount;
        wheelcartHealthBar.value = wheelcartHealthAmount;
    }
}
