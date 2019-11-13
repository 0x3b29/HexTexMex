using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;

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

        // Fetch Scripts
        spawnTiles = GetComponent<SpawnTiles>();
        actionManager = GetComponent<ActionManager>();
        turnManager = GetComponent<TurnManager>();
        uiManager = GetComponent<UIManager>();
        inputHandler = GetComponent<InputHandler>();
        cameraController = GetComponent<CameraController>();
    }

    public SpawnTiles spawnTiles;
    public ActionManager actionManager;
    public UIManager uiManager;
    public TurnManager turnManager;
    public InputHandler inputHandler;
    public CameraController cameraController;

    // Start is called before the first frame update
    void Start()
    {
        // Create Players
        turnManager.AddPlayer(new Player("Olivier", Color.red));
        turnManager.AddPlayer(new Player("Jérôme", Color.blue));
        //turnManager.AddPlayer(new Player("Gérard", Color.yellow));

        // Create Map to play on
        int seed = Random.Range(0, 1000);
        Debug.Log("Map Seed: " + seed);
        spawnTiles.CreateMap(seed, false, false);

        actionManager.Initialize();
        uiManager.Initialize();
        inputHandler.Initialize();

        uiManager.UpdateCurrentPlayer(turnManager.GetCurrentPlayer());
        uiManager.UpdateResources(
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
