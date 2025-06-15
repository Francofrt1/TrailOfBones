using FishNet.Transporting.Multipass;
using FishNet;
using Multiplayer.PopupSystem;
using Multiplayer.Steam;
using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
        MultiplayerMenu.ChangeCamera();
        GameManager.Instance.SetCurrentGameState(GameState.InLobby);
        InstanceFinder.TransportManager.GetTransport<Multipass>().SetClientTransport<FishyFacepunch.FishyFacepunch>();
        SteamLobby.Singleton.JoinLobbyAsync(_lobby);
    }
}
