using Multiplayer; 
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PauseView : View
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button controlsButton; 

    
    [SerializeField] private Button backFromControlsButton;

    public static event Action OnPauseMenuHide;

    public override void Initialize()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        continueButton.onClick.AddListener(OnContinueButtonClicked);
        controlsButton.onClick.AddListener(OnControlsButtonClicked);
        backFromControlsButton.onClick.AddListener(OnBackFromControlsPauseClicked);

        base.Initialize(); 
    }

    
    public override void Show(object args = null)
    {
        base.Show(args); 
        
    }

    
    public override void Hide()
    {
        base.Hide(); 
       
        
        Debug.Log("PauseView: Ocultada.");
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
        
        Time.timeScale = 1f; 
      
    }

    public void OnControlsButtonClicked()
    {
        ViewManager.Instance.Show<ControlsView>(); 
        
        Time.timeScale = 0f;
        
    }

    public void OnBackFromControlsPauseClicked()
    {
        ViewManager.Instance.Show<PauseView>(); 
      
       
    }
}
