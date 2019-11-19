using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public void StartGame()
    {
        // Create Map to play on
        spawnTiles.CreateMap(MenuManager.Instance.Seed, MenuManager.Instance.RoundMap, MenuManager.Instance.Mountains);

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

    public void AddPlayers(List<Player> players)
    {
        foreach (Player player in players)
        {
            turnManager.AddPlayer(player);
        }
    }
}
