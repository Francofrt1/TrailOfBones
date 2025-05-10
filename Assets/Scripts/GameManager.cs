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
    public event Action<float> OnPlayerHealthUpdate;
    public event Action<float> OnWheelcartHealthUpdate;

    private SplineAnimate wheelcartSpline;
    private WheelcartController wheelcart;
    private InputHandler playerInputHandler;
    private PlayerController playerController;

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
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        if (playerController == null) return;
        playerController.playerDie += GameOverScreen;
        playerController.OnPlayerHealthVariation += UpdatePlayerHealthbar;
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
        wheelcart = GameObject.Find("Wheelcart").GetComponent<WheelcartController>();
        wheelcart.OnWheelcartDestroyed += GameOverScreen;
        wheelcartSpline.Completed += WinScreen;
        wheelcart.OnWheelcartHealthVariation += UpdateWheelcartHealthbar;
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
        if (wheelcart != null)
        {
            wheelcart.OnWheelcartDestroyed -= GameOverScreen;
            wheelcart.OnWheelcartHealthVariation -= UpdateWheelcartHealthbar;
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
        Debug.Log("Fin de la partida, ganaste");
    }

    public void GameOverScreen()
    {
        gameOver = true;
        SetPausedState(true);
        OnLoseScreen?.Invoke();
        Debug.Log("Fin de la partida, perdiste");
    }

    private void TogglePause()
    {
        gamePaused = !gamePaused;
        SetPausedState(gamePaused);
        OnGamePaused?.Invoke(gamePaused);
        Debug.Log(gamePaused ? "Juego pausado" : "Juego resumido");
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

    public void UpdatePlayerHealthbar(float currentPlayerHealth)
    {
        OnPlayerHealthUpdate?.Invoke(currentPlayerHealth);
    }

    public void UpdateWheelcartHealthbar(float currentWheelcartHealth)
    {
        OnWheelcartHealthUpdate?.Invoke(currentWheelcartHealth);
    }
}
