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
    private GameObject loseScreen;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button winExitButton;
    [SerializeField] private Button loseExitButton;

    void Start()
    {
        pauseMenu = transform.Find("PauseMenu")?.gameObject;
        winScreen = transform.Find("WinScreen")?.gameObject;
        loseScreen = transform.Find("LoseScreen")?.gameObject;

        GameManager.Instance.OnGamePaused += PauseMenuVisibility;
        GameManager.Instance.OnWinScreen += WinScreenShow;
        GameManager.Instance.OnLoseScreen += LoseScreenShow;

        pauseMenu.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        if (continueButton != null) { continueButton.onClick.AddListener(OnContinueButtonClick); }
        if (exitButton != null) { exitButton.onClick.AddListener(ToMainMenu); }
        if (winExitButton != null) { winExitButton.onClick.AddListener(ToMainMenu); }
        if (loseExitButton != null) { loseExitButton.onClick.AddListener(ToMainMenu); }
    }

    void OnDisable()
    {
        GameManager.Instance.OnGamePaused -= PauseMenuVisibility;
        GameManager.Instance.OnWinScreen -= WinScreenShow;
        GameManager.Instance.OnLoseScreen -= LoseScreenShow;
    }

    private void PauseMenuVisibility(bool value)
    {
        if (pauseMenu == null) return;

        pauseMenu.SetActive(value);
    }

    private void OnContinueButtonClick()
    {
        GameManager.Instance.SetPauseGame(false);
    }

    private void WinScreenShow()
    {
        if (winScreen == null) return;

        winScreen.SetActive(true);
    }

    private void LoseScreenShow()
    {
        if (loseScreen == null) return;

        loseScreen.SetActive(true);
    }

    private void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
