using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public SpawnTiles spawnTiles;
    public ActionManager actionManager;
    public UIManager uiManager;
    public TurnManager turnManager;
    public InputHandler inputHandler;
    public CameraController cameraController;

    public List<Player> Players { get; set; }
    public int Seed { get; set; }
    public int BoardSizeX { get; set; }
    public int BoardSizeY { get; set; }
    public bool RoundMap { get; set; }
    public bool Mountains { get; set; }
    public bool DragonMadness { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Players = new List<Player>();
    }


    // called first
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals("GameScene"))
        {
            if (Players.Count == 0)
            {
                Players.Add(new Player("Jérôme", Color.red));
                Players.Add(new Player("Olivier", Color.blue));
                Players.Add(new Player("Gérard", Color.yellow));
            }

            StartGame();
        }
    }

    public void StartGame()
    {
        // Fetch Scripts
        GameObject gameScripts = GameObject.Find("GameScripts");

        spawnTiles = gameScripts.GetComponent<SpawnTiles>();
        actionManager = gameScripts.GetComponent<ActionManager>();
        turnManager = gameScripts.GetComponent<TurnManager>();
        uiManager = gameScripts.GetComponent<UIManager>();
        inputHandler = gameScripts.GetComponent<InputHandler>();
        cameraController = gameScripts.GetComponent<CameraController>();

        // Create Map to play on
        spawnTiles.CreateMap(Seed, RoundMap, Mountains);

        // Initialize scripts in a certain order
        actionManager.Initialize();
        uiManager.Initialize();
        inputHandler.Initialize();
        turnManager.Initialize();

        foreach (Player player in Players)
        {
            turnManager.AddPlayer(player);
        }

        uiManager.UpdateCurrentPlayer(turnManager.GetCurrentPlayer());
        uiManager.UpdateResources(
            turnManager.GetCurrentPlayer().GetStone(),
            turnManager.GetCurrentPlayer().GetWood(),
            turnManager.GetCurrentPlayer().GetWheat(),
            turnManager.GetCurrentPlayer().GetCoins());
    }

    public void AddPlayers(List<Player> players)
    {
        foreach (Player player in players)
        {
            turnManager.AddPlayer(player);
        }
    }

    public Tile GetRandomTile()
    {
        Tile randomTile = null;

        while (randomTile == null || !randomTile.isActive)
        {
            randomTile = GameObject.Find("Tile" + Random.Range(0, BoardSizeX) + "-" + Random.Range(0, BoardSizeX)).GetComponent<Tile>();
        }

        return randomTile;
    }
}
