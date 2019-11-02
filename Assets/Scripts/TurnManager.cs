using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    private List<Player> players;
    private Player currentPlayer;

    public void Awake()
    {
        players = new List<Player>();
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
        currentPlayer = player;
    }

    // Function referenced in UI
    public void EndTurn()
    {
        // Hand over the game to the next player

        int index = players.IndexOf(currentPlayer);
        index += 1;

        if (index >= players.Count)
        {
            index = 0;
        }

        currentPlayer = players.ToArray()[index];

        GameManager.Instance.UIManager.UpdatePlayername(currentPlayer.GetName());
        GameManager.Instance.UIManager.UpdateRessources(currentPlayer.GetStone(), currentPlayer.GetWood(), currentPlayer.GetWheat());
        GameManager.Instance.UIManager.SetButtonColor(currentPlayer.GetColor());
    }

    public Color GetCurrentPlayerColor()
    {
        return currentPlayer.GetColor();
    }
}
