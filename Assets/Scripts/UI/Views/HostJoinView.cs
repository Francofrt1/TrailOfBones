using FishNet.Transporting.Multipass;
using FishNet;
using Multiplayer;
using Multiplayer.Steam;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class HostJoinView : View
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button backButton;

    public override void Initialize()
    {
        hostButton.onClick.AddListener(OnHostButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
        backButton?.onClick.AddListener(OnBackButtonClicked);

        base.Initialize();
    }

    private void OnHostButtonClicked()
    {
        //ChangeCamera();
        ViewManager.Instance.Show<PartyView>();
        GameManager.Instance.SetCurrentGameState(GameState.InLobby);
        InstanceFinder.TransportManager.GetTransport<Multipass>().SetClientTransport<FishyFacepunch.FishyFacepunch>();
        SteamLobby.Singleton.CreateLobbyAsync();
    }

    private void OnJoinButtonClicked()
    {
        ViewManager.Instance.Show<LobbyListView>();
    }

    private void OnBackButtonClicked()
    {
        ScenesManager.ChangeScene("MainMenu");
    }
}
