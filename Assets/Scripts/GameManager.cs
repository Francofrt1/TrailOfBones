using Assets.Scripts.Interfaces;
using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Multiplayer;
using Multiplayer.PlayerSystem;
using Multiplayer.Steam;
using Multiplayer.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class GameManager : NetworkBehaviour
{
    public enum GameState
    {
        None,
        InMenu,
        InLobby,
        Loading,
        StartMatch,
        Playing,
        End
    }
    public static GameManager Instance { get; private set; }

    private HUDView hudView;
    private WheelcartMovement wheelcartMovement;
    private int deadPlayers = 0;
    public readonly SyncVar<bool> MatchWin = new SyncVar<bool>();
    public readonly SyncVar<bool> GameOver = new SyncVar<bool>();

    [SerializeField] private GameObject wheelCartPrefab;    

    [field: SerializeField]
    public GameState CurrentGameState { get; private set; } = GameState.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        SetCurrentGameState(GameState.InMenu);
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        MatchWin.OnChange += WinScreen;
        GameOver.OnChange += GameOverScreen;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
    }

    private void UnregisterEvents()
    {
        MatchWin.OnChange -= WinScreen;
        GameOver.OnChange -= GameOverScreen;
        PlayerPresenter.OnPlayerSpawned -= HandlePlayerSpawned;
        PlayerPresenter.OnPlayerSpawned -= _subscribeToPlayerPresenter;
        wheelcartMovement.Completed -= () => Cmd_WinMatch();
    }

    private void _subscribeToPlayerPresenter(IHealthVariation playerHealthEvents)
    {
        try
        {
            if (playerHealthEvents == null) return;
            hudView.SetPlayerHealthEvent(playerHealthEvents);
            playerHealthEvents.OnDie += HandlePlayerDeath;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private void _subscribeToWheelcart()
    {
        try
        {
            var wheelCart = GameObject.FindObjectOfType<WheelcartController>();
            var wheelcartDurationEvent = wheelCart.GetComponent<IWheelcartDuration>();
            var wheelcartEvents = wheelCart.GetComponent<IHealthVariation>();
            var wheelcartStopEvent = wheelCart.GetComponent<IStopWheelcart>();
            wheelcartEvents.OnDie += () => Cmd_GameOver();
            wheelcartMovement.Completed += () => Cmd_WinMatch();
            hudView.SetWheelcartHealthEvent(wheelcartEvents);
            hudView.SetWheelcartDuration(wheelcartDurationEvent);
            hudView.SetWheelcartBlockedEvent(wheelcartStopEvent);
            wheelCart.GetComponent<WheelcartController>().OnWheelcartSpawned();
        }
        catch (Exception ex)
        {
            Debug.Log($"_subscribeToWheelcart error {ex.Message}");
        }
    }

    void Start()
    {
        Debug.Log("Game Started");
    }

    private void WinScreen(bool prev, bool value, bool asserver)
    {
        Time.timeScale = 0f;
        SetCursorState(true);
        ViewManager.Instance.Show<WinView>();
        Debug.Log("Game Over, you win.");
    }

    public void GameOverScreen(bool prev, bool value, bool asserver)
    {
        Time.timeScale = 0f;
        SetCursorState(true);
        ViewManager.Instance.Show<LoseView>();
    }

    private void SetCursorState(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void SetCurrentGameState(GameState newState)
    {
        CurrentGameState = newState;

        switch (CurrentGameState)
        {
            case GameState.InMenu:
                InMenu();
                break;
            case GameState.InLobby:
                // Handle lobby state
                break;
            case GameState.Loading:
                break;
            case GameState.StartMatch:
                StartMatch();
                break;
            case GameState.Playing:
                break;
            case GameState.End:
                EndMatch();
                break;
            default:
                break;
        }
    }

    private void StartMatch()
    {
        try
        {
            InstanceFinder.SceneManager.OnLoadEnd += InitializeMatch;
            ScenesManager.ChangeScene("MainLevelMultiplayer", true);
            Debug.Log("Match started.");
        }
        catch (Exception ex)
        {
            Debug.Log($"Start match failed: {ex.Message}");
        }
    }

    private void InitializeMatch(SceneLoadEndEventArgs obj) {
        try
        {
            if (obj.LoadedScenes[0].name != "MainLevelMultiplayer") return;
            InstanceFinder.SceneManager.OnLoadEnd -= InitializeMatch;
            SetCurrentGameState(GameState.Playing);
            SpawnWheelCart();
            PlayerPresenter.OnPlayerSpawned += HandlePlayerSpawned;
            var audios = GetComponents<AudioSource>();
            
            foreach (var audio in audios)
            {
                audio.Play();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"InitializeMatch failed: {ex.Message}");
        }
    }

    private void SpawnWheelCart()
    {
        try
        {
            var wheelCartSpawnPosition = GameObject.Find("WheelcartSpawnPosition");
            var wheelcart = Instantiate(wheelCartPrefab, wheelCartSpawnPosition.transform.position, Quaternion.identity);
            ServerManager.Spawn(wheelcart);
            var spline = GameObject.Find("Spline");
            var splineContainer = spline.GetComponent<SplineContainer>();
            wheelcartMovement = wheelcart.GetComponent<WheelcartMovement>();
            wheelcartMovement.SetSpline(splineContainer);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SpawnWheelCart failed: {ex.Message}");
        }
    }

    private void HandlePlayerDeath()
    {
        deadPlayers++;
        int totalClients = GameObject.FindObjectsByType<PlayerClient>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
        if (deadPlayers >= totalClients)
        {
            Cmd_GameOver();
        }
    }

    private void EndMatch()
    {
        var clients = GameObject.FindObjectsByType<PlayerClient>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        UnregisterEvents();
        foreach (var client in clients)
        {
            Destroy(client.gameObject);
        }
        PlayerConnectionManager.Instance.Unsubscribe();
        PlayerSpawnManager.Instance.Unsubscribe();
        SteamLobby.LeaveLobby();
        Debug.Log("Game Over, you lose.");
    }

    private void InMenu()
    {
        deadPlayers = 0;

        var audios = GetComponents<AudioSource>();

        foreach (var audio in audios)
        {
            audio.Stop();
        }
    }

    private void HandlePlayerSpawned(PlayerPresenter player)
    {
        hudView = GameObject.FindObjectOfType<HUDView>(true);
        _subscribeToPlayerPresenter(player.gameObject.GetComponent<IHealthVariation>());
        player.gameObject.GetComponentInParent<PlayerClient>().OnPlayerSpawned();
        _subscribeToWheelcart();
    }

    public void Cmd_WinMatch()
    {
        MatchWin.Value = !MatchWin.Value;
    }

    [ObserversRpc(BufferLast = false)]
    private void Rpc_ShowGameOverScreen()
    {
        GameOverScreen(false, true, false);
    }

    public void Cmd_GameOver()
    {
        GameOver.Value = !GameOver.Value;
        Rpc_ShowGameOverScreen();
    }
}
