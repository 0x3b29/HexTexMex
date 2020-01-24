using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private GamePhase gamePhase;
    private List<Player> players;
    private Player currentPlayer;
    private bool firstHalfOfBuildPhase;

    private int buildPhaseStoneAdd = 4;
    private int buildPhaseWoodAdd = 3;
    private int buildPhaseWheatAdd = 1;
    private CameraController cameraController;

    public void Awake()
    {
        players = new List<Player>();
        gamePhase = GamePhase.BuildPhase;
        firstHalfOfBuildPhase = true;
    }

    public void AddPlayer(Player player)
    {
        if (!cameraController)
        {
            cameraController = GameManager.Instance.cameraController;
        }

        player.SaveCamera(cameraController.GetCameraContainerPosition(), 
            cameraController.GetCameraContainerRotation(), 
            cameraController.GetZoomLevel(), 
            cameraController.GetCameraRotation());
        players.Add(player);

        if (currentPlayer == null)
        {
            currentPlayer = player;
            currentPlayer.AddStone(buildPhaseStoneAdd);
            currentPlayer.AddWood(buildPhaseWoodAdd);
            currentPlayer.AddWheat(buildPhaseWheatAdd);
        }
    }

    private void GivePlayerBuildPhaseResources()
    {
        currentPlayer.AddStone(buildPhaseStoneAdd);
        currentPlayer.AddWood(buildPhaseWoodAdd);
        currentPlayer.AddWheat(buildPhaseWheatAdd);
    }

    // Function referenced in UI
    public void EndTurn()
    {
        // Store camera position and rotation of the player finishing his turn
        currentPlayer.SaveCamera(cameraController.GetCameraContainerPosition(),
            cameraController.GetCameraContainerRotation(),
            cameraController.GetZoomLevel(),
            cameraController.GetCameraRotation());

        if (gamePhase.Equals(GamePhase.BuildPhase))
        {
            // During buildphase, player get resources for one building and one street
            if (firstHalfOfBuildPhase)
            {
                // In the first half of the buildphase, process players from 0 to x
                if (currentPlayer == players.Last())
                {
                    // If the current player was the last player in list, the second halve of the buildphase starts
                    firstHalfOfBuildPhase = false;

                    // Go backwards in players list
                    currentPlayer = players.ToArray()[players.IndexOf(currentPlayer) - 1];
                    GivePlayerBuildPhaseResources();
                }
                else
                {
                    // Go forward in players list
                    currentPlayer = players.ToArray()[players.IndexOf(currentPlayer) + 1];
                    GivePlayerBuildPhaseResources();

                    // Last player gets double resources
                    if (currentPlayer == players.Last())
                    {
                        GivePlayerBuildPhaseResources();
                    }
                }
            }
            else
            {
                // In the second half of the buildphase, process players from x to 0
                if (currentPlayer == players.First())
                {
                    // When the current player was the first player in list, the buildphase is over
                    gamePhase = GamePhase.PlayPhase;
                    // Assign last player in list to start next phase with first player 
                    currentPlayer = players.Last();
                }
                else
                {
                    // Go backwards in players list
                    currentPlayer = players.ToArray()[players.IndexOf(currentPlayer) - 1];
                    GivePlayerBuildPhaseResources();
                }
            }
        }

        if (gamePhase.Equals(GamePhase.PlayPhase))
        {
            // Hand over the game to the next player
            int index = players.IndexOf(currentPlayer);
            index += 1;

            // If current player is the last player, reset player to first player
            if (index >= players.Count)
            {
                index = 0;
            }

            currentPlayer = players.ToArray()[index];
            
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

        currentPlayer.walkAllTraders();

        // Update UI
        GameManager.Instance.uiManager.UpdateCurrentPlayer(currentPlayer);
        GameManager.Instance.uiManager.UpdateResources(
            currentPlayer.GetStone(), 
            currentPlayer.GetWood(), 
            currentPlayer.GetWheat(),
            currentPlayer.GetCoins());
        GameManager.Instance.uiManager.SetActionType(currentPlayer.GetSelectedActionType());

        // Reset Camera
        Tuple<Vector3, Quaternion, int, Quaternion> cameraPositionRotationAndZoom = currentPlayer.RetrieveCameraPositionRotationAndZoom();

        cameraController.SetCameraContainerPosition(cameraPositionRotationAndZoom.Item1);
        cameraController.SetCameraContainerRotation(cameraPositionRotationAndZoom.Item2);
        cameraController.SetZoomLevel(cameraPositionRotationAndZoom.Item3);
        cameraController.SetCameraRotation(cameraPositionRotationAndZoom.Item4);
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
