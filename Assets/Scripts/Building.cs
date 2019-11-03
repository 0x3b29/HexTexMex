using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    private string name;
    private int stoneCost;
    private int woodCost;
    private int wheatCost;

    public Building (string name, int stoneCost, int woodCost, int wheatCost)
    {
        this.name = name;
        this.stoneCost = stoneCost;
        this.woodCost = woodCost;
        this.wheatCost = wheatCost;
    }

    public int getStoneCost()
    {
        return stoneCost;
    }

    public int getWoodCost()
    {
        return woodCost;
    }

    public int getWheatCost()
    {
        return wheatCost;
    }
}