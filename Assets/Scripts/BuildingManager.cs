using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    List<Building> buildings;

    ConstructionType currentBuildingMode;

    // Start is called before the first frame update
    void Awake()
    {
        buildings = new List<Building>();
        buildings.Add(new Building(ConstructionType.House, "Woodhouse", 3, 2, 1));
        buildings.Add(new Building(ConstructionType.Road, "Road", 1, 1, 0));
    }

    public void SetBuildingMode(ConstructionType buildingMode)
    {
        this.currentBuildingMode = buildingMode;
    }

    public bool IsActionAllowed(Tile tile, Player currentPlayer)
    {
        // Test if a road can be build on this tile
        if (tile.isFree() && currentBuildingMode.Equals(ConstructionType.Road) && HasPlayerEnoughRessourcesToBuild(currentPlayer, currentBuildingMode))
        {
            return true;
        }

        // Test if a house can be build on this tile
        if (tile.isFree() && currentBuildingMode.Equals(ConstructionType.House) && HasPlayerEnoughRessourcesToBuild(currentPlayer, currentBuildingMode))
        {
            return true;
        }

        // Test if the constructed object can be destroyed
        if ((tile.isRoad || tile.woodhouse) && tile.owner == currentPlayer && currentBuildingMode.Equals(ConstructionType.Destroy))
        {
            return true;
        }

        return false;
    }

    public void PerformAction(Tile tile, Player currentPlayer)
    {
        switch (currentBuildingMode)
        {
            case ConstructionType.House:
                SubstractBuildingCostFromPlayer(currentPlayer, currentBuildingMode);
                tile.placeHouse(currentPlayer);
                break;
            case ConstructionType.Road:
                SubstractBuildingCostFromPlayer(currentPlayer, currentBuildingMode);
                tile.placeRoad(currentPlayer);
                break;
            case ConstructionType.Destroy:
                tile.destroyFeature();
                break;
            default:
                Debug.Log("BuildingMode " + currentBuildingMode + " not implemented");
                break;
        }
    }

    public bool HasPlayerEnoughRessourcesToBuild(Player player, ConstructionType constructionType)
    {
        // Destroy and none are free
        if (constructionType.Equals(ConstructionType.Destroy) || constructionType.Equals(ConstructionType.None))
        {
            return true;
        }

        // Get the correct building from the buildings list
        Building building = buildings.First(x => x.GetConstructionType().Equals(constructionType));

        // Check if ressource requirements are met
        if (building.GetStoneCost() <= player.GetStone() && 
            building.GetWoodCost() <= player.GetWood() && 
            building.GetWheatCost() <= player.GetWheat())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SubstractBuildingCostFromPlayer(Player player, ConstructionType constructionType)
    {
        // Get the correct building from the buildings list
        Building building = buildings.First(x => x.GetConstructionType().Equals(constructionType));

        player.AddStone(- building.GetStoneCost());
        player.AddWood(- building.GetWoodCost());
        player.AddWheat(- building.GetWheatCost());
    }
}
