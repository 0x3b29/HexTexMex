using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    public CameraController cameraController;
    public TurnManager turnManager;
    public SpawnTiles spawnTiles;
    public InputHandler inputHandler;
    public UIManager uiManager;
    public BuildingManager buildingManager;

    // Start is called before the first frame update
    void Start()
    {
        // Fetch Scripts
        inputHandler = GetComponent<InputHandler>();
        turnManager = GetComponent<TurnManager>();
        spawnTiles = GetComponent<SpawnTiles>();
        uiManager = GetComponent<UIManager>();
        buildingManager = GetComponent<BuildingManager>();
        cameraController = GetComponent<CameraController>();
        
        // Create Players
        turnManager.AddPlayer(new Player("Olivier", Color.red));
        turnManager.AddPlayer(new Player("Jérôme", Color.blue));
        turnManager.AddPlayer(new Player("Gérard", Color.yellow));

        // Create Map to play on
        int seed = Random.Range(0, 1000);
        Debug.Log("Map Seed: " + seed);
        spawnTiles.CreateMap(seed, false, false);

        GameManager.Instance.uiManager.UpdateCurrentPlayer(turnManager.GetCurrentPlayer());
        GameManager.Instance.uiManager.UpdateResources(
            turnManager.GetCurrentPlayer().GetStone(), 
            turnManager.GetCurrentPlayer().GetWood(), 
            turnManager.GetCurrentPlayer().GetWheat(),
            turnManager.GetCurrentPlayer().GetCoins());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
