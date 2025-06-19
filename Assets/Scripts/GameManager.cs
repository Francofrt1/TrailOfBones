using Assets.Scripts.Interfaces;
using FishNet;
using FishNet.Managing.Scened;
using Multiplayer;
using Multiplayer.PlayerSystem;
using Multiplayer.Steam;
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
        Playing,
        End
    }
    public static GameManager Instance { get; private set; }

    private HUDView hudView;
    private WheelcartMovement wheelcartMovement;
    private InputHandler playerInputHandler;
    public List<GameObject> treePrefab;
    private int deadPlayers = 0;

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
            wheelcartEvents.OnDie += GameOverScreen;
            wheelcartMovement.Completed += WinScreen;
            hudView.SetWheelcartHealthEvent(wheelcartEvents);
            hudView.SetWheelcartDuration(wheelcartDurationEvent);
            wheelCart.GetComponent<WheelcartController>().OnWheelcartSpawned();
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
    }

    void Start()
    {
        Debug.Log("Game Started");
    }

    public void WinScreen()
    {
        Time.timeScale = 0f;
        ViewManager.Instance.Show<WinView>();
        Debug.Log("Game Over, you win.");
    }

    public void GameOverScreen()
    {
        Time.timeScale = 0f;
        ViewManager.Instance.Show<LoseView>();
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
        if (wheelcartMovement != null)
        {
            wheelcartMovement.Completed -= WinScreen;
        }

        PlayerPresenter.OnPlayerSpawned -= HandlePlayerSpawned;
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
                // Handle loading state
                break;
            case GameState.Playing:
                StartMatch();
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
            GenerateForest();
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
            GameOverScreen();
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
        _subscribeToWheelcart();
    }
}
