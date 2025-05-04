using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool gameOver = false;
    public bool gamePaused = false;
    public bool winConditionReached = false;

    public event Action<bool> OnGamePaused;
    public event Action OnWinScreen;
    public event Action OnLooseScreen;


    private InputHandler playerInputHandler;

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

    void OnEnable()
    {
        _suscribeToPlayerInputHandler();
    }

    private void _suscribeToPlayerInputHandler()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null) return;
        playerInputHandler = player.GetComponent<InputHandler>();
        if (playerInputHandler == null) return;
        playerInputHandler.OnPauseTogglePerformed += TogglePause;
    }

    void OnDisable()
    {
        if (playerInputHandler != null) { playerInputHandler.OnPauseTogglePerformed -= TogglePause; }
    }


    void Start()
    {
        gameOver = false;
        SetCursorState(false);
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
        OnLooseScreen?.Invoke();

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

    public void UnpauseGame()
    {
        if (gamePaused) {TogglePause();}
    }

    public void RestartGame()   // No funciona jaja
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
}