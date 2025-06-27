using Multiplayer; 
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public sealed class MainMenuView : View
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button controlsButton; 

    
    [SerializeField] private Button backFromControlsButton;

    public override void Initialize()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        optionsButton?.onClick.AddListener(OnOptionsButtonClicked);
        creditsButton.onClick.AddListener(OnCreditsButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        controlsButton.onClick.AddListener(OnControlsButtonClicked); 

       
        backFromControlsButton.onClick.AddListener(OnBackFromControlsMainMenuClicked);

        base.Initialize(); 
    }

    private void OnPlayButtonClicked()
    {
        ScenesManager.ChangeScene("MultiplayerSelector");
    }

    private void OnOptionsButtonClicked()
    {
        
    }

    private void OnCreditsButtonClicked()
    {
        ViewManager.Instance.Show<CreditsView>();
    }

   
    public void OnControlsButtonClicked()
    {
        ViewManager.Instance.Show<ControlsView>(); 
    }

    
    public void OnBackFromControlsMainMenuClicked() 
    {
        
        ViewManager.Instance.Show<MainMenuView>();
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}