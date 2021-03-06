﻿using System;
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

    public GameObject fire;

    public void SetInitialValues(int xCoordinate, int yCoordinate, GameObject tileGameObject, GameObject hexagonGameObject)
    {
        this.isActive = true;

        this.xCoordinate = xCoordinate;
        this.yCoordinate = yCoordinate;

        this.tileGameObject = tileGameObject;
        this.hexagonGameObject = hexagonGameObject;
    }

    public List<Tile> GetNeighbours()
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

    public int NeighboursWaterCount()
    {
        // Iterate over neighbour tiles and count water tiles.
        int neighboursWaterCount = 0;

        foreach (Tile tile in GetNeighbours())
        {
            if (tile.isWater) neighboursWaterCount++;
        }

        return neighboursWaterCount;
    }

    public int GetNeighboursWoodCount()
    {
        // Iterate over neighbour tiles and count tree tiles.
        int neighboursWoodCount = 0;

        foreach (Tile tile in GetNeighbours())
        {
            if (tile.wood) neighboursWoodCount++;
        }

        return neighboursWoodCount;
    }

    public int GetNeighboursStoneCount()
    {
        // Iterate over neighbour tiles and count stone tiles.
        int neighboursStoneCount = 0;

        foreach (Tile tile in GetNeighbours())
        {
            if (tile.rock) neighboursStoneCount++;
        }

        return neighboursStoneCount;
    }

    public int GetNeighboursWheatCount()
    {
        // Iterate over neighbour tiles and count wheat tiles.
        int neighboursWheatCount = 0;

        foreach (Tile tile in GetNeighbours())
        {
            if (tile.wheat) neighboursWheatCount++;
        }

        return neighboursWheatCount;
    }

    public bool IsFree()
    {
        return isActive && !isWater && !wood && !wheat && !well && !rock && !woodhouse && !isRoad;
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
        if (topLeftTile && topLeftTile.roadToLowerRightTile)
            DestroyAndUnset(topLeftTile.roadToLowerRightTile);

        // Top Right
        if (topRightTile && topRightTile.roadToLowerLeftTile)
            DestroyAndUnset(topRightTile.roadToLowerLeftTile);

        // Left
        if (leftTile && leftTile.roadToRightTile)
            DestroyAndUnset(leftTile.roadToRightTile);

        // Right
        if (rightTile && rightTile.roadToLeftTile)
            DestroyAndUnset(rightTile.roadToLeftTile);

        // Lower left
        if (lowerLeftTile && lowerLeftTile.roadToTopRightTile)
            DestroyAndUnset(lowerLeftTile.roadToTopRightTile);

        // Lower right
        if (lowerRightTile && lowerRightTile.roadToTopLeftTile)
            DestroyAndUnset(lowerRightTile.roadToTopLeftTile);
    }

    private void DeleteRoadsConnectingFromThisTile()
    {
        // Check if this tile has a road in a given direction.
        // If so, delete that road

        // Top Left
        if (roadToTopLeftTile)
            DestroyAndUnset(roadToTopLeftTile);

        // Top Right
        if (roadToTopRightTile)
            DestroyAndUnset(roadToTopRightTile);

        // Left
        if (roadToLeftTile)
            DestroyAndUnset(roadToLeftTile);

        // Right
        if (roadToRightTile)
            DestroyAndUnset(roadToRightTile);

        // Lower left
        if (roadToLowerLeftTile)
            DestroyAndUnset(roadToLowerLeftTile);

        // Lower right
        if (roadToLowerRightTile)
            DestroyAndUnset(roadToLowerRightTile);
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
        fire = Instantiate(Resources.Load(Constants.prefabFolder + "Fire") as GameObject, tileGameObject.transform.position, Quaternion.identity);
        fire.transform.parent = tileGameObject.transform;
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
    
        if (isRoad)
        {
            isRoad = false;

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
        roadCenter = Instantiate(Resources.Load(Constants.prefabFolder + "roadCenter") as GameObject, tileGameObject.transform.position, Quaternion.identity);
        roadCenter.transform.parent = tileGameObject.transform;

        // Set the players color and assign tile to him
        HelperFunctions.colorizeGameObject(roadCenter, owner.GetColor());
        owner.AddTileWithRoad(this);
        this.owner = owner;

        isRoad = true;      

        // Actual roadPieces are only placed by the checkRoad function
        CheckRoads();  
    }

    private bool NeedConnection(Tile neighbourTile)
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
    public void CheckRoads()
    {
        // Only proceed if tile has a road 
        // never check the same tile twice during one click
        if (!isRoad || Time.frameCount == lastFrameRoadCheck)
            return;

        lastFrameRoadCheck = Time.frameCount;

        // Check right
        if (NeedConnection(rightTile))
            roadToRightTile = SpawnRoad(180);

        // Check left
        if (NeedConnection(leftTile))
            roadToLeftTile = SpawnRoad(0);

        // Check upper right
        if (NeedConnection(topRightTile))
            roadToTopRightTile = SpawnRoad(120);

        // Check upper left
        if (NeedConnection(topLeftTile))
            roadToTopLeftTile = SpawnRoad(60);

        // Check lower right
        if (NeedConnection(lowerRightTile))
            roadToLowerRightTile= SpawnRoad(-120);

        // Check lower left
        if (NeedConnection(lowerLeftTile))
            roadToLowerLeftTile = SpawnRoad(-60);

        // Check all the neighbour tiles in a floodfill manner
        foreach (Tile tile in GetNeighbours())
            tile.CheckRoads();
    }

    private GameObject SpawnRoad(float roadRotation)
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

    public void PlaceHouse(Player owner)
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
        CheckRoads();

        foreach (Tile tile in GetNeighbours())
        {
            tile.CheckRoads();
        }
    }

    public void SetHeight(float height, float slope)
    {
        // Function is called by world generation. The first tile will be the top of the mountain
        // The function then recursivly calls itselve and sets its neighbours to some lower hight.

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

        // Recursivly call function on neighbouring tiles
        foreach (Tile neighbour in GetNeighbours())
        {
            neighbour.SetHeight(height - Random.Range(0.75f * slope, 1.25f * slope), slope);
        }
    }

    private float health = 0;
    public void SetHealth(float health, float slope)
    {
        // Function is called by world generation. The first tile will be the center of the map
        // The function then recursivly calls itselve and sets its neighbours to some lower health.

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

        // Recursivly call function on neighbouring tiles
        foreach (Tile neighbour in GetNeighbours())
        {
            neighbour.SetHealth(health - Random.Range(0.5f * slope, 1.5f * slope), slope);
        }
    }

    public void CheckHealth()
    {
        // After the health function iterated over the map, deactivate all the tiles with health lower or equal to 0
        if (health <= 0)
        {
            hexagonGameObject.SetActive(false);
            isActive = false;
        }
    }

    public Tile GetRandomWaterNeighbour()
    {
        // This function returns a random neighboruing water tile. It is called by boats to get a new destination
        List<Tile> waterTiles = new List<Tile>();

        foreach (Tile tile in GetNeighbours())
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

    public List<Tile> GetWalkableNeighbours()
    {
        List<Tile> walkableNeighbours = new List<Tile>();

        foreach (Tile tile in GetNeighbours())
        {
            if (tile.owner == this.owner &&
                (tile.isRoad || tile.woodhouse))
            {
                walkableNeighbours.Add(tile);
            }
        }

        return walkableNeighbours;
    }

    public bool HasNeighbourTileRoad(Player owner)
    {
        foreach (Tile tile in GetNeighbours())
        {
            if (tile.isRoad && tile.owner == owner)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasNeighbourTileBuilding(Player owner)
    {
        foreach (Tile tile in GetNeighbours())
        {
            if (tile.woodhouse && tile.owner == owner)
            {
                return true;
            }
        }

        return false;
    }
}
