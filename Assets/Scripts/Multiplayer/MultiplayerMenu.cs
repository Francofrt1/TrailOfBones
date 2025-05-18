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

public class MultiplayerMenu : BaseMonoBehaviour
{
    [SerializeField] private GameObject _selectorMenu;
    [SerializeField] private GameObject _partyMenu;

    private bool steamworksInitialized = false;
    protected override void RegisterEvents()
    {
        if (!SteamClient.IsValid)
        {
            try
            {
                SteamClient.Init(480);
                steamworksInitialized = true;
            }
            catch (Exception e)
            {
                // Something went wrong - it's one of these:
                //
                //     Steam is closed?
                //     Can't find steam_api dll?
                //     Don't have permission to play app?
                //

                steamworksInitialized = false;

                Debug.LogException(e);
                Application.Quit();
            }
            steamworksInitialized = false;
        }

        PlayerClient.OnStartClient += UpdateMenu;
    }

    protected override void UnregisterEvents()
    {
        PlayerClient.OnStartClient -= UpdateMenu;
    }

    public void ReadyUp()
    {
        NetworkExtensions.GetLocalPlayer().Cmd_ReadyUp();
    }

    public void CreateParty()
    {
        ChangeCamera();
        InstanceFinder.TransportManager.GetTransport<Multipass>().SetClientTransport<FishyFacepunch.FishyFacepunch>();
        SteamLobby.Singleton.CreateLobbyAsync();
    }

    public void JoinParty()
    {
        ChangeCamera();
        InstanceFinder.TransportManager.GetTransport<Multipass>().SetClientTransport<FishyFacepunch.FishyFacepunch>();
        SteamLobby.Singleton.FindLobby();
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

    //public void StartGame()
    //{
    //    //Game.GameSystem.GameManager.StartGame();
    //}

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

        ScenesManager.ChangeScene("MainLevelMultiplayer", true);
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

    private void ChangeCamera()
    {
        var mainCamera = GameObject.Find("MainCamera");
        var partyCamera = GameObject.Find("PartyCamera");
        mainCamera.GetComponent<Camera>().enabled = false;
        partyCamera.GetComponent<Camera>().enabled = true;
    }
}
