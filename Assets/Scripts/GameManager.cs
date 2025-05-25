using Assets.Scripts.Interfaces;
using FishNet;
using FishNet.Component.Spawning;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using Multiplayer;
using Multiplayer.Utils;
using System;
using UnityEngine;
using UnityEngine.Splines;

public class GameManager : BaseNetworkBehaviour
{
    public enum GameState
    {
        None,
        InMenu,
        InLobby,
        Loading,
        Pause,
        Playing,
        End
    }
    public static GameManager Instance { get; private set; }
    public bool gameOver = false;
    public bool gamePaused = false;
    public bool winConditionReached = false;

    public event Action<bool> OnGamePaused;
    public event Action OnWinScreen;
    public event Action OnLoseScreen;

    private HUD HUD;
    private SplineAnimate wheelcartSpline;
    private InputHandler playerInputHandler;
    [SerializeField]
    private GameState currentGameState = GameState.None;

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

    private void _subscribeToPlayerController(IHealthVariation playerHealthEvents)
    {
        //IHealthVariation playerHealthEvents = GameObject.Find("Player").GetComponent<IHealthVariation>();
        if (playerHealthEvents == null) return;
        HUD.SetPlayerHealthEvent(playerHealthEvents);
        playerHealthEvents.OnDie += GameOverScreen;
    }

    private void _subscribeToPlayerInputHandler(GameObject player)
    {
        if (player == null) return;
        playerInputHandler = player.GetComponent<InputHandler>();
        if (playerInputHandler == null) return;
        playerInputHandler.OnPauseTogglePerformed += TogglePause;
    }

    private void _subscribeToWheelcart()
    {
        var wheelCartGameObject = GameObject.FindGameObjectWithTag("DefendableObject");
        wheelcartSpline = wheelCartGameObject.GetComponent<SplineAnimate>();
        var wheelcartDurationEvent = wheelCartGameObject.GetComponent<IWheelcartDuration>();
        var wheelcartEvents = wheelCartGameObject.GetComponent<IHealthVariation>();
        wheelcartEvents.OnDie += GameOverScreen;
        wheelcartSpline.Completed += WinScreen;
        HUD.SetWheelcartHealthEvent(wheelcartEvents);
        HUD.SetWheelcartDuration(wheelcartDurationEvent);
    }

    private void OnEnable()
    {
        
    }

    void OnDisable()
    {
        if (playerInputHandler != null)
        {
            playerInputHandler.OnPauseTogglePerformed -= TogglePause;
        }
        if (wheelcartSpline != null)
        {
            wheelcartSpline.Completed -= WinScreen;
        }
    }

    void Start()
    {
        Debug.Log("Game Started");
    }

    public void WinScreen()
    {
        winConditionReached = true;
        SetPausedState(true);
        OnWinScreen?.Invoke();
        Debug.Log("Game Over, you win.");
    }

    public void GameOverScreen()
    {
        gameOver = true;
        SetPausedState(true);
        OnLoseScreen?.Invoke();
        Debug.Log("Game Over, you lose.");
    }

    private void TogglePause()
    {
        gamePaused = !gamePaused;
        SetPausedState(gamePaused);
        OnGamePaused?.Invoke(gamePaused);
        Debug.Log(gamePaused ? "Game paused" : "Game resumed");
    }

    private void SetPausedState(bool paused)
    {
        //Time.timeScale = paused ? 0f : 1f;
        SetCursorState(paused);
    }

    public void SetPauseGame(bool value)
    {
        if (gamePaused != value) { TogglePause(); }
    }

    private void SetCursorState(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    protected override void RegisterEvents()
    {
    }

    protected override void UnregisterEvents()
    {
    }

    public void SetCurrentGameState(GameState newState)
    {
        currentGameState = newState;

        switch (currentGameState)
        {
            case GameState.InMenu:
                // Handle menu state
                break;
            case GameState.InLobby:
                // Handle lobby state
                break;
            case GameState.Loading:
                // Handle loading state
                break;
            case GameState.Pause:
                SetPausedState(true);
                break;
            case GameState.Playing:
                StartMatch();
                break;
            case GameState.End:
                // Handle end state
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
            // Additional logic to start the match, like spawning players, etc.
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
            var playerSpawner = GameObject.Find("NetworkManager").GetComponent<PlayerSpawner>();
            playerSpawner.OnSpawned += (player) =>
            {
                _subscribeToPlayerController(player.GetComponent<IHealthVariation>());
                _subscribeToPlayerInputHandler(player.gameObject);
            };
            var hudObj = GameObject.Find("HUD");
            HUD = hudObj.GetComponent<HUD>();
            _subscribeToWheelcart();
            var audios = GetComponents<AudioSource>();
            foreach (var audio in audios)
            {
                audio.Play();
            }

            InstanceFinder.SceneManager.OnLoadEnd -= InitializeMatch;
        }
        catch (Exception ex)
        {
            Debug.Log($"InitializeMatch failed: {ex.Message}");
        }
    }
}
