using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using FishNet;
using Multiplayer.PlayerSystem;
using Steamworks;
using System;
using UnityEngine;
using Multiplayer.Utils;
using Multiplayer.Steam;
using Multiplayer;
using Multiplayer.PopupSystem;
using static GameManager;

public class MultiplayerMenu : BaseMonoBehaviour
{
    [SerializeField] private GameObject _selectorMenu;
    [SerializeField] private GameObject _partyMenu;
    [SerializeField] private GameObject _lobbyFinder;
    [SerializeField] private GameObject _buttonsContainer;

    protected override void RegisterEvents()
    {
        if (!SteamClient.IsValid)
        {
            try
            {
                SteamClient.Init(480);
            }
            catch (Exception e)
            {
                // Something went wrong - it's one of these:
                //
                //     Steam is closed?
                //     Can't find steam_api dll?
                //     Don't have permission to play app?
                //

                Debug.LogException(e);
                Application.Quit();
            }
        }

        PlayerClient.OnStartClient += UpdateMenu;
    }

    protected override void UnregisterEvents()
    {
        PlayerClient.OnStartClient -= UpdateMenu;
    }

    public void ReadyUp()
    {
        var player = NetworkExtensions.GetLocalPlayer();
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        } else
        {
            player.Cmd_ReadyUp();
        }
    }

    public void CreateParty()
    {
        ChangeCamera();
        GameManager.Instance.SetCurrentGameState(GameState.InLobby);
        InstanceFinder.TransportManager.GetTransport<Multipass>().SetClientTransport<FishyFacepunch.FishyFacepunch>();
        SteamLobby.Singleton.CreateLobbyAsync();
    }

    public void FindParty()
    {
        _buttonsContainer.SetActive(false);
        _lobbyFinder.SetActive(true);
    }

    public void CreatePartyLocal()
    {
        InstanceFinder.TransportManager.GetTransport<Multipass>().SetClientTransport<Tugboat>();
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
    }

    public void JoinPartyLocal()
    {
        InstanceFinder.TransportManager.GetTransport<Multipass>().SetClientTransport<Tugboat>();
        InstanceFinder.ClientManager.StartConnection();
    }

    private void UpdateMenu(PlayerClient client)
    {
        _selectorMenu.SetActive(!client);
        _partyMenu.SetActive(client);
    }

    public static void StartGame()
    {
        if (!IsAllPlayersReady())
        {
            PopupManager.Popup_Show(new PopupContent("CAN NOT START THE GAME", "ALL PLAYERS MUST BE READY TO START.", true));
            return;
        }

        GameManager.Instance.SetCurrentGameState(GameState.Playing);
    }

    public static bool IsAllPlayersReady()
    {
        foreach (PlayerClient client in PlayerConnectionManager.Instance.AllClients)
        {
            if (!client.IsReady.Value)
                return false;
        }

        return true;
    }

    public static void ChangeCamera()
    {
        var mainCamera = GameObject.Find("MainCamera");
        var partyCamera = GameObject.Find("PartyCamera");
        mainCamera.GetComponent<Camera>().enabled = false;
        partyCamera.GetComponent<Camera>().enabled = true;
    }
}
