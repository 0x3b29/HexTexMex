using System.Collections;
using System.Collections.Generic;
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

        // Create Players
        turnManager.AddPlayer(new Player("Jérôme", Color.blue, 10, 10, 10));
        turnManager.AddPlayer(new Player("Olivier", Color.red, 10, 10, 10));
        turnManager.AddPlayer(new Player("Gérard", Color.yellow, 10, 10, 10));
        turnManager.EndTurn();

        // Create Map to play on
        spawnTiles.CreateMap(42, false, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
