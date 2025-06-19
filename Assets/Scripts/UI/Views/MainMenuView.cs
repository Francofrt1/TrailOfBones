using Multiplayer;
using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenuView : View
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    public override void Initialize()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        optionsButton?.onClick.AddListener(OnOptionsButtonClicked);
        creditsButton.onClick.AddListener(OnCreditsButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);

        base.Initialize();
    }

    private void OnPlayButtonClicked()
    {
        ScenesManager.ChangeScene("MultiplayerSelector");
    }

    private void OnOptionsButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void OnCreditsButtonClicked()
    {
        ViewManager.Instance.Show<CreditsView>();
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
