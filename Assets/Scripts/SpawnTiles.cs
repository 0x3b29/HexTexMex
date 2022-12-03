using System.Collections.Generic;
using UnityEngine;

public class SpawnTiles : MonoBehaviour
{
    private const float horizontalTileOffset = 1.732f;
    private const float verticalTileOffset = 1.5f;

    private const float newWaterProbability = 0.006f;
    private const float adjacentWaterProbability = 0.35f;

    private const float newTreeProbability = 0.015f;
    private const float adjacentTreeProbability = 0.20f;

    private const float wellProbability = 0.0025f;
    private const float wheatProbability = 0.008f;

    private const float newRockProbability = 0.010f;
    private const float adjacentRockProbability = 0.20f;

    private const int minBoatCount = 1;
    private const int maxBoatCount = 15;

    public Tile[,] tiles;
    public List<Tile> waterTiles;
    public List<GameObject> boats = new List<GameObject>();
    public List<GameObject> tileContainers = new List<GameObject>();

    public bool recreateMap = false;

    public bool customBreak = false;

    public int mapSize = 20;

    public bool roundishShape = true;

    public float perlinNoiseHfScale = 1;
    public float perlinNoiseHfFactor = 0.2f;

    public float perlinNoiseLfScale = 0.1f;
    public float perlinNoiseLfFactor = 2;

    public float verticalOffset = 0.5f;

    public void Update()
    {
        if (recreateMap)
        {
            // recreateMap = false;

            if (tiles != null)
            {
                foreach (GameObject tile in tileContainers)
                    Destroy(tile);

                tileContainers.Clear();

                foreach (GameObject boat in boats)
                    Destroy(boat);

                boats.Clear();
            }

            CreateMap(mapSize, mapSize, Random.Range(int.MinValue, int.MaxValue), true, true);
        }
    }

    public void CreateMap(int boardSizeX, int boardSizeY, int seed, bool roundishShape, bool mountains)
    {
        if (customBreak)
            return;

        GameObject tilesContainer = GameObject.Find("Tiles");
        
        Random.InitState(seed);

        float perlinNoiseOffset = Random.Range(-1000f, 1000f);

        tiles = new Tile[boardSizeX, boardSizeY];
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

        // Create Tiles 
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                // Every second line of tiles needs to be shifted by half the horizontal tile 
                // offset to make the hexagons match.
                GameObject newTile = Instantiate(Resources.Load(Constants.prefabFolder + "Hex Parent") as GameObject,
                    new Vector3(j * horizontalTileOffset + (i % 2 * (horizontalTileOffset / 2)), 0, i * verticalTileOffset), Quaternion.identity);

                tileContainers.Add(newTile);

                newTile.name = "Tile" + i + "-" + j;
                newTile.transform.parent = tilesContainer.transform;

                GameObject hexagonGameObject = newTile.transform.Find("Hexagon").gameObject;

                int grassKind = Mathf.RoundToInt(Random.Range(1f, 3f));
                Material[] tileMats = hexagonGameObject.transform.GetComponent<Renderer>().materials;
                tileMats[1] = Resources.Load(Constants.materialsFolder + "Grass" + grassKind, typeof(Material)) as Material;
                newTile.transform.Find("Hexagon").GetComponent<Renderer>().materials = tileMats;

                // Attach tile script
                tiles[i, j] = newTile.AddComponent<Tile>();
                tiles[i, j].SetInitialValues(i, j, newTile, hexagonGameObject);
            }
        }

        // Connect tiles 
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject goThisTile = tiles[x, y].gameObject;

                GameObject goLeftTile = null;
                GameObject goRightTile = null;
                GameObject goTopLeftTile = null;
                GameObject goTopRightTile = null;
                GameObject goLowerLeftTile = null;
                GameObject goLowerRightTile = null;

                if ((y - 1) > 0)
                    goLeftTile = tiles[x, (y - 1)].gameObject;

                if ((y + 1) < boardSizeY)
                    goRightTile = tiles[x, (y + 1)].gameObject;

                if ((x + 1) < boardSizeX && ((y - 1) + (x % 2)) > 0)
                    goTopLeftTile = tiles[(x + 1), (y - 1) + (x % 2)].gameObject;

                if ((x + 1) < boardSizeX && (y + (x % 2)) < boardSizeY)
                    goTopRightTile = tiles[(x + 1), (y + (x % 2))].gameObject;

                if ((x - 1) > 0 && ((y - 1) + (x % 2)) > 0)
                    goLowerLeftTile = tiles[(x - 1), ((y - 1) + (x % 2))].gameObject;

                if ((x - 1) > 0 && (y + (x % 2)) < boardSizeY)
                    goLowerRightTile = tiles[(x - 1), (y + (x % 2))].gameObject;

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
            for (int x = 0; x < boardSizeX; x++)
            {
                for (int y = 0; y < boardSizeY; y++)
                {
                    float distanceToCenter = Vector2.Distance(new Vector2(x, y), new Vector2(boardSizeX / 2, boardSizeY / 2));

                    tiles[x, y].SetHealth(
                        (boardSizeX / 2 + boardSizeY / 2)
                        / 2
                        - distanceToCenter
                        - 0.5f 
                        - Random.Range(0f, 1f));

                    tiles[x, y].CheckHealth();
                }
            }
        }

        // Add Mountains
        if (mountains)
        {
            for (int i = 0; i < boardSizeX; i++)
            {
                for (int j = 0; j < boardSizeY; j++)
                {
                    float lfHeight = 
                        Mathf.PerlinNoise((i * perlinNoiseLfScale) + perlinNoiseOffset,(j * perlinNoiseLfScale) + perlinNoiseOffset)
                            * perlinNoiseLfFactor - (perlinNoiseLfFactor / 2);

                    float hfHeight =
                        Mathf.PerlinNoise((i * perlinNoiseHfScale) + perlinNoiseOffset, (j * perlinNoiseHfScale) + perlinNoiseOffset)
                            * perlinNoiseHfFactor - (perlinNoiseHfFactor / 2);

                    tiles[i, j].SetHeight(lfHeight + hfHeight + verticalOffset);
                }
            }
        }

        // Set Water
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                Tile tile = tiles[i, j];

                float waterProbability = newWaterProbability + (adjacentWaterProbability * tile.NeighboursWaterCount());

                if (tile.GetHeight() < 0 && tile.isActive)
                {
                    tile.SetHeight(0);

                    tile.tileGameObject.transform.Find("Hexagon").GetComponent<Renderer>().materials = waterTile;
                    tile.isWater = true;
                    waterTiles.Add(tile);
                }
            }
        }

        if (waterTiles.Count != 0)
        {
            // Add boats
            for (int i = 0; i < Random.Range(minBoatCount, maxBoatCount); i++)
            {
                // Spawn boat at a random active water tile
                Tile randomWaterTile = waterTiles.ToArray()[Random.Range(0, waterTiles.Count - 1)];

                GameObject boat = Instantiate(Resources.Load(Constants.prefabFolder + "boatyMacBootface") as GameObject, randomWaterTile.transform.position, Quaternion.identity);
                boat.AddComponent<BoatBehaviour>().Initialize(boat, randomWaterTile.GetRandomWaterNeighbour(), i);
                boats.Add(boat);
            }
        }

        // Set Trees
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                Tile tile = tiles[i, j];

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
        }
        // Set Wheat
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                Tile tile = tiles[i, j];

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
        }

        // Set Rocks
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                Tile tile = tiles[i, j];
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
}
