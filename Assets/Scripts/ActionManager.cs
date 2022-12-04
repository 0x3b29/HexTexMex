using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    List<Action> actions;
    ActionType currentAction;
    TurnManager turnManager;

    private List<ActionType> actionTypes;

    public void Initialize()
    {
        actions = new List<Action>
        {
            new Action(ActionType.House, "Woodhouse", "Button House", 3, 2, 1),
            new Action(ActionType.Road, "Road", "Button Road", 1, 1, 0),
            new Action(ActionType.Trader, "Trader", "Button Trader", 0, 0, 10),
            new Action(ActionType.Destroy, "Destroy", "Button Destroy", 0, 0, 0),
            new Action(ActionType.Dragon, "Dragon", "Button Dragon", 1)
        };

        actionTypes = new List<ActionType>
        {
            ActionType.House,
            ActionType.Road,
            ActionType.Trader,
            ActionType.Destroy,
            ActionType.Dragon
        };
    }

    private void Start()
    {
        turnManager = GameManager.Instance.turnManager;
    }

    public void SetSelectedAction(ActionType buildingMode)
    {
        this.currentAction = buildingMode;
    }

    // Function used to indicate (green / red) if an action can be performed on the currently highlighted tile
    public bool IsCurrentActionAllowedOnTile(TileManager tileManager, Player currentPlayer)
    {
        // Test if a road can be build on this tile
        if (tileManager.IsFree() &&
            (tileManager.HasNeighbouringTileManagerBuilding(currentPlayer) || tileManager.HasNeighbouringTileManagerRoad(currentPlayer)) &&
            currentAction.Equals(ActionType.Road) &&
            IsActionAllowed(currentPlayer, currentAction))
        {
            return true;
        }

        // Test if a house can be build on this tile
        if (turnManager.GetGamePhase().Equals(GamePhase.BuildPhase))
        {
            // During buildphase, houses do not need to be placed next to an existing road
            if (tileManager.IsFree() &&
                !tileManager.HasNeighbouringTileManagerBuilding(currentPlayer) &&
                currentAction.Equals(ActionType.House) &&
                IsActionAllowed(currentPlayer, currentAction))
            {
                return true;
            }
        }
        else if (turnManager.GetGamePhase().Equals(GamePhase.PlayPhase))
        {
            // During playphase, houses must be placed next to an existing road
            if (tileManager.IsFree() &&
                !tileManager.HasNeighbouringTileManagerBuilding(currentPlayer) &&
                tileManager.HasNeighbouringTileManagerRoad(currentPlayer) &&
                currentAction.Equals(ActionType.House) &&
                IsActionAllowed(currentPlayer, currentAction))
            {
                return true;
            }
        }

        if(currentAction.Equals(ActionType.Trader) &&
            tileManager.woodhouse &&
            tileManager.owner == currentPlayer &&
            IsActionAllowed(currentPlayer, currentAction))
        {
            return true;
        }

        // Test if the constructed object can be destroyed
        if ((tileManager.hasRoad || tileManager.woodhouse) && 
            tileManager.owner == currentPlayer && 
            currentAction.Equals(ActionType.Destroy) &&
            IsActionAllowed(currentPlayer, currentAction))
        {
            return true;
        }

        // The dragon can always be sent to any tile
        if (currentAction.Equals(ActionType.Dragon) &&
            IsActionAllowed(currentPlayer, currentAction))
        {
            return true;
        }
        
        return false;
    }

    public void PerformAction(TileManager tileManager, Player currentPlayer)
    {
        switch (currentAction)
        {
            case ActionType.House:
                SubstractBuildingCostFromPlayer(currentPlayer, currentAction);
                tileManager.PlaceHouse(currentPlayer);
                break;
            case ActionType.Road:
                SubstractBuildingCostFromPlayer(currentPlayer, currentAction);
                tileManager.PlaceRoad(currentPlayer);
                break;
            case ActionType.Trader:
                SubstractBuildingCostFromPlayer(currentPlayer, currentAction);

                GameObject trader = Instantiate(Resources.Load(Constants.prefabFolder + "Trader") as GameObject, tileManager.transform.position, Quaternion.identity);
                TraderBehaviour newTraderBehaviour = trader.AddComponent<TraderBehaviour>();
                newTraderBehaviour.Initialize(trader, tileManager, currentPlayer);

                currentPlayer.addTrader(newTraderBehaviour);
                break;
            case ActionType.Dragon:

                // Get random tile from border
                TileManager edgeTile = GameManager.Instance.spawnTiles.edgeTileManagers[Random.Range(0, GameManager.Instance.spawnTiles.edgeTileManagers.Count)];

                // Get position of boarder tile and set heigt for dragon to spawn
                Vector3 dragonSpawnPosition = edgeTile.transform.position;
                dragonSpawnPosition.y = 5;
                
                // Spawn dragon over this border tile
                GameObject dragon = Instantiate(Resources.Load(Constants.prefabFolder + "Dragon") as GameObject, dragonSpawnPosition, Quaternion.identity);
                DragonBehaviour dragonBehaviour = dragon.AddComponent<DragonBehaviour>();
                dragonBehaviour.Initialize(dragon, tileManager);

                // In case of DragonMadness gamemode, the game can launch attacks as well
                if (!(currentPlayer == null))
                    currentPlayer.DecrementRemainingDragonAttacks();

                break;
            case ActionType.Destroy:
                tileManager.DestroyFeature();
                break;
            default:
                Debug.Log("BuildingMode " + currentAction + " not implemented");
                break;
        }
    }

    public bool IsActionAllowed(Player player, ActionType actionType)
    {
        // Destroy and none are free
        if (actionType.Equals(ActionType.Destroy) || actionType.Equals(ActionType.None))
        {
            return true;
        }

        // Get the correct building from the buildings list
        Action action = actions.First(actionListItem => actionListItem.GetActionType().Equals(actionType));

        // Check if resource requirements are met
        if (action.GetStoneCost() <= player.GetStone() && 
            action.GetWoodCost() <= player.GetWood() && 
            action.GetWheatCost() <= player.GetWheat() &&
            action.GetDragonCost() <= player.GetDragonAttacks())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SubstractBuildingCostFromPlayer(Player player, ActionType actionType)
    {
        // Get the correct building from the buildings list
        Action action = actions.First(actionListItem => actionListItem.GetActionType().Equals(actionType));

        player.AddStone(- action.GetStoneCost());
        player.AddWood(- action.GetWoodCost());
        player.AddWheat(- action.GetWheatCost());
    }

    public List<Action> GetActions()
    {
        return actions;
    }

    public List<ActionType> GetActionTypes()
    {
        return actionTypes;
    }
}
