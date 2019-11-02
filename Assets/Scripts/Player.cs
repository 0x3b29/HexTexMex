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

    private List<Tile> tilesWithHouses;

    public Player (string name, Color color, int wood, int stone, int wheat)
    {
        tilesWithHouses = new List<Tile>();

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

    public int GetStone()
    {
        return stone;
    }

    public int GetWood()
    {
        return wood;
    }

    public int GetWheat()
    {
        return wheat;
    }
}
