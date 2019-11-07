using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    private GamePhase gamePhase;
    private List<Player> players;
    private Player currentPlayer;
    private bool firstHalfOfBuildPhase;

    public void Awake()
    {
        players = new List<Player>();
        gamePhase = GamePhase.BuildPhase;
        firstHalfOfBuildPhase = true;
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);

        if (currentPlayer == null)
        {
            currentPlayer = player;
        }
    }

    // Function referenced in UI
    public void EndTurn()
    {
        if (gamePhase.Equals(GamePhase.BuildPhase))
        {
            if (firstHalfOfBuildPhase)
            {
                if (players.Last().Equals(currentPlayer))
                {
                    firstHalfOfBuildPhase = false;
                    currentPlayer = players.ToArray()[players.IndexOf(currentPlayer) - 1];
                }
                else
                {
                    currentPlayer = players.ToArray()[players.IndexOf(currentPlayer) + 1];
                }
            }
            else
            {
                if (players.First().Equals(currentPlayer))
                {
                    gamePhase = GamePhase.PlayPhase;
                }
                else
                {
                    currentPlayer = players.ToArray()[players.IndexOf(currentPlayer) - 1];
                }
            }
        }
        else if (gamePhase.Equals(GamePhase.PlayPhase))
        {
            // Hand over the game to the next player
            int index = players.IndexOf(currentPlayer);
            index += 1;

            if (index >= players.Count)
            {
                index = 0;
            }

            // Collect the resources from the resources on neighbouring house tiles
            int totalStone = 0;
            int totalWood = 0;
            int totalWheat = 0;

            foreach (Tile tile in currentPlayer.GetListOfTilesWithHouses())
            {
                totalStone += tile.GetNeighboursStoneCount();
                totalWood += tile.GetNeighboursWoodCount();
                totalWheat += tile.GetNeighboursWheatCount();
            }

            // In case a player does not have any basic neighbouring resources, he receives automatically a base income
            currentPlayer.AddStone(Math.Max(totalStone, 1));
            currentPlayer.AddWood(Math.Max(totalWood, 1));
            currentPlayer.AddWheat(Math.Max(totalWheat, 1));

        }

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
