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

    // Start is called before the first frame update
    void Start()
    {
        // Create Players
        turnManager = GetComponent<TurnManager>();
        turnManager.AddPlayer(new Player("Jérôme", Color.red, 10, 10, 10));
        turnManager.AddPlayer(new Player("Olivier", Color.blue, 10, 10, 10));
        turnManager.AddPlayer(new Player("Gérard", Color.green, 10, 10, 10));
        turnManager.EndTurn();

        // Create Map to play on
        spawnTiles = GetComponent<SpawnTiles>();
        spawnTiles.CreateMap(42, false, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
