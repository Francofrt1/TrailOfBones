using Multiplayer;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseView : View
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button continueButton;

    public static event Action OnPauseMenuHide;

    public override void Initialize()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        continueButton.onClick.AddListener(OnContinueButtonClicked);

        base.Initialize();
    }

    private void OnMainMenuButtonClicked()
    {
        GameManager.Instance.SetCurrentGameState(GameManager.GameState.End);
        ScenesManager.ChangeScene("MainMenu");
        GameManager.Instance.SetCurrentGameState(GameManager.GameState.InMenu);
    }

    private void OnContinueButtonClicked()
    {
        OnPauseMenuHide?.Invoke();
    }
}
