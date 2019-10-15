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

    public GameObject forest;
    public GameObject wheat;
    public GameObject well;
    public GameObject rock;
    public GameObject woodhouse;

    public bool isRoad;
    public GameObject roadToLeftTile;
    public GameObject roadToRightTile;
    public GameObject roadToTopLeftTile;
    public GameObject roadToTopRightTile;
    public GameObject roadToLowerLeftTile;
    public GameObject roadToLowerRightTile;


    public void setInitialValues(int xCoordinate, int yCoordinate, GameObject tileGameObject, GameObject hexagonGameObject)
    {
        this.isActive = true;

        this.xCoordinate = xCoordinate;
        this.yCoordinate = yCoordinate;

        this.tileGameObject = tileGameObject;
        this.hexagonGameObject = hexagonGameObject;
    }

    public List<Tile> getNeighbours()
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

        foreach (Tile tile in getNeighbours())
        {
            if (tile.isWater) neighboursWaterCount++;
        }

        return neighboursWaterCount;
    }

    public int neighboursTreeCount()
    {
        int neighboursTreeCount = 0;

        foreach (Tile tile in getNeighbours())
        {
            if (tile.forest) neighboursTreeCount++;
        }

        return neighboursTreeCount;
    }

    public int neighboursRockCount()
    {
        int neighboursRockCount = 0;

        foreach (Tile tile in getNeighbours())
        {
            if (tile.rock) neighboursRockCount++;
        }

        return neighboursRockCount;
    }

    public bool isFree()
    {
        return isActive && !isWater && !forest && !wheat && !well && !rock && !woodhouse && !isRoad;
    }

    public void destroyFeature()
    {

    }

    public void addRoad()
    {
        // Function is only called when player placed a road
        GameObject roadCenter = Instantiate(Resources.Load(Constants.prefabFolder + "roadCenter") as GameObject, tileGameObject.transform.position, Quaternion.identity);
        roadCenter.transform.parent = tileGameObject.transform;

        isRoad = true;

        // Actual roadPieces are only placed by the checkRoad function
        checkRoads();  
    }

    public int lastFrameRoadCheck;
    public void checkRoads()
    {
        // Only proceed if tile has a road 
        // never check the same tile twice during one click
        if (!isRoad || Time.frameCount == lastFrameRoadCheck)
        {
            return;
        }

        lastFrameRoadCheck = Time.frameCount;

        // Check right
        if (!roadToRightTile && (rightTile.isRoad || rightTile.woodhouse))
        {
            roadToRightTile = spawnRoad(180);
        }

        // Check left
        if (!roadToLeftTile && (leftTile.isRoad || leftTile.woodhouse))
        {
            roadToLeftTile = spawnRoad(0);
        }

        // Check upper right
        if (!roadToTopRightTile && (topRightTile.isRoad || topRightTile.woodhouse))
        {
            roadToTopRightTile = spawnRoad(120);
        }

        // Check upper left
        if (!roadToTopLeftTile && (topLeftTile.isRoad || topLeftTile.woodhouse))
        {
            roadToTopLeftTile = spawnRoad(60);
        }

        // Check lower right
        if (!roadToLowerRightTile && (lowerRightTile.isRoad || lowerRightTile.woodhouse))
        {
            roadToLowerRightTile= spawnRoad(-120);
        }

        // Check lower left
        if (!roadToLowerLeftTile && (lowerLeftTile.isRoad || lowerLeftTile.woodhouse))
        {
            roadToLowerLeftTile = spawnRoad(-60);
        }

        // Check all the neighbour tiles
        foreach (Tile tile in getNeighbours())
        {
            tile.checkRoads();
        }
    }

    private GameObject spawnRoad(float roadRotation)
    {
        // Spawn the roadpiece
        GameObject roadPiece = Instantiate(Resources.Load(Constants.prefabFolder + "roadPiece") as GameObject, tileGameObject.transform.position, Quaternion.identity);
        roadPiece.transform.parent = tileGameObject.transform;

        // Turn it to face the correct neighbour
        Quaternion rotation = roadPiece.transform.rotation;
        rotation.eulerAngles = new Vector3(0, roadRotation, 0);
        roadPiece.transform.rotation = rotation;

        // Return the object for storage and later use
        return roadPiece;
    }

    public void placeHouse()
    {
        GameObject woodHouse = Instantiate(Resources.Load(Constants.prefabFolder + "Woodhouse Parent") as GameObject, tileGameObject.transform.position, Quaternion.identity);
        woodHouse.transform.parent = tileGameObject.transform;

        Quaternion rotation = woodHouse.transform.rotation;
        rotation.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        woodHouse.transform.rotation = rotation;

        woodHouse.name = "woodHouse";
        this.woodhouse = woodHouse;
        checkRoads();

        foreach (Tile tile in getNeighbours())
        {
            tile.checkRoads();
        }
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

        foreach (Tile neighbour in getNeighbours())
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
        foreach (Tile neighbour in getNeighbours())
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

    public Tile getRandomWaterNeighbour()
    {
        List<Tile> waterTiles = new List<Tile>();

        foreach (Tile tile in getNeighbours())
        {
            if (tile.isWater && tile.isActive)
            {
                waterTiles.Add(tile);
            }
        }

        if (waterTiles.Count == 0)
        {
            return this;
        }
        else
        {
            return waterTiles.ToArray()[Random.Range(0, waterTiles.Count)];
        }
    }
}
