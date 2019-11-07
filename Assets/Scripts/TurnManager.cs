using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    private GamePhase gamePhase;
    private List<Player> players;
    private Player currentPlayer;

    public void Awake()
    {
        players = new List<Player>();
        gamePhase = GamePhase.BuildPhase;
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

        // Collect the ressources from the ressources on neighbouring house tiles
        int totalStone = 0;
        int totalWood = 0;
        int totalWheat = 0;

        foreach (Tile tile in currentPlayer.GetListOfTilesWithHouses())
        {
            totalStone += tile.GetNeighboursStoneCount();
            totalWood += tile.GetNeighboursWoodCount();
            totalWheat += tile.GetNeighboursWheatCount();
        }

        currentPlayer.AddStone(totalStone);
        currentPlayer.AddWood(totalWood);
        currentPlayer.AddWheat(totalWheat);

        // Update UI
        GameManager.Instance.uiManager.UpdateCurrentPlayer(currentPlayer);
        GameManager.Instance.uiManager.UpdateRessources(currentPlayer.GetStone(), currentPlayer.GetWood(), currentPlayer.GetWheat());
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public GamePhase GetGamePhase()
    {
        return gamePhase;
    }
}
