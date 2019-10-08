using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject tileGameObject;
    public GameObject hexagonGameObject;
    public Tile leftTile;
    public Tile rightTile;
    public Tile topLeftTile;
    public Tile topRightTile;
    public Tile lowerLeftTile;
    public Tile lowerRightTile;

    public int xCoordinate;
    public int yCoordinate;

    public bool isActive;
    public bool isWater;
    public bool isForest;
    public bool isWheat;
    public bool isWell;
    public bool isRock;

    public void setInitialValues(int xCoordinate, int yCoordinate, GameObject tileGameObject, GameObject hexagonGameObject)
    {
        this.xCoordinate = xCoordinate;
        this.yCoordinate = yCoordinate;

        this.tileGameObject = tileGameObject;
        this.hexagonGameObject = hexagonGameObject;
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

        // Enlarge the hexagon to give them all the same bottom level
        Vector3 scale = hexagonGameObject.transform.localScale;
        scale.y = 1 + height / 2;
        hexagonGameObject.transform.localScale = scale;

        foreach (Tile neighbour in getNeigbhours())
        {
            neighbour.setHeight(height - Random.Range(0.75f * slope, 1.25f * slope), slope);
        }
    }

    private float health = 0;
    public void setHealth(float health, float slope)
    {
        // If my health is heigher than the new health, bail out
        if (this.health >= health || health <= 0 || slope < 0.01)
        {
            return;
        }

        // Set my health to the new health
        this.health = health;

        // Add my health to the tilescale to create a cloud island effect
        Vector3 scale = this.tileGameObject.transform.localScale;
        scale.y += (health / 100);
        this.tileGameObject.transform.localScale = scale;

        // Set health to neighbours
        foreach (Tile neighbour in getNeigbhours())
        {
            neighbour.setHealth(health - Random.Range(0.5f * slope, 1.5f * slope), slope);
        }
    }

    public void checkHealth()
    {
        if (health <= 0)
        {
            tileGameObject.SetActive(false);
            isActive = false;
        }
    }
}
