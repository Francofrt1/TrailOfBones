using System;
using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        // LOS COMENTARIOS DE AC� SON DEL EJEMPLO DEL PROFE
        // Creamos la instancia unica.
        // Si ya existe una instancia, destruye esta
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Es un metodo que permite mantener la informacion de un objeto(GameManager) al cargar una nueva escena.
        }
    }

    private void _suscribeToPlayerController()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        if (playerController == null) return;
        playerController.playerDie += GameOverScreen;
        playerController.OnPlayerHealthVariation += UpdatePlayerHealthbar;
    }
    private void _suscribeToPlayerInputHandler()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null) return;
        playerInputHandler = player.GetComponent<InputHandler>();
        if (playerInputHandler == null) return;
        playerInputHandler.OnPauseTogglePerformed += TogglePause;
    }

    private void _suscribeToWheelcart()
    {
        wheelcartSpline = GameObject.Find("Wheelcart").GetComponent<SplineAnimate>();
        wheelcart = GameObject.Find("Wheelcart").GetComponent<WheelcartController>();
        wheelcart.OnWheelcartDestroyed += GameOverScreen;
        wheelcartSpline.Completed += WinScreen;
        wheelcart.OnWheelcartHealthVariation += UpdateWheelcartHealthbar;
    }

    void OnDisable()
    {
        if (playerInputHandler != null) { playerInputHandler.OnPauseTogglePerformed -= TogglePause; }
    }


    void Start()
    {
        gameOver = false;
        SetCursorState(gamePaused);
        Debug.Log("Game Started");
    }

    public void WinScreen()
    {
        winConditionReached = true;

        Time.timeScale = 0f;

        SetCursorState(true);
        OnWinScreen?.Invoke();

        Debug.Log("Fin de la partida, ganaste");    // Ac� se mostrar�a la UI del fin del nivel
    }

    public void GameOverScreen()
    {
        gameOver = true;

        Time.timeScale = 0f;

        SetCursorState(true);
        OnLoseScreen?.Invoke();

        Debug.Log("Fin de la partida, perdiste");   // Y ac� la de volver al men� o reempezar?
    }

    private void TogglePause()
    {
        gamePaused = !gamePaused;

        if (gamePaused)
        {
            Time.timeScale = 0f;
            Debug.Log("Juego pausado"); // Ac� se mostrar�a el men� de pausa
        }
        else
        {
            Time.timeScale = 1f;
            Debug.Log("Juego resumido"); // Y ac� se cerrar�a
        }

        SetCursorState(gamePaused);
        OnGamePaused?.Invoke(gamePaused);
    }

    public void SetPauseGame(bool value)
    {
        if (gamePaused ^ value) {TogglePause();}
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        SetCursorState(false);
        SceneManager.LoadScene("MainLevel",LoadSceneMode.Single);
        StartCoroutine("LoadListeners");
        SetPauseGame(false);
    }

    IEnumerator LoadListeners()
    {
        yield return new WaitForSeconds(1f);
        _suscribeToPlayerInputHandler();
        _suscribeToWheelcart();
        _suscribeToPlayerController();

    }

    public void SetCursorState(bool value)
    {
        if (value)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
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