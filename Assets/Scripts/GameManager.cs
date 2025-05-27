using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
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

    private HUD HUD;
    private SplineAnimate wheelcartSpline;
    private InputHandler playerInputHandler;
    public List<GameObject> treePrefab;
    

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
        GenerateForest();
    }

    private void GenerateForest()
    {
        int i = 0;
        Terrain terrain = Terrain.activeTerrain;
        int grassIndex = 0;
        float minDistanceTrees = 3f;

        while (i < 3800)
        {
            Vector3 randomPos = GetRandomPositionOnTerrain(terrain);
            if (IsOnGrass(randomPos, terrain, grassIndex) && IsPositionFarEnough(randomPos, minDistanceTrees))
            {
                int randomTree = UnityEngine.Random.Range(0, treePrefab.Count);
                Instantiate(treePrefab[randomTree], randomPos, Quaternion.identity);
                i++;
            }
        }
    }

    Vector3 GetRandomPositionOnTerrain(Terrain terrain)
    {
        // Obtiene las dimensiones del terreno
        Vector3 terrainSize = terrain.terrainData.size;

        // Genera coordenadas X y Z aleatorias dentro del terreno
        float randomX = UnityEngine.Random.Range(0, terrainSize.x);
        float randomZ = UnityEngine.Random.Range(0, terrainSize.z);

        // Calcula la altura (Y) en ese punto del terreno
        float height = terrain.SampleHeight(new Vector3(randomX, 0, randomZ));

        // Retorna la posición en coordenadas mundiales (ajustando al centro del terreno)
        return new Vector3(randomX, height, randomZ) + terrain.transform.position;
    }

    bool IsOnGrass(Vector3 worldPosition, Terrain terrain, int grassTextureIndex)
    {
        // Convierte la posición mundial a coordenadas locales del terreno
        Vector3 terrainLocalPos = worldPosition - terrain.transform.position;

        // Normaliza las coordenadas (0-1)
        Vector2 normalizedPos = new Vector2(
            terrainLocalPos.x / terrain.terrainData.size.x,
            terrainLocalPos.z / terrain.terrainData.size.z
        );

        // Obtén el alphamap en esa posición
        int alphaX = (int)(normalizedPos.x * terrain.terrainData.alphamapWidth);
        int alphaY = (int)(normalizedPos.y * terrain.terrainData.alphamapHeight);

        float[,,] alphaMap = terrain.terrainData.GetAlphamaps(alphaX, alphaY, 1, 1);

        // Si el valor de la textura de pasto es mayor a un umbral (ej. 0.5), está en pasto
        return alphaMap[0, 0, grassTextureIndex] > 0.5f;
    }

    bool IsPositionFarEnough(Vector3 position, float minDistance)
    {
        Vector3 halfExtents = new Vector3(minDistance / 2f, minDistance / 2f, minDistance / 2f);

        // Usa OverlapBox para detectar colliders en un área cúbica
        Collider[] nearbyColliders = Physics.OverlapBox(
            position,          // Centro del Box
            halfExtents,       // Mitad del tamaño del Box
            Quaternion.identity, // Rotación (ninguna en este caso)
            LayerMask.GetMask("TerrainElements") // Layer a filtrar
        );

        return nearbyColliders.Length == 0;
    }

    private void _subscribeToPlayerController()
    {
        IHealthVariation playerHealthEvents = GameObject.Find("Player").GetComponent<IHealthVariation>();
        if (playerHealthEvents == null) return;
        HUD.SetPlayerHealthEvent(playerHealthEvents);
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
        var wheelcartDurationEvent = GameObject.Find("Wheelcart").GetComponent<IWheelcartDuration>();
        var wheelcartEvents = GameObject.Find("Wheelcart").GetComponent<IHealthVariation>();
        wheelcartEvents.OnDie += GameOverScreen;
        wheelcartSpline.Completed += WinScreen;
        HUD.SetWheelcartHealthEvent(wheelcartEvents);
        HUD.SetWheelcartDuration(wheelcartDurationEvent);
    }

    private void OnEnable()
    {
        HUD = GameObject.Find("HUD").GetComponent<HUD>();
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
}
