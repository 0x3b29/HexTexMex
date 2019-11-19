using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    List<Player> players;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            players = new List<Player>();
        }
        else
        {
            Destroy(gameObject);
        }
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
            if (players.Count == 0)
            {
                players.Add(new Player("Jérôme", Color.red));
                players.Add(new Player("Olivier", Color.blue));
                players.Add(new Player("Gérard", Color.yellow));
            }

            GameManager.Instance.AddPlayers(players);
            GameManager.Instance.StartGame();
        }
    }

    public void ClearPlayers()
    {
        players.Clear();
    }

    public void AddPlayer(string name, Color color)
    {
        players.Add(new Player(name, color));
    }
}
