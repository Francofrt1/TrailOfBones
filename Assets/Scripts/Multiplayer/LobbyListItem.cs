using FishNet.Transporting.Multipass;
using FishNet;
using Multiplayer.Steam;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using static GameManager;

public class LobbyListItem : MonoBehaviour
{
    private Lobby _lobby;
    [SerializeField] private TMP_Text _lobbyText;

    public void Setup(Lobby lobby)
    {
        _lobby = lobby;
        _lobbyText.text = $"Host: {lobby.GetData("Name")} - Capacity: ({lobby.MemberCount}/{lobby.MaxMembers})";
    }

    public void JoinLobby()
    {
        GameManager.Instance.SetCurrentGameState(GameState.InLobby);
        InstanceFinder.TransportManager.GetTransport<Multipass>().SetClientTransport<FishyFacepunch.FishyFacepunch>();
        SteamLobby.Singleton.JoinLobbyAsync(_lobby);
        ViewManager.Instance.Show<PartyView>();
    }
}
