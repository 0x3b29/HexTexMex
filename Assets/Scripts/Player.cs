using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Player
{
    private string name;
    private Color color;
    private int wood;
    private int stone;
    private int wheat;
    private int coins;
    private int remainingDragonAttacks;

    private CameraSetup cameraSetup;
    private ActionType selectedActionType;
    
    public List<TileManager> tilesWithHouses;
    private List<TileManager> tilesWithRoads;
    private List<TraderBehaviour> traders;

    public Player (string name, Color color)
    {
        tilesWithHouses = new List<TileManager>();
        tilesWithRoads = new List<TileManager>();
        traders = new List<TraderBehaviour>();

        this.name = name;
        this.color = color;
        this.wood = 0;
        this.stone = 0;
        this.wheat = 0;
        this.coins = 0;

        this.remainingDragonAttacks = Constants.DefaultNumberOfDragonAttacks;
        selectedActionType = ActionType.None;
    }

    public Color GetColor()
    {
        return color;
    }

    public string GetName()
    {
        return name;
    }

    public void AddStone(int stoneToAdd)
    {
        stone += stoneToAdd;
    }

    public int GetStone()
    {
        return stone;
    }

    public void AddWood(int woodToAdd)
    {
        wood += woodToAdd;
    }

    public int GetWood()
    {
        return wood;
    }

    public void AddWheat(int wheatToAdd)
    {
        wheat += wheatToAdd;
    }

    public int GetWheat()
    {
        return wheat;
    }

    public int GetCoins()
    {
        return coins;
    }
    
    public List<TileManager> GetListOfTilesWithHouses()
    {
        return tilesWithHouses;
    }

    public List<TileManager> GetListOfTilesWithRoads()
    {
        return tilesWithRoads;
    }

    public void AddTileWithHouse(TileManager tileManager)
    {
        tilesWithHouses.Add(tileManager);
    }

    public void RemoveTileWithHouse(TileManager tileManager)
    {
        tilesWithHouses.Remove(tileManager);
    }

    public void AddTileWithRoad(TileManager tileManager)
    {
        tilesWithRoads.Add(tileManager);
    }

    public void RemoveTileWithRoad(TileManager tileManager)
    {
        tilesWithRoads.Remove(tileManager);
    }

    public void addTrader(TraderBehaviour traderBehaviour)
    {
        traders.Add(traderBehaviour);
    }

    public void removeTrader(TraderBehaviour traderBehaviour)
    {
        traders.Remove(traderBehaviour);
    }

    public void walkAllTraders()
    {
        foreach(TraderBehaviour traderBehaviour in traders)
        {
            traderBehaviour.Walk();
        }
    }
    
    public void AddCoins(int numberOfCoins)
    {
        this.coins += numberOfCoins;
        
        // Check if the player has won the game
        if (this.coins >= Constants.minimumCoinsNeededToWin)
        {
            GameManager.Instance.uiManager.ShowWinnerLabel(this.name);
        }
    }

    public void SetCameraSetup(CameraSetup cameraSetup)
    {
        this.cameraSetup = cameraSetup;
    }
    
    public CameraSetup GetCameraSetup()
    {
        return cameraSetup;
    }

    public int GetDragonAttacks()
    {
        return remainingDragonAttacks;
    }

    public void DecrementRemainingDragonAttacks()
    {
        remainingDragonAttacks -= 1;
    }

    public void SetSelectedActionType(ActionType actionType)
    {
        selectedActionType = actionType;
    }

    public ActionType GetSelectedActionType()
    {
        return selectedActionType;
    }
}
