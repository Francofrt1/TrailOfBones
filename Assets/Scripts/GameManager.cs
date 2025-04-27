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

    void Start()
    {
        gameOver = false;
        Debug.Log("Game Started");

    }

    public void WinScreen()
    {
        winConditionReached = true;

        Time.timeScale = 0f;

        Debug.Log("Fin de la partida, ganaste");    // Ac� se mostrar�a la UI del fin del nivel
    }

    public void GameOverScreen()
    {
        gameOver = true;

        Time.timeScale = 0f;

        Debug.Log("Fin de la partida, perdiste");   // Y ac� la de volver al men� o reempezar?
    }



    public void PauseGame()
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
    }

    public void RestartGame()   // No funciona jaja
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}