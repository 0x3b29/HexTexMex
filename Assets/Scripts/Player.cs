using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private string name;
    private Color color;
    private int wood;
    private int stone;
    private int wheat;

    public List<Tile> tilesWithHouses;
    private List<Tile> tilesWithRoads;

    public Player (string name, Color color, int wood, int stone, int wheat)
    {
        tilesWithHouses = new List<Tile>();
        tilesWithRoads = new List<Tile>();

        this.name = name;
        this.color = color;
        this.wood = wood;
        this.stone = stone;
        this.wheat = wheat;
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
}
