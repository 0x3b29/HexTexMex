using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    private ConstructionType constructionType;
    private string name;
    private int stoneCost;
    private int woodCost;
    private int wheatCost;

    public Building (ConstructionType constructionType, string name, int stoneCost, int woodCost, int wheatCost)
    {
        this.constructionType = constructionType;
        this.name = name;
        this.stoneCost = stoneCost;
        this.woodCost = woodCost;
        this.wheatCost = wheatCost;
    }

    public ConstructionType GetConstructionType()
    {
        return constructionType;
    }

    public string GetName()
    {
        return this.name;
    }

    public int GetStoneCost()
    {
        return stoneCost;
    }

    public int GetWoodCost()
    {
        return woodCost;
    }

    public int GetWheatCost()
    {
        return wheatCost;
    }
}