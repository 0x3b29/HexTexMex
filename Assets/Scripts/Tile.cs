using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public Player owner;

    public GameObject rock;
    public GameObject wood;
    public GameObject wheat;
    public GameObject well;
    public GameObject woodhouse;

    public bool isRoad;
    public GameObject roadCenter;
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
        // Iterate over neighbour tiles and count water tiles.
        int neighboursWaterCount = 0;

        foreach (Tile tile in getNeighbours())
        {
            if (tile.isWater) neighboursWaterCount++;
        }

        return neighboursWaterCount;
    }

    public int GetNeighboursWoodCount()
    {
        // Iterate over neighbour tiles and count tree tiles.
        int neighboursWoodCount = 0;

        foreach (Tile tile in getNeighbours())
        {
            if (tile.wood) neighboursWoodCount++;
        }

        return neighboursWoodCount;
    }

    public int GetNeighboursStoneCount()
    {
        // Iterate over neighbour tiles and count stone tiles.
        int neighboursStoneCount = 0;

        foreach (Tile tile in getNeighbours())
        {
            if (tile.rock) neighboursStoneCount++;
        }

        return neighboursStoneCount;
    }

    public int GetNeighboursWheatCount()
    {
        // Iterate over neighbour tiles and count wheat tiles.
        int neighboursWheatCount = 0;

        foreach (Tile tile in getNeighbours())
        {
            if (tile.wheat) neighboursWheatCount++;
        }

        return neighboursWheatCount;
    }


    public bool isFree()
    {
        return isActive && !isWater && !wood && !wheat && !well && !rock && !woodhouse && !isRoad;
    }

    private void deleteRoadsConnectingToThisTile()
    {
        // Check if a tile has a road to this one.
        // If so, delete that road

        // Top Left
        if (topLeftTile && topLeftTile.roadToLowerRightTile)
        {
            Destroy(topLeftTile.roadToLowerRightTile);
            topLeftTile.roadToLowerRightTile = null;
        }

        // Top Right
        if (topRightTile && topRightTile.roadToLowerLeftTile)
        {
            Destroy(topRightTile.roadToLowerLeftTile);
            topRightTile.roadToLowerLeftTile = null;
        }

        // Left
        if (leftTile && leftTile.roadToRightTile)
        {
            Destroy(leftTile.roadToRightTile);
            leftTile.roadToRightTile = null;
        }

        // Right
        if (rightTile && rightTile.roadToLeftTile)
        {
            Destroy(rightTile.roadToLeftTile);
            rightTile.roadToLeftTile = null;
        }

        // Lower left
        if (lowerLeftTile && lowerLeftTile.roadToTopRightTile)
        {
            Destroy(lowerLeftTile.roadToTopRightTile);
            lowerLeftTile.roadToTopRightTile = null;
        }

        // Lower right
        if (lowerRightTile && lowerRightTile.roadToTopLeftTile)
        {
            Destroy(lowerRightTile.roadToTopLeftTile);
            lowerRightTile.roadToTopLeftTile = null;
        }
    }

    private void deleteRoadsConnectingFromThisTile()
    {
        // Check if this tile has a road in a given direction.
        // If so, delete that road

        // Top Left
        if (roadToTopLeftTile)
        {
            Destroy(roadToTopLeftTile);
            roadToTopLeftTile = null;
        }

        // Top Right
        if (roadToTopRightTile)
        {
            Destroy(roadToTopRightTile);
            roadToTopRightTile = null;
        }

        // Left
        if (roadToLeftTile)
        {
            Destroy(roadToLeftTile);
            roadToLeftTile = null;
        }

        // Right
        if (roadToRightTile)
        {
            Destroy(roadToRightTile);
            roadToRightTile = null;
        }

        // Lower left
        if (roadToLowerLeftTile)
        {
            Destroy(roadToLowerLeftTile);
            roadToLowerLeftTile = null;
        }

        // Lower right
        if (roadToLowerRightTile)
        {
            Destroy(roadToLowerRightTile);
            roadToLowerRightTile = null;
        }
    }

    public void destroyFeature()
    {
        if (woodhouse)
        {
            Destroy(woodhouse);
            woodhouse = null;

            // Delete roads to this tile
            deleteRoadsConnectingToThisTile();

            // Also remove from player
            GameManager.Instance.turnManager.GetCurrentPlayer().RemoveTileWithHouse(this);
            this.owner = null;
        }
    
        if (isRoad)
        {
            isRoad = false;

            // Remove center piece
            Destroy(roadCenter);
            roadCenter = null;

            // Delete roads from and to this tile
            deleteRoadsConnectingFromThisTile();
            deleteRoadsConnectingToThisTile();

            // Also remove from player
            GameManager.Instance.turnManager.GetCurrentPlayer().RemoveTileWithRoad(this);
            this.owner = null;
        }
    }

    public void addRoad(Player owner)
    {
        // Function is only called when player placed a road
        roadCenter = Instantiate(Resources.Load(Constants.prefabFolder + "roadCenter") as GameObject, tileGameObject.transform.position, Quaternion.identity);
        roadCenter.transform.parent = tileGameObject.transform;

        // Set the players color and assign tile to him
        HelperFunctions.colorizeGameObject(roadCenter, owner.GetColor());
        owner.AddTileWithRoad(this);
        this.owner = owner;

        isRoad = true;      

        // Actual roadPieces are only placed by the checkRoad function
        checkRoads();  
    }

    private bool needConnection(Tile neighbourTile)
    {
        // This function checks if a roadpiece to the passed tile needs to be placed
        // Test if tile exists
        if (!neighbourTile)
            return false;

        // Test if tile is active
        if (!neighbourTile.isActive)
            return false;

        // Test if the owner is the same
        if (neighbourTile.owner != owner)
            return false;

        // Test if there is even a reason to have a road to this tile
        if (!neighbourTile.isRoad && !neighbourTile.woodhouse)
            return false;

        // Test if there is already a road to the neighbour tile
        if ((neighbourTile == topLeftTile && roadToTopLeftTile) ||
            (neighbourTile == topRightTile && roadToTopRightTile) ||
            (neighbourTile == leftTile && roadToLeftTile) ||
            (neighbourTile == rightTile && roadToRightTile) ||
            (neighbourTile == lowerLeftTile && roadToLowerLeftTile) ||
            (neighbourTile == lowerRightTile && roadToLowerRightTile))
            return false;

        // Only if all of these tests pass, we need to put a roadpiece
        return true;
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
        if (needConnection(rightTile))
            roadToRightTile = spawnRoad(180);

        // Check left
        if (needConnection(leftTile))
            roadToLeftTile = spawnRoad(0);

        // Check upper right
        if (needConnection(topRightTile))
            roadToTopRightTile = spawnRoad(120);

        // Check upper left
        if (needConnection(topLeftTile))
            roadToTopLeftTile = spawnRoad(60);

        // Check lower right
        if (needConnection(lowerRightTile))
            roadToLowerRightTile= spawnRoad(-120);

        // Check lower left
        if (needConnection(lowerLeftTile))
            roadToLowerLeftTile = spawnRoad(-60);

        // Check all the neighbour tiles in a floodfill manner
        foreach (Tile tile in getNeighbours())
            tile.checkRoads();
    }

    private GameObject spawnRoad(float roadRotation)
    {
        // Spawn the roadpiece
        GameObject roadPiece = Instantiate(Resources.Load(Constants.prefabFolder + "roadPiece") as GameObject, tileGameObject.transform.position, Quaternion.identity);
        roadPiece.transform.parent = tileGameObject.transform;

        // Set the players color
        HelperFunctions.colorizeGameObject(roadPiece, GameManager.Instance.turnManager.GetCurrentPlayer().GetColor());

        // Turn it to face the correct neighbour
        Quaternion rotation = roadPiece.transform.rotation;
        rotation.eulerAngles = new Vector3(0, roadRotation, 0);
        roadPiece.transform.rotation = rotation;

        // Return the object for storage and later use
        return roadPiece;
    }

    public void placeHouse(Player owner)
    {
        // Spawn the house
        GameObject woodHouse = Instantiate(Resources.Load(Constants.prefabFolder + "Woodhouse") as GameObject, tileGameObject.transform.position, Quaternion.identity);
        woodHouse.transform.parent = tileGameObject.transform;

        // Set the players color and assign tile to him
        HelperFunctions.colorizeGameObject(woodHouse, owner.GetColor());
        owner.AddTileWithHouse(this);
        this.owner = owner;

        // Randomly rotate house to add some variation
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
        // If my current height is higher than the new height, bail out
        if (tileGameObject.transform.position.y >= height || height < 0 || slope < 0.01 || isWater)
        {
            return;
        }

        // Set height to the new height
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
        // If my health is higher than the new health, bail out
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
