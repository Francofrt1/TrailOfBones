using Multiplayer;
using UnityEngine;
using static GameManager;
using UnityEngine.UI;
using Multiplayer.PlayerSystem;
using Multiplayer.PopupSystem;
using Multiplayer.Utils;
using FishNet;
using Multiplayer.Steam;
using TMPro;
using System;

public class PartyView : View
{
    public static event Action OnClassChanged;
    [SerializeField] private Button readyUpButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button leaveLobbyButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private TMP_Text className;
    private bool isMage = false;

    public override void Initialize()
    {
        readyUpButton.onClick.AddListener(OnReadyUpButtonClicked);
        startButton.onClick.AddListener(OnStartButtonClicked);
        leaveLobbyButton?.onClick.AddListener(OnLeaveLobbyButtonClicked);
        rightArrowButton?.onClick.AddListener(OnClassChangeButtonClicked);
        leftArrowButton?.onClick.AddListener(OnClassChangeButtonClicked);

        if (InstanceFinder.IsClientStarted)
        {
            startButton.gameObject.SetActive(false);
        }

        base.Initialize();
    }

    private void OnClassChangeButtonClicked()
    {
        OnClassChanged?.Invoke();
        isMage = !isMage;
        className.text = isMage ? "Mage" : "Warrior";
    }

    public override void Show(object args = null)
    {
        base.Show(args);
        if(args is bool)
        {
            startButton.gameObject.SetActive((bool)args);
        }
    }

    private void OnReadyUpButtonClicked()
    {
        var player = NetworkExtensions.GetLocalPlayer();
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }
        else
        {
            player.Cmd_ReadyUp();
        }
    }

    private void OnStartButtonClicked()
    {
        if (!IsAllPlayersReady())
        {
            PopupManager.Popup_Show(new PopupContent("CAN NOT START THE GAME", "ALL PLAYERS MUST BE READY TO START.", true));
            return;
        }

        GameManager.Instance.SetCurrentGameState(GameState.StartMatch);
    }

    private void OnLeaveLobbyButtonClicked()
    {
        SteamLobby.LeaveLobby();
        ViewManager.Instance.Show<HostJoinView>();
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
}
