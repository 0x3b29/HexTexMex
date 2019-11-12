using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    List<Action> actions;
    ActionType currentAction;
    TurnManager turnManager;

    // Start is called before the first frame update
    void Awake()
    {
        actions = new List<Action>();
        actions.Add(new Action(ActionType.House, "Woodhouse", 3, 2, 1));
        actions.Add(new Action(ActionType.Road, "Road", 1, 1, 0));
        actions.Add(new Action(ActionType.Trader, "Trader", 0, 0, 10));
        actions.Add(new Action(ActionType.Destroy, "Destroy", 0, 0, 0));
        actions.Add(new Action(ActionType.Dragon, "Dragon", 1));
    }

    private void Start()
    {
        turnManager = GameManager.Instance.turnManager;
    }

    public void SetBuildingMode(ActionType buildingMode)
    {
        this.currentAction = buildingMode;
    }

    // Function used to indicate (green / red) if an action can be performed on the currently highlighted tile
    public bool IsCurrentActionAllowedOnTile(Tile tile, Player currentPlayer)
    {
        // Test if a road can be build on this tile
        if (tile.isFree() &&
            (tile.hasNeighbourTileBuilding(currentPlayer) || tile.hasNeighbourTileRoad(currentPlayer)) &&
            currentAction.Equals(ActionType.Road) &&
            IsActionAllowed(currentPlayer, currentAction))
        {
            return true;
        }

        // Test if a house can be build on this tile
        if (turnManager.GetGamePhase().Equals(GamePhase.BuildPhase))
        {
            // During buildphase, houses do not need to be placed next to an existing road
            if (tile.isFree() &&
                !tile.hasNeighbourTileBuilding(currentPlayer) &&
                currentAction.Equals(ActionType.House) &&
                IsActionAllowed(currentPlayer, currentAction))
            {
                return true;
            }
        }
        else if (turnManager.GetGamePhase().Equals(GamePhase.PlayPhase))
        {
            // During playphase, houses must be placed next to an existing road
            if (tile.isFree() &&
                !tile.hasNeighbourTileBuilding(currentPlayer) &&
                tile.hasNeighbourTileRoad(currentPlayer) &&
                currentAction.Equals(ActionType.House) &&
                IsActionAllowed(currentPlayer, currentAction))
            {
                return true;
            }
        }

        if(currentAction.Equals(ActionType.Trader) &&
            tile.woodhouse &&
            tile.owner == currentPlayer &&
            IsActionAllowed(currentPlayer, currentAction))
        {
            return true;
        }

        // Test if the constructed object can be destroyed
        if ((tile.isRoad || tile.woodhouse) && 
            tile.owner == currentPlayer && 
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

    public void PerformAction(Tile tile, Player currentPlayer)
    {
        switch (currentAction)
        {
            case ActionType.House:
                SubstractBuildingCostFromPlayer(currentPlayer, currentAction);
                tile.placeHouse(currentPlayer);
                break;
            case ActionType.Road:
                SubstractBuildingCostFromPlayer(currentPlayer, currentAction);
                tile.placeRoad(currentPlayer);
                break;
            case ActionType.Trader:
                SubstractBuildingCostFromPlayer(currentPlayer, currentAction);

                GameObject trader = Instantiate(Resources.Load(Constants.prefabFolder + "Trader") as GameObject, tile.transform.position, Quaternion.identity);
                TraderBehaviour newTraderBehaviour = trader.AddComponent<TraderBehaviour>();
                    newTraderBehaviour.Initialize(trader, tile, currentPlayer);

                currentPlayer.addTrader(newTraderBehaviour);
                break;
            case ActionType.Dragon:
                tile.DragonAttack();
                currentPlayer.DecrementRemainingDragonAttacks();
                break;
            case ActionType.Destroy:
                tile.DestroyFeature();
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
        Action action = actions.First(x => x.GetConstructionType().Equals(actionType));

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
        Action action = actions.First(x => x.GetConstructionType().Equals(actionType));

        player.AddStone(- action.GetStoneCost());
        player.AddWood(- action.GetWoodCost());
        player.AddWheat(- action.GetWheatCost());
    }
}
