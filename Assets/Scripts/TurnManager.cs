using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    private List<Player> players;
    private Player currentPlayer;

    public TurnManager()
    {
        players = new List<Player>();
    }

    public void AddPlayer(Player player)
    {
        if (currentPlayer == null)
        {
            currentPlayer = player;
        }

        players.Add(player);
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
        Debug.Log("Player " + currentPlayer.GetName() + " can now play");
    }

    public Color GetCurrentPlayerColor()
    {
        return currentPlayer.GetColor();
    }
}
