using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    Building woodHouse;
    Building road;

    // Start is called before the first frame update
    void Start()
    {
        woodHouse = new Building("Woodhouse", 3, 2, 1);
        road = new Building("Woodhouse", 3, 2, 1);
        woodHouse = new Building("Woodhouse", 3, 2, 1);
    }

    public Building getWoodhouse()
    {
        return woodHouse;
    }

    public Building getRoad()
    {
        return road;
    }

    public bool HasPlayerEnoughRessourcesToBuild(Player player, Building building)
    {
        if (building.getStoneCost() <= player.GetStone() && 
            building.getWoodCost() <= player.GetWood() && 
            building.getWheatCost() <= player.GetWheat())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void substractBuildingCostFromPlayer(Player player, Building building)
    {
        player.AddStone(- building.getStoneCost());
        player.AddWood(- building.getWoodCost());
        player.AddWheat(- building.getWheatCost());
    }
}
