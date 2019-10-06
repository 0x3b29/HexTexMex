using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject tileGameObject;
    public Tile leftTile;
    public Tile rightTile;
    public Tile topLeftTile;
    public Tile topRightTile;
    public Tile lowerLeftTile;
    public Tile lowerRightTile;

    public int xCoordinate;
    public int yCoordinate;

    public bool isWater;
    public bool isForest;
    public bool isWheat;
    public bool isWell;
    public bool isRock;

    public void setInitialValues(int xCoordinate, int yCoordinate, GameObject tileGameObject)
    {
        this.xCoordinate = xCoordinate;
        this.yCoordinate = yCoordinate;

        this.tileGameObject = tileGameObject;
    }

    public List<Tile> getNeigbhours()
    {
        List <Tile> neighbourList = new List<Tile>();

        if (leftTile != null) neighbourList.Add(leftTile);
        if (rightTile != null) neighbourList.Add(rightTile);
        if (topLeftTile != null) neighbourList.Add(topLeftTile);
        if (topRightTile != null) neighbourList.Add(topRightTile);
        if (lowerLeftTile != null) neighbourList.Add(lowerLeftTile);
        if (lowerRightTile != null) neighbourList.Add(lowerRightTile);

        return neighbourList;
    }

    public int neighboursWaterCount()
    {
        int neighboursWaterCount = 0;

        foreach (Tile tile in getNeigbhours())
        {
            if (tile.isWater) neighboursWaterCount++;
        }

        return neighboursWaterCount;
    }

    public int neighboursTreeCount()
    {
        int neighboursTreeCount = 0;

        foreach (Tile tile in getNeigbhours())
        {
            if (tile.isForest) neighboursTreeCount++;
        }

        return neighboursTreeCount;
    }

    public int neighboursRockCount()
    {
        int neighboursRockCount = 0;

        foreach (Tile tile in getNeigbhours())
        {
            if (tile.isRock) neighboursRockCount++;
        }

        return neighboursRockCount;
    }
}
