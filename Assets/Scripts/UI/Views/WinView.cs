using Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class WinView : View
{
    [SerializeField] private Button mainMenuButton;

    public override void Initialize()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

        base.Initialize();
    }

    private void OnMainMenuButtonClicked()
    {
        GameManager.Instance.SetCurrentGameState(GameManager.GameState.End);
        ScenesManager.ChangeScene("MainMenu");
        GameManager.Instance.SetCurrentGameState(GameManager.GameState.InMenu);
    }
}
