using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject pauseMenu;
    private GameObject winScreen;
    private GameObject looseScreen;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button winExitButton;
    [SerializeField] private Button looseExitButton;

    void Start()
    {
        pauseMenu = transform.Find("PauseMenu")?.gameObject;
        winScreen = transform.Find("WinScreen")?.gameObject;
        looseScreen = transform.Find("LooseScreen")?.gameObject;

        GameManager.Instance.OnGamePaused += PauseMenuVisibility;
        GameManager.Instance.OnWinScreen += WinScreenShow;
        GameManager.Instance.OnLooseScreen += WinScreenShow;

        if (continueButton != null) { continueButton.onClick.AddListener(OnContinueButtonClick); }
        if (exitButton != null) { exitButton.onClick.AddListener(ToMainMenu); }
        if (winExitButton != null) { winExitButton.onClick.AddListener(ToMainMenu); }
        if (looseExitButton != null) { looseExitButton.onClick.AddListener(ToMainMenu); }
    }

    void Onsable()
    {
        GameManager.Instance.OnGamePaused -= PauseMenuVisibility;
    }

    private void PauseMenuVisibility(bool value)
    {
        if (pauseMenu == null) return;

        pauseMenu.SetActive(value);
    }

    private void OnContinueButtonClick()
    {
        GameManager.Instance.UnpauseGame();
    }

    private void WinScreenShow()
    {
        if (winScreen == null) return;

        winScreen.SetActive(true);
    }

    private void LooseScreenShow()
    {
        if (looseScreen == null) return;

        looseScreen.SetActive(true);
    }

    private void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
