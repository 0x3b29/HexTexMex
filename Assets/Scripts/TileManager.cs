using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileManager : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public TileManager leftTileManager;
    public TileManager rightTileManager;
    public TileManager topLeftTileManager;
    public TileManager topRightTileManager;
    public TileManager lowerLeftTileManager;
    public TileManager lowerRightTileManager;

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

    public bool hasRoad;
    public GameObject roadCenter;
    public GameObject roadToLeft;
    public GameObject roadToRight;
    public GameObject roadToTopLeft;
    public GameObject roadToTopRight;
    public GameObject roadToLowerLeft;
    public GameObject roadToLowerRight;

    public GameObject fire;

    public float health = 0;
    public float height = 0;

    public bool isEdgeTile;

    public void SetInitialValues(int xCoordinate, int yCoordinate, MeshRenderer meshRenderer)
    {
        this.isActive = true;

        this.xCoordinate = xCoordinate;
        this.yCoordinate = yCoordinate;

        this.meshRenderer = meshRenderer;
    }

    public List<TileManager> GetNeighbouringTileManagers()
    {
        List <TileManager> neighbouringTileManagers = new List<TileManager>();

        if (leftTileManager != null) neighbouringTileManagers.Add(leftTileManager);
        if (rightTileManager != null) neighbouringTileManagers.Add(rightTileManager);
        if (topLeftTileManager != null) neighbouringTileManagers.Add(topLeftTileManager);
        if (topRightTileManager != null) neighbouringTileManagers.Add(topRightTileManager);
        if (lowerLeftTileManager != null) neighbouringTileManagers.Add(lowerLeftTileManager);
        if (lowerRightTileManager != null) neighbouringTileManagers.Add(lowerRightTileManager);

        return neighbouringTileManagers;
    }

    public int NeighboursWaterCount()
    {
        // Iterate over neighbour tiles and count water tiles.
        int neighboursWaterCount = 0;

        foreach (TileManager tile in GetNeighbouringTileManagers())
        {
            if (tile.isWater) neighboursWaterCount++;
        }

        return neighboursWaterCount;
    }

    public int GetNeighboursWoodCount()
    {
        // Iterate over neighbour tiles and count tree tiles.
        int neighboursWoodCount = 0;

        foreach (TileManager tile in GetNeighbouringTileManagers())
        {
            if (tile.wood) neighboursWoodCount++;
        }

        return neighboursWoodCount;
    }

    public int GetNeighboursStoneCount()
    {
        // Iterate over neighbour tiles and count stone tiles.
        int neighboursStoneCount = 0;

        foreach (TileManager tile in GetNeighbouringTileManagers())
        {
            if (tile.rock) neighboursStoneCount++;
        }

        return neighboursStoneCount;
    }

    public int GetNeighboursWheatCount()
    {
        // Iterate over neighbour tiles and count wheat tiles.
        int neighboursWheatCount = 0;

        foreach (TileManager tile in GetNeighbouringTileManagers())
        {
            if (tile.wheat) neighboursWheatCount++;
        }

        return neighboursWheatCount;
    }

    public bool IsFree()
    {
        return isActive && 
            !isWater && 
            !wood && 
            !wheat && 
            !well && 
            !rock && 
            !woodhouse && 
            !hasRoad;
    }

    private void DestroyAndUnset(GameObject gameObject)
    {
        Destroy(gameObject);
        gameObject = null;
    }

    private void DeleteRoadsConnectingToThisTile()
    {
        // Check if a tile has a road to this one.
        // If so, delete that road

        // Top Left
        if (topLeftTileManager && topLeftTileManager.roadToLowerRight)
            DestroyAndUnset(topLeftTileManager.roadToLowerRight);

        // Top Right
        if (topRightTileManager && topRightTileManager.roadToLowerLeft)
            DestroyAndUnset(topRightTileManager.roadToLowerLeft);

        // Left
        if (leftTileManager && leftTileManager.roadToRight)
            DestroyAndUnset(leftTileManager.roadToRight);

        // Right
        if (rightTileManager && rightTileManager.roadToLeft)
            DestroyAndUnset(rightTileManager.roadToLeft);

        // Lower left
        if (lowerLeftTileManager && lowerLeftTileManager.roadToTopRight)
            DestroyAndUnset(lowerLeftTileManager.roadToTopRight);

        // Lower right
        if (lowerRightTileManager && lowerRightTileManager.roadToTopLeft)
            DestroyAndUnset(lowerRightTileManager.roadToTopLeft);
    }

    private void DeleteRoadsConnectingFromThisTile()
    {
        // Check if this tile has a road in a given direction.
        // If so, delete that road

        // Top Left
        if (roadToTopLeft)
            DestroyAndUnset(roadToTopLeft);

        // Top Right
        if (roadToTopRight)
            DestroyAndUnset(roadToTopRight);

        // Left
        if (roadToLeft)
            DestroyAndUnset(roadToLeft);

        // Right
        if (roadToRight)
            DestroyAndUnset(roadToRight);

        // Lower left
        if (roadToLowerLeft)
            DestroyAndUnset(roadToLowerLeft);

        // Lower right
        if (roadToLowerRight)
            DestroyAndUnset(roadToLowerRight);
    }

    public void SetOnFire()
    {
        if (!isActive || isWater)
        {
            return;
        }

        Invoke("SpawnFire", Random.Range(0, 1.5f));
        Invoke("DestroyFeature", Random.Range(2.5f, 4f));
    }

    private void SpawnFire()
    {
        fire = Instantiate(Resources.Load(Constants.prefabFolder + "Fire") as GameObject, gameObject.transform.position, Quaternion.identity);
        fire.transform.parent = gameObject.transform;
        Invoke("RemoveFire", 8f);
    }

    private void RemoveFire()
    {
        Destroy(fire);
        fire = null;
    }

    public void DestroyFeature()
    {
        if (woodhouse)
        {
            DestroyAndUnset(woodhouse);

            // Delete roads to this tile
            DeleteRoadsConnectingToThisTile();

            // Also remove from player
            owner.RemoveTileWithHouse(this);
            this.owner = null;
        }
    
        if (hasRoad)
        {
            hasRoad = false;

            // Remove center piece
            Destroy(roadCenter);
            roadCenter = null;

            // Delete roads from and to this tile
            DeleteRoadsConnectingFromThisTile();
            DeleteRoadsConnectingToThisTile();

            // Also remove from player
            owner.RemoveTileWithRoad(this);
            this.owner = null;
        }
    }

    public void PlaceRoad(Player owner)
    {
        // Function is only called when player placed a road
        roadCenter = Instantiate(Resources.Load(Constants.prefabFolder + "roadCenter") as GameObject, gameObject.transform.position, Quaternion.identity);
        roadCenter.transform.parent = gameObject.transform;

        // Set the players color and assign tile to him
        HelperFunctions.colorizeGameObject(roadCenter, owner.GetColor());
        owner.AddTileWithRoad(this);
        this.owner = owner;

        hasRoad = true;      

        // Actual roadPieces are only placed by the checkRoad function
        CheckRoads();  
    }

    private bool NeedsConnection(TileManager neighbouringTileManager)
    {
        // This function checks if a roadpiece to the passed tile needs to be placed
        // Test if tile exists
        if (!neighbouringTileManager)
            return false;

        // Test if tile is active
        if (!neighbouringTileManager.isActive)
            return false;

        // Test if the owner is the same
        if (neighbouringTileManager.owner != owner)
            return false;

        // Test if there is even a reason to have a road to this tile
        if (!neighbouringTileManager.hasRoad && !neighbouringTileManager.woodhouse)
            return false;

        // Test if there is already a road to the neighbour tile
        if ((neighbouringTileManager == topLeftTileManager && roadToTopLeft) ||
            (neighbouringTileManager == topRightTileManager && roadToTopRight) ||
            (neighbouringTileManager == leftTileManager && roadToLeft) ||
            (neighbouringTileManager == rightTileManager && roadToRight) ||
            (neighbouringTileManager == lowerLeftTileManager && roadToLowerLeft) ||
            (neighbouringTileManager == lowerRightTileManager && roadToLowerRight))
            return false;

        // Only if all of these tests pass, we need to put a roadpiece
        return true;
    }

    public int lastFrameRoadCheck;
    public void CheckRoads()
    {
        // Only proceed if tile has a road 
        // never check the same tile twice during one click
        if (!hasRoad || Time.frameCount == lastFrameRoadCheck)
            return;

        lastFrameRoadCheck = Time.frameCount;

        // Check right
        if (NeedsConnection(rightTileManager))
            roadToRight = SpawnRoad(180);

        // Check left
        if (NeedsConnection(leftTileManager))
            roadToLeft = SpawnRoad(0);

        // Check upper right
        if (NeedsConnection(topRightTileManager))
            roadToTopRight = SpawnRoad(120);

        // Check upper left
        if (NeedsConnection(topLeftTileManager))
            roadToTopLeft = SpawnRoad(60);

        // Check lower right
        if (NeedsConnection(lowerRightTileManager))
            roadToLowerRight= SpawnRoad(-120);

        // Check lower left
        if (NeedsConnection(lowerLeftTileManager))
            roadToLowerLeft = SpawnRoad(-60);

        // Check all the neighbour tiles in a floodfill manner
        foreach (TileManager tile in GetNeighbouringTileManagers())
            tile.CheckRoads();
    }

    private GameObject SpawnRoad(float roadRotation)
    {
        // Spawn the roadpiece
        GameObject roadPiece = Instantiate(Resources.Load(Constants.prefabFolder + "roadPiece") as GameObject, gameObject.transform.position, Quaternion.identity);
        roadPiece.transform.parent = gameObject.transform;

        // Set the players color
        HelperFunctions.colorizeGameObject(roadPiece, GameManager.Instance.turnManager.GetCurrentPlayer().GetColor());

        // Turn it to face the correct neighbour
        Quaternion rotation = roadPiece.transform.rotation;
        rotation.eulerAngles = new Vector3(0, roadRotation, 0);
        roadPiece.transform.rotation = rotation;

        // Return the object for storage and later use
        return roadPiece;
    }

    public void PlaceHouse(Player owner)
    {
        // Spawn the house
        GameObject woodHouse = Instantiate(Resources.Load(Constants.prefabFolder + "Woodhouse") as GameObject, gameObject.transform.position, Quaternion.identity);
        woodHouse.transform.parent = gameObject.transform;

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
        CheckRoads();

        foreach (TileManager tile in GetNeighbouringTileManagers())
        {
            tile.CheckRoads();
        }
    }

    public float GetHeight()
    {
        return height;
    }

    public void SetHeight(float height)
    {
        this.height = height;

        // Set height to the new height
        Vector3 position = gameObject.transform.position;
        position.y = height;
        gameObject.transform.position = position;

        // Enlarge the hexagon to give them all the same bottom level
        /*
        Vector3 scale = gameObject.transform.localScale;
        scale.y = 1 + height / 2;
        gameObject.transform.localScale = scale;
        */
    }

    
    public void SetHealth(float health)
    {
        // Set my health to the new health
        this.health = health;

        // Add my health to the tilescale to create a cloud island effect
        Vector3 scale = this.gameObject.transform.localScale;
        scale.y = 0.1f + health * 0.8f + Random.Range(0, Mathf.Log(Mathf.Max(1, health)));
        this.gameObject.transform.localScale = scale;
    }

    public void CheckHealth()
    {
        // After the health function iterated over the map, deactivate all the tiles with health lower or equal to 0
        if (health <= 0)
        {
            gameObject.SetActive(false);
            isActive = false;
        }
    }

    public TileManager GetRandomNeighboursHavingWater()
    {
        // This function returns a random neighboruing water tile. It is called by boats to get a new destination
        List<TileManager> waterTiles = new List<TileManager>();

        foreach (TileManager tileManager in GetNeighbouringTileManagers())
        {
            if (tileManager.isWater && tileManager.isActive)
            {
                waterTiles.Add(tileManager);
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

    public List<TileManager> GetWalkableNeighbours()
    {
        List<TileManager> walkableNeighbours = new List<TileManager>();

        foreach (TileManager tileManager in GetNeighbouringTileManagers())
        {
            if (tileManager.owner == this.owner &&
                (tileManager.hasRoad || tileManager.woodhouse))
            {
                walkableNeighbours.Add(tileManager);
            }
        }

        return walkableNeighbours;
    }

    public bool HasNeighbouringTileManagerRoad(Player owner)
    {
        foreach (TileManager tile in GetNeighbouringTileManagers())
        {
            if (tile.hasRoad && tile.owner == owner)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasNeighbouringTileManagerBuilding(Player owner)
    {
        foreach (TileManager tile in GetNeighbouringTileManagers())
        {
            if (tile.woodhouse && tile.owner == owner)
            {
                return true;
            }
        }

        return false;
    }
}
