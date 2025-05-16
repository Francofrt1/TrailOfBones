using Assets.Scripts.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Splines;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool gameOver = false;
    public bool gamePaused = false;
    public bool winConditionReached = false;

    public event Action<bool> OnGamePaused;
    public event Action OnWinScreen;
    public event Action OnLoseScreen;
    public event Action<float, float> OnPlayerHealthUpdate;
    public event Action<float, float> OnWheelcartHealthUpdate;
    public event Action<float> onWheelcartTrailDuration;

    private SplineAnimate wheelcartSpline;
    private IWheelcartDuration wheelcartDurationEvent;
    private IHealthVariation wheelcartEvents;
    private InputHandler playerInputHandler;

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
    }

    private void _subscribeToPlayerController()
    {
        IHealthVariation playerHealthEvents = GameObject.Find("Player").GetComponent<IHealthVariation>();
        if (playerHealthEvents == null) return;
        GameObject.Find("HUD").GetComponent<HUD>().SetPlayerHealthEvent(playerHealthEvents);
        playerHealthEvents.OnDie += GameOverScreen;
    }

    private void _subscribeToPlayerInputHandler()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null) return;
        playerInputHandler = player.GetComponent<InputHandler>();
        if (playerInputHandler == null) return;
        playerInputHandler.OnPauseTogglePerformed += TogglePause;
    }

    private void _subscribeToWheelcart()
    {
        wheelcartSpline = GameObject.Find("Wheelcart").GetComponent<SplineAnimate>();
        wheelcartDurationEvent = GameObject.Find("Wheelcart").GetComponent<IWheelcartDuration>();
        wheelcartEvents = GameObject.Find("Wheelcart").GetComponent<IHealthVariation>();
        wheelcartEvents.OnDie += GameOverScreen;
        wheelcartSpline.Completed += WinScreen;
        wheelcartEvents.OnHealthVariation += UpdateWheelcartHealthbar;
        //change here later
        wheelcartDurationEvent.OnWheelcartDuration += SetUIGameDuration;
    }

    private void OnEnable()
    {
        _subscribeToPlayerInputHandler();
        _subscribeToWheelcart();
        _subscribeToPlayerController();
    }

    void OnDisable()
    {
        if (playerInputHandler != null)
        {
            playerInputHandler.OnPauseTogglePerformed -= TogglePause;
        }
        if (wheelcartSpline != null )
        {
            wheelcartDurationEvent.OnWheelcartDuration -= SetUIGameDuration;
        }
        if(wheelcartEvents != null)
        {
            wheelcartEvents.OnDie -= GameOverScreen;
            wheelcartEvents.OnHealthVariation -= UpdateWheelcartHealthbar;
        }
        if (wheelcartSpline != null)
        {
            wheelcartSpline.Completed -= WinScreen;
        }
    }

    void Start()
    {
        gameOver = false;
        SetPausedState(false);
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
        Time.timeScale = paused ? 0f : 1f;
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

    public void UpdatePlayerHealthbar(float currentPlayerHealth, float maxHealth)
    {
        OnPlayerHealthUpdate?.Invoke(currentPlayerHealth, maxHealth);
    }

    public void UpdateWheelcartHealthbar(float currentWheelcartHealth, float maxHealth)
    {
        OnWheelcartHealthUpdate?.Invoke(currentWheelcartHealth, maxHealth);
    }

    public void SetUIGameDuration(float duration)
    {
        onWheelcartTrailDuration?.Invoke(duration);
    }
}
