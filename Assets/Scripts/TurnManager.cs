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

    private int turnCount = 0;

    private CameraController cameraController;
    private ActionManager actionManager;

    public void Awake()
    {
        players = new List<Player>();
        gamePhase = GamePhase.BuildPhase;
        firstHalfOfBuildPhase = true;
    }

    public void Initialize()
    {
        cameraController = GameManager.Instance.cameraController;
        actionManager = GameManager.Instance.actionManager;
    }

    public void AddPlayer(Player player)
    {
        if (!cameraController)
        {
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

                    // Go backwards in players list (If there is more than one player)
                    if (!currentPlayer.Equals(players[0]))
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
                turnCount += 1;
            }

            // Launch dragon attacks
            if (GameManager.Instance.DragonMadness)
            {
                for (int i = 0; i < (turnCount / 2); i++)
                {
                    actionManager.SetSelectedAction(ActionType.Dragon);
                    actionManager.PerformAction(GameManager.Instance.GetRandomTile(), null);
                }
            }

            currentPlayer = players.ToArray()[index];
            
            // Players always have a base income of 1
            int totalStone = 1;
            int totalWood = 1;
            int totalWheat = 1;

            // Collect the resources from the resources on neighbouring house tiles
            foreach (Tile tile in currentPlayer.GetListOfTilesWithHouses())
            {
                totalStone += tile.GetNeighboursStoneCount();
                totalWood += tile.GetNeighboursWoodCount();
                totalWheat += tile.GetNeighboursWheatCount();
            }

            // Add resources to player
            currentPlayer.AddStone(totalStone);
            currentPlayer.AddWood(totalWood);
            currentPlayer.AddWheat(totalWheat);
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
