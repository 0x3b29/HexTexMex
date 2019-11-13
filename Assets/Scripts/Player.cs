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

    private Vector3 cameraContainerPosition;
    private Quaternion cameraContainerRotation;
    private int zoomLevel;
    private Quaternion cameraRotation;
    private ActionType selectedActionType;
    
    public List<Tile> tilesWithHouses;
    private List<Tile> tilesWithRoads;
    private List<TraderBehaviour> traders;

    public Player (string name, Color color)
    {
        tilesWithHouses = new List<Tile>();
        tilesWithRoads = new List<Tile>();
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
    
    public List<Tile> GetListOfTilesWithHouses()
    {
        return tilesWithHouses;
    }

    public List<Tile> GetListOfTilesWithRoads()
    {
        return tilesWithRoads;
    }

    public void AddTileWithHouse(Tile tile)
    {
        tilesWithHouses.Add(tile);
    }

    public void RemoveTileWithHouse(Tile tile)
    {
        tilesWithHouses.Remove(tile);
    }

    public void AddTileWithRoad(Tile tile)
    {
        tilesWithRoads.Add(tile);
    }

    public void RemoveTileWithRoad(Tile tile)
    {
        tilesWithRoads.Remove(tile);
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

    public void SaveCamera(Vector3 cameraContainerPosition, Quaternion cameraContainerRotation, int zoomLevel, Quaternion cameraRotation)
    {
        this.cameraContainerPosition = cameraContainerPosition;
        this.cameraContainerRotation = cameraContainerRotation;
        this.zoomLevel = zoomLevel;
        this.cameraRotation = cameraRotation;
    }
    
    public Tuple<Vector3, Quaternion, int, Quaternion> RetrieveCameraPositionRotationAndZoom()
    {
        return Tuple.Create(cameraContainerPosition, cameraContainerRotation, zoomLevel, cameraRotation);
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
