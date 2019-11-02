using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    private List<Player> players;
    private Player currentPlayer;

    // UI references
    private Text playername;
    private Text stoneCount;
    private Text woodCount;
    private Text wheatCount;

    public void Start()
    {
        playername = GameObject.Find("Text Playername").GetComponent<Text>();
        stoneCount = GameObject.Find("Text Stone").GetComponent<Text>();
        woodCount = GameObject.Find("Text Wood").GetComponent<Text>();
        wheatCount = GameObject.Find("Text Wheat").GetComponent<Text>();
    }

    public TurnManager()
    {
        players = new List<Player>();
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
        currentPlayer = player;
    }

    public void EndTurn()
    {
        // Function gets called by player through a button
        // Mainly hands over the game to the next player

        int index = players.IndexOf(currentPlayer);
        index += 1;

        if (index >= players.Count)
        {
            index = 0;
        }

        currentPlayer = players.ToArray()[index];

        playername.text = currentPlayer.GetName();
        stoneCount.text = currentPlayer.GetStone().ToString();
        woodCount.text = currentPlayer.GetWood().ToString();
        wheatCount.text = currentPlayer.GetWheat().ToString();
        

        Debug.Log("Player " + currentPlayer.GetName() + " can now play");
    }

    public Color GetCurrentPlayerColor()
    {
        return currentPlayer.GetColor();
    }
}
