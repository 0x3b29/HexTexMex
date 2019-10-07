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

    public void setHeight(float height, float slope)
    {
        // If my height is heigher than the new height, bail out
        if (tileGameObject.transform.position.y >= height || height < 0 || slope < 0.01 || isWater)
        {
            return;
        }

        // Set my height to the new height
        Vector3 position = tileGameObject.transform.position;
        position.y = height;
        tileGameObject.transform.position = position;

        // 5 % chance
        if (Random.Range(0,1f) < .05f)
        {
            // to decrease slope by a factor of 0 to 1/4 * slope
            slope = slope - Random.Range( 0, 0.25f * slope);
        }

        foreach (Tile neighbour in getNeigbhours())
        {
            neighbour.setHeight(height - slope, slope);
        }
    }
}
