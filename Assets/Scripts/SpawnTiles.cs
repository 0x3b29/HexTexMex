﻿using System.Collections.Generic;
using UnityEngine;

public class SpawnTiles : MonoBehaviour
{
    private float horizontalTileOffset = 1.732f;
    private float verticalTileOffset = 1.5f;

    private const float newWaterProbability = 0.006f;
    private const float adjacentWaterProbability = 0.35f;

    private const float newTreeProbability = 0.015f;
    private const float adjacentTreeProbability = 0.20f;

    private const float wellProbability = 0.0025f;
    private const float wheatProbability = 0.008f;

    private const float newRockProbability = 0.010f;
    private const float adjacentRockProbability = 0.20f;

    private const int minMountainCount = 0;
    private const int maxMountainCount = 20;
    private const float maxMountainHeight = 2f;
    private const float minMountainSlope = .1f;
    private const float maxMountainSlope = .5f;

    private const float tileHealthFactor = 10f;
    private const float tileHealthSlope = 13f;

    private const int minBoatCount = 1;
    private const int maxBoatCount = 15;

    private Tile[] tiles;
    private List<Tile> waterTiles;

    public void CreateMap(int seed, bool roundishShape, bool mountains)
    {
        GameObject tilesContainer = GameObject.Find("Tiles");
        Random.InitState(seed);

        int boardSizeX = GameManager.Instance.BoardSizeX;
        int boardSizeY = GameManager.Instance.BoardSizeX;

        tiles = new Tile[boardSizeX * boardSizeY];
        waterTiles = new List<Tile>();

        Material water = Resources.Load(Constants.materialsFolder + "Water", typeof(Material)) as Material;
        Material earth = Resources.Load(Constants.materialsFolder + "Earth", typeof(Material)) as Material;
        Material stone = Resources.Load(Constants.materialsFolder + "Stone", typeof(Material)) as Material;

        Material[] waterTile = new Material[2];
        waterTile[0] = earth;
        waterTile[1] = water;

        Material[] earthTile = new Material[2];
        earthTile[0] = earth;
        earthTile[1] = earth;

        Material[] stoneTile = new Material[2];
        stoneTile[0] = earth;
        stoneTile[1] = stone;

        int tileCounter = 0;

        // Create Tiles 
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                GameObject newTile;

                // Every second line of tiles needs to be shifted by half the horizontal tile 
                // offset to make the hexagons match.
                newTile = Instantiate(Resources.Load(Constants.prefabFolder + "Hex Parent") as GameObject, 
                    new Vector3(j * horizontalTileOffset + (i % 2 * (horizontalTileOffset / 2)), 0, i * verticalTileOffset), Quaternion.identity);

                newTile.name = "Tile" + i + "-" + j;
                newTile.transform.parent = tilesContainer.transform;

                GameObject hexagonGameObject = newTile.transform.Find("Hexagon").gameObject;

                int grassKind = Mathf.RoundToInt(Random.Range(1f, 3f));
                Material[] tileMats = hexagonGameObject.transform.GetComponent<Renderer>().materials;
                tileMats[1] = Resources.Load(Constants.materialsFolder + "Grass" + grassKind, typeof(Material)) as Material;
                newTile.transform.Find("Hexagon").GetComponent<Renderer>().materials = tileMats;

                // Attach tile script
                tiles[tileCounter] = newTile.AddComponent<Tile>(); 
                tiles[tileCounter].SetInitialValues(i, j, newTile, hexagonGameObject);

                tileCounter++;
            }
        }

        // Connect tiles 
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                GameObject goThisTile = GameObject.Find("Tile" + i + "-" + j);

                GameObject goLeftTile = GameObject.Find("Tile" + i + "-" + (j - 1));
                GameObject goRightTile = GameObject.Find("Tile" + i + "-" + (j + 1));

                GameObject goTopLeftTile = GameObject.Find("Tile" + (i + 1) + "-" + ((j - 1) + (i % 2)));
                GameObject goTopRightTile = GameObject.Find("Tile" + (i + 1) + "-" + (j + (i % 2)));

                GameObject goLowerLeftTile = GameObject.Find("Tile" + (i - 1) + "-" + ((j - 1) + (i % 2)));
                GameObject goLowerRightTile = GameObject.Find("Tile" + (i - 1) + "-" + (j + (i % 2)));

                Tile tile = goThisTile.GetComponent<Tile>();
                
                if (goLeftTile) tile.leftTile = goLeftTile.GetComponent<Tile>();
                if (goRightTile) tile.rightTile = goRightTile.GetComponent<Tile>();
                if (goTopLeftTile) tile.topLeftTile = goTopLeftTile.GetComponent<Tile>();
                if (goTopRightTile) tile.topRightTile = goTopRightTile.GetComponent<Tile>();
                if (goLowerLeftTile) tile.lowerLeftTile = goLowerLeftTile.GetComponent<Tile>();
                if (goLowerRightTile) tile.lowerRightTile = goLowerRightTile.GetComponent<Tile>();
            }
        }

        // Generate roundish shape
        if (roundishShape)
        {
            GameObject.Find("Tile" + Mathf.RoundToInt(boardSizeX / 2) + "-" + Mathf.RoundToInt(boardSizeY / 2)).GetComponent<Tile>().SetHealth( (Mathf.Min(boardSizeX, boardSizeY) / 2) * tileHealthFactor, tileHealthSlope);
            for (int i = 0; i < boardSizeX * boardSizeY; i++)
            {
                tiles[i].CheckHealth();
            }
        }

        // Set Water
        for (int i = 0; i < boardSizeX * boardSizeY; i++)
        {
            Tile tile = tiles[i];

            float waterProbability = newWaterProbability + (adjacentWaterProbability * tile.NeighboursWaterCount());

            if (Random.Range(0f, 1f) < waterProbability && tile.isActive)
            {
                tile.tileGameObject.transform.Find("Hexagon").GetComponent<Renderer>().materials = waterTile;
                tile.isWater = true;
                waterTiles.Add(tile);
            }
        }

        if (waterTiles.Count != 0) {
            // Add boats
            for (int i = 0; i < Random.Range(minBoatCount, maxBoatCount); i++)
            {
                // Spawn boat at a random active water tile
                Tile randomWaterTile = waterTiles.ToArray()[Random.Range(0, waterTiles.Count - 1)];

                GameObject boat = Instantiate(Resources.Load(Constants.prefabFolder + "boatyMacBootface") as GameObject, randomWaterTile.transform.position, Quaternion.identity);
                boat.AddComponent<BoatBehaviour>().Initialize(boat, randomWaterTile.GetRandomWaterNeighbour(), i);
            }
        }

        // Add Mountains
        if (mountains)
        {
            int mountainCount = Mathf.RoundToInt(Random.Range(minMountainCount, maxMountainCount));
            Debug.Log("Attempting to add " + mountainCount + " mountains");
        
            for (int i = 0; i < mountainCount; i++)
            {
                tiles[Mathf.RoundToInt(Random.Range(0, boardSizeX * boardSizeY))].SetHeight(Random.Range(0, maxMountainHeight), Random.Range(minMountainSlope, maxMountainSlope));
            }
        }
        
        // Set Trees
        for (int i = 0; i < boardSizeX * boardSizeY; i++)
        {
            Tile tile = tiles[i];

            float treeProbability = newTreeProbability + (adjacentTreeProbability * tile.GetNeighboursWoodCount());

            if (tile.isActive && !tile.isWater && Random.Range(0f, 1f) < treeProbability)
            {
                tile.tileGameObject.transform.Find("Hexagon").GetComponent<Renderer>().materials = earthTile;

                GameObject newTree;
                string treeToSpawn;
                int neighboursTreeCount = tile.GetNeighboursWoodCount();

                if (neighboursTreeCount == 0)
                {
                    treeToSpawn = "OneTreeParent";
                }
                else if (neighboursTreeCount == 1)
                {
                    treeToSpawn = "TwoTreeParent";
                }
                else
                {
                    treeToSpawn = "ThreeTreeParent";
                }

                newTree = Instantiate(Resources.Load(Constants.prefabFolder + treeToSpawn) as GameObject, tile.tileGameObject.transform.position, Quaternion.identity); //Vector3(j * 1.7f + (i % 2 * 0.85f), 0, i * 1.5f)
                newTree.transform.parent = tile.tileGameObject.transform;
                newTree.name = "Tree";

                tile.wood = newTree;
            }
        }

        // Set Wheat
        for (int i = 0; i < boardSizeX * boardSizeY; i++)
        {
            Tile tile = tiles[i];

            if (tile.isActive && !tile.isWater && !tile.wood && !tile.well && Random.Range(0f, 1f) < wheatProbability)
            {
                tile.tileGameObject.transform.Find("Hexagon").GetComponent<Renderer>().materials = earthTile;
                
                GameObject WheatParent;
                WheatParent = Instantiate(Resources.Load(Constants.prefabFolder + "WheatParent") as GameObject, tile.tileGameObject.transform.position, Quaternion.identity);
                WheatParent.transform.parent = tile.tileGameObject.transform;
                WheatParent.name = "WheatParent";

                tile.wheat = WheatParent;
            }
        }

        // Set Rocks
        for (int i = 0; i < boardSizeX * boardSizeY; i++)
        {
            Tile tile = tiles[i];
            float RockProbability = newRockProbability + (adjacentRockProbability * tile.GetNeighboursStoneCount());

            if (tile.isActive && !tile.isWater && !tile.wood && !tile.well && !tile.wheat && Random.Range(0f, 1f) < RockProbability)
            {
                tile.tileGameObject.transform.Find("Hexagon").GetComponent<Renderer>().materials = stoneTile;
                
                GameObject rock;
                string rockToSpawn;
                int neighboursRockCount = tile.GetNeighboursStoneCount();

                if (neighboursRockCount == 0)
                {
                    rockToSpawn = "OneStone";
                }
                else if (neighboursRockCount == 1)
                {
                    rockToSpawn = "TwoStones";
                }
                else if (neighboursRockCount == 2)
                {
                    rockToSpawn = "ThreeStones";
                }
                else
                {
                    rockToSpawn = "FourStones";
                }

                rock = Instantiate(Resources.Load(Constants.prefabFolder + rockToSpawn) as GameObject, tile.tileGameObject.transform.position, Quaternion.identity);
                rock.transform.parent = tile.tileGameObject.transform;
                rock.name = "Rock";

                tile.rock = rock;
            }
        }
    }
}
