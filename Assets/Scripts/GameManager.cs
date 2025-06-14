using Assets.Scripts.Interfaces;
using FishNet;
using FishNet.Component.Spawning;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using Multiplayer;
using Multiplayer.Utils;
using System;
using System.Collections.Generic;
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
    private WheelcartMovement wheelcartMovement;
    private InputHandler playerInputHandler;
    public List<GameObject> treePrefab;
    

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

    public void GenerateForest()
    {
        try
        {
            int i = 0;
            Terrain terrain = Terrain.activeTerrain;
            int grassIndex = 0;
            float minDistanceTrees = 3f;
            GameObject treeContainer = GameObject.Find("TreeContainer");
            while (i < 3800)
            {
                Vector3 randomPos = GetRandomPositionOnTerrain(terrain);
                bool isOnGrass = IsOnGrass(randomPos, terrain, grassIndex);
                bool isFarEnough = IsPositionFarEnough(randomPos, minDistanceTrees);
                if (isOnGrass && isFarEnough)
                {
                    int treeCount = treePrefab.Count - 1;
                    int randomTreeIndex = UnityEngine.Random.Range(0, treeCount);
                    SpawnTree(randomTreeIndex, randomPos, terrain.transform);
                    i++;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"GenerateForest error {ex.Message}");
        }
    }

    public void SpawnTree(int randomTreeIndex, Vector3 randomPos, Transform transform)
    {
        var tree = Instantiate(treePrefab[randomTreeIndex], randomPos, Quaternion.identity, transform);
        ServerManager.Spawn(tree);
    }

    Vector3 GetRandomPositionOnTerrain(Terrain terrain)
    {

        Vector3 terrainSize = terrain.terrainData.size;

        float randomX = UnityEngine.Random.Range(0, terrainSize.x);
        float randomZ = UnityEngine.Random.Range(0, terrainSize.z);

        float height = terrain.SampleHeight(new Vector3(randomX, 0, randomZ));

        return new Vector3(randomX, height, randomZ) + terrain.transform.position;
    }

    bool IsOnGrass(Vector3 worldPosition, Terrain terrain, int grassTextureIndex)
    {

        Vector3 terrainLocalPos = worldPosition - terrain.transform.position;

        Vector2 normalizedPos = new Vector2(
            terrainLocalPos.x / terrain.terrainData.size.x,
            terrainLocalPos.z / terrain.terrainData.size.z
        );


        int alphaX = (int)(normalizedPos.x * terrain.terrainData.alphamapWidth);
        int alphaY = (int)(normalizedPos.y * terrain.terrainData.alphamapHeight);

        float[,,] alphaMap = terrain.terrainData.GetAlphamaps(alphaX, alphaY, 1, 1);


        return alphaMap[0, 0, grassTextureIndex] > 0.5f;
    }

    bool IsPositionFarEnough(Vector3 position, float minDistance)
    {
        Vector3 halfExtents = new Vector3(minDistance / 2f, minDistance / 2f, minDistance / 2f);


        Collider[] nearbyColliders = Physics.OverlapBox(
            position,
            halfExtents,
            Quaternion.identity,
            LayerMask.GetMask("TerrainElements") 
        );

        return nearbyColliders.Length == 0;
    }

    private void _subscribeToPlayerController(IHealthVariation playerHealthEvents)
    {
        if (playerHealthEvents == null) return;
        HUD.SetPlayerHealthEvent(playerHealthEvents);
        playerHealthEvents.OnDie += GameOverScreen;
    }

    private void _subscribeToPlayerInputHandler(InputHandler playerInputHandler)
    {
        if (playerInputHandler == null) return;
        playerInputHandler.OnPauseTogglePerformed += TogglePause;
    }

    private void _subscribeToWheelcart()
    {
        try
        {
            GameObject wheelcart = GameObject.Find("WheelcartMultiplayer");
            wheelcartMovement = wheelcart.GetComponent<WheelcartMovement>();
            var wheelcartDurationEvent = wheelcart.GetComponent<IWheelcartDuration>();
            var wheelcartEvents = wheelcart.GetComponent<IHealthVariation>();
            wheelcartEvents.OnDie += GameOverScreen;
            wheelcartMovement.Completed += WinScreen;
            HUD.SetWheelcartHealthEvent(wheelcartEvents);
            HUD.SetWheelcartDuration(wheelcartDurationEvent);
        }
        catch (Exception ex)
        {
            Debug.Log($"_subscribeToWheelcart error {ex.Message}");
        }
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
        if (wheelcartMovement != null)
        {
            wheelcartMovement.Completed -= WinScreen;
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
            InstanceFinder.SceneManager.OnLoadEnd -= InitializeMatch;
            GenerateForest();

            PlayerPresenter.OnPlayerSpawned += (PlayerPresenter) =>
            {
                _subscribeToPlayerController(PlayerPresenter.gameObject.GetComponent<IHealthVariation>());
                _subscribeToPlayerInputHandler(PlayerPresenter.gameObject.GetComponent<InputHandler>());
            };

            var hudObj = GameObject.Find("HUD");
            HUD = hudObj.GetComponent<HUD>();
            _subscribeToWheelcart();
            var audios = GetComponents<AudioSource>();
            
            foreach (var audio in audios)
            {
                audio.Play();
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"InitializeMatch failed: {ex.Message}");
        }
    }
}
