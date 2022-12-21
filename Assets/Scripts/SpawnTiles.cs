using System.Collections.Generic;
using UnityEngine;

public class SpawnTiles : MonoBehaviour
{
    private const float horizontalTileOffset = 1.732f;
    private const float verticalTileOffset = 1.5f;

    private const float newTreeProbability = 0.015f;
    private const float adjacentTreeProbability = 0.20f;

    private const float wellProbability = 0.0025f;
    private const float wheatProbability = 0.008f;

    private const float newRockProbability = 0.010f;
    private const float adjacentRockProbability = 0.20f;

    private const int minBoatCount = 1;
    private const int maxBoatCount = 15;

    public TileManager[,] tileManagers;
    public List<TileManager> tileManagersList;

    public List<TileManager> activeTileManagers;
    public List<TileManager> edgeTileManagers;
    public List<TileManager> waterTiles;
    public List<GameObject> boats = new List<GameObject>();
    public List<GameObject> tileContainers = new List<GameObject>();

    public bool recreateMap = false;

    public bool customBreak = false;

    public int mapSize = 40;

    public bool roundishShape = true;
    public bool mountains = true;

    public float perlinNoiseHfScale = 1;
    public float perlinNoiseHfFactor = 0.2f;

    public float perlinNoiseLfScale = 0.1f;
    public float perlinNoiseLfFactor = 2;

    public float perlinNoiseULfScale = 0.01f;
    public float perlinNoiseULfFactor = 10;

    public float verticalOffset = 0.5f;

    public float snowLevel;
    public float beachLevel;

    public int seed;

    public void Update()
    {
        if (recreateMap)
        {
            recreateMap = false;

            if (tileManagers != null)
            {
                foreach (GameObject tile in tileContainers)
                    Destroy(tile);

                tileContainers.Clear();

                foreach (GameObject boat in boats)
                    Destroy(boat);

                boats.Clear();

                tileManagersList.Clear();

                activeTileManagers.Clear();
                edgeTileManagers.Clear();
                waterTiles.Clear();
            }

            CreateMap(mapSize, mapSize, seed, roundishShape, mountains);
        }
    }

    public void CreateMap(int boardSizeX, int boardSizeY, int seed, bool roundishShape, bool mountains)
    {
        if (customBreak)
            return;

        GameObject tilesContainer = GameObject.Find("Tiles");

        Random.InitState(seed);

        float perlinNoiseOffset = Random.Range(-1000f, 1000f);

        tileManagers = new TileManager[boardSizeX, boardSizeY];
        waterTiles = new List<TileManager>();

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
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                // Every second line of tiles needs to be shifted by half the horizontal tile 
                // offset to make the hexagons match.

                float xOffset = y * horizontalTileOffset + (x % 2 * (horizontalTileOffset / 2));
                float zOffset = x * verticalTileOffset;

                GameObject newTile = Instantiate(Resources.Load(Constants.prefabFolder + "Hexagon") as GameObject,
                    new Vector3(xOffset, 0, zOffset), Quaternion.identity);

                newTile.GetComponent<MeshRenderer>().enabled = false;

                // Attach tile script
                TileManager tileManager = newTile.AddComponent<TileManager>();
                tileManagers[x, y] = tileManager;
                tileManagers[x, y].SetInitialValues(x, y, newTile.GetComponent<MeshRenderer>());

                tileManager.xOffset = xOffset;
                tileManager.zOffset = zOffset;

                tileContainers.Add(newTile);

                newTile.name = "Tile" + x + "-" + y;
                newTile.transform.parent = tilesContainer.transform;

                int grassKind = Mathf.RoundToInt(Random.Range(1f, 3f));
                Material[] tileMats = tileManager.meshRenderer.materials;
                tileMats[1] = Resources.Load(Constants.materialsFolder + "Grass" + grassKind, typeof(Material)) as Material;
                newTile.GetComponent<Renderer>().materials = tileMats;

                if (grassKind == 1)
                    tileManager.isGrass1 = true;
                else if (grassKind == 2)
                    tileManager.isGrass2 = true;
                else if (grassKind == 3)
                    tileManager.isGrass3 = true;

                tileManagersList.Add(tileManager);
            }
        }

        // Connect tiles 
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                TileManager currentTileManager = tileManagers[x, y];

                TileManager leftTileManager = null;
                TileManager rightTileManager = null;
                TileManager frontLeftTileManager = null;
                TileManager frontRightTileManager = null;
                TileManager backLeftTileManager = null;
                TileManager backRightTileManager = null;

                if ((y - 1) > 0)
                    leftTileManager = tileManagers[x, (y - 1)];

                if ((y + 1) < boardSizeY)
                    rightTileManager = tileManagers[x, (y + 1)];

                if ((x + 1) < boardSizeX && ((y - 1) + (x % 2)) > 0)
                    frontLeftTileManager = tileManagers[(x + 1), (y - 1) + (x % 2)];

                if ((x + 1) < boardSizeX && (y + (x % 2)) < boardSizeY)
                    frontRightTileManager = tileManagers[(x + 1), (y + (x % 2))];

                if ((x - 1) > 0 && ((y - 1) + (x % 2)) > 0)
                    backLeftTileManager = tileManagers[(x - 1), ((y - 1) + (x % 2))];

                if ((x - 1) > 0 && (y + (x % 2)) < boardSizeY)
                    backRightTileManager = tileManagers[(x - 1), (y + (x % 2))];

                if (leftTileManager) currentTileManager.leftTileManager = leftTileManager;
                if (rightTileManager) currentTileManager.rightTileManager = rightTileManager;
                if (frontLeftTileManager) currentTileManager.frontLeftTileManager = frontLeftTileManager;
                if (frontRightTileManager) currentTileManager.frontRightTileManager = frontRightTileManager;
                if (backLeftTileManager) currentTileManager.backLeftTileManager = backLeftTileManager;
                if (backRightTileManager) currentTileManager.backRightTileManager = backRightTileManager;
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

                    tileManagers[x, y].SetHealth(
                        (boardSizeX / 2 + boardSizeY / 2)
                        / 2
                        - distanceToCenter
                        - 0.5f
                        - Random.Range(0f, 1f));

                    tileManagers[x, y].CheckHealth();
                }
            }
        }

        // Classify tiles into active and edges
        foreach (TileManager tileManager in tileManagersList)
        {
            if (tileManager.gameObject.activeSelf == false)
                continue;

            activeTileManagers.Add(tileManager);

            if (tileManager.leftTileManager == null || tileManager.leftTileManager.gameObject.activeSelf == false ||
                tileManager.rightTileManager == null || tileManager.rightTileManager.gameObject.activeSelf == false ||
                tileManager.frontLeftTileManager == null || tileManager.frontLeftTileManager.gameObject.activeSelf == false ||
                tileManager.frontRightTileManager == null || tileManager.frontRightTileManager.gameObject.activeSelf == false ||
                tileManager.backLeftTileManager == null || tileManager.backLeftTileManager.gameObject.activeSelf == false ||
                tileManager.backRightTileManager == null || tileManager.backRightTileManager.gameObject.activeSelf == false)
            {
                tileManager.isEdgeTile = true;
                edgeTileManagers.Add(tileManager);
            }
        }

        // Add Mountains
        if (mountains)
        {
            Material snow = Resources.Load(Constants.materialsFolder + "Snow", typeof(Material)) as Material;

            for (int i = 0; i < boardSizeX; i++)
            {
                for (int j = 0; j < boardSizeY; j++)
                {
                    float ulfHeight =
                        Mathf.PerlinNoise((i * perlinNoiseULfScale) + perlinNoiseOffset, (j * perlinNoiseULfScale) + perlinNoiseOffset)
                            * perlinNoiseULfFactor - (perlinNoiseULfFactor / 2);

                    float lfHeight =
                        Mathf.PerlinNoise((i * perlinNoiseLfScale) + perlinNoiseOffset, (j * perlinNoiseLfScale) + perlinNoiseOffset)
                            * perlinNoiseLfFactor - (perlinNoiseLfFactor / 2);

                    float hfHeight =
                        Mathf.PerlinNoise((i * perlinNoiseHfScale) + perlinNoiseOffset, (j * perlinNoiseHfScale) + perlinNoiseOffset)
                            * perlinNoiseHfFactor - (perlinNoiseHfFactor / 2);

                    float height = ulfHeight + lfHeight + hfHeight + verticalOffset;

                    tileManagers[i, j].SetHeight(height);

                    if (height > snowLevel)
                    {
                        Material[] materials = tileManagers[i, j].meshRenderer.materials;
                        materials[1] = snow;
                        tileManagers[i, j].isSnow = true;
                        tileManagers[i, j].meshRenderer.materials = materials;
                    }
                }
            }
        }

        // Set Water
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                TileManager tileManager = tileManagers[x, y];

                if (tileManager.GetHeight() < 0 && tileManager.isActive)
                {
                    tileManager.SetHeight(0);

                    tileManager.gameObject.transform.GetComponent<Renderer>().materials = waterTile;
                    tileManager.isWater = true;
                    waterTiles.Add(tileManager);
                }
            }
        }

        // Set Beach
        Material beach = Resources.Load(Constants.materialsFolder + "Beach", typeof(Material)) as Material;

        foreach (TileManager tileManager in tileManagersList)
        {
            if (tileManager.isWater)
                continue;

            if (tileManager.NeighboursWaterCount() > 0 && tileManager.height < beachLevel)
            {
                Material[] materials = tileManager.meshRenderer.materials;
                materials[1] = beach;
                tileManager.meshRenderer.materials = materials;
                tileManager.isSand = true;
            }

        }

        // Add boats
        if (waterTiles.Count != 0)
        {
            for (int i = 0; i < Random.Range(minBoatCount, maxBoatCount); i++)
            {
                // Spawn boat at a random active water tile
                TileManager randomWaterTile = waterTiles.ToArray()[Random.Range(0, waterTiles.Count - 1)];

                GameObject boat = Instantiate(Resources.Load(Constants.prefabFolder + "boatyMacBootface") as GameObject, randomWaterTile.transform.position, Quaternion.identity);
                boat.AddComponent<BoatBehaviour>().Initialize(boat, randomWaterTile.GetRandomNeighboursHavingWater(), i);
                boats.Add(boat);
            }
        }

        // Set Trees
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                TileManager tileManager = tileManagers[x, y];

                float treeProbability = newTreeProbability + (adjacentTreeProbability * tileManager.GetNeighboursWoodCount());

                if (tileManager.isActive && !tileManager.isWater && Random.Range(0f, 1f) < treeProbability)
                {
                    tileManager.meshRenderer.materials = earthTile;

                    GameObject newTree;
                    string treeToSpawn;
                    int neighboursTreeCount = tileManager.GetNeighboursWoodCount();

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

                    newTree = Instantiate(Resources.Load(Constants.prefabFolder + treeToSpawn) as GameObject, tileManager.gameObject.transform.position, Quaternion.identity); //Vector3(j * 1.7f + (i % 2 * 0.85f), 0, i * 1.5f)
                    newTree.transform.parent = tileManager.gameObject.transform;
                    newTree.name = "Tree";

                    tileManager.wood = newTree;
                }
            }
        }

        // Set Wheat
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                TileManager tileManager = tileManagers[x, y];

                if (tileManager.isActive && !tileManager.isWater && !tileManager.wood && !tileManager.well && Random.Range(0f, 1f) < wheatProbability)
                {
                    tileManager.meshRenderer.materials = earthTile;

                    GameObject WheatParent;
                    WheatParent = Instantiate(Resources.Load(Constants.prefabFolder + "WheatParent") as GameObject, tileManager.gameObject.transform.position, Quaternion.identity);
                    WheatParent.transform.parent = tileManager.gameObject.transform;
                    WheatParent.name = "WheatParent";

                    tileManager.wheat = WheatParent;
                }
            }
        }

        // Set Rocks
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                TileManager tileManager = tileManagers[x, y];
                float RockProbability = newRockProbability + (adjacentRockProbability * tileManager.GetNeighboursStoneCount());

                if (tileManager.isActive && !tileManager.isWater && !tileManager.wood && !tileManager.well && !tileManager.wheat && Random.Range(0f, 1f) < RockProbability)
                {
                    tileManager.meshRenderer.materials = stoneTile;

                    GameObject rock;
                    string rockToSpawn;
                    int neighboursRockCount = tileManager.GetNeighboursStoneCount();

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

                    rock = Instantiate(Resources.Load(Constants.prefabFolder + rockToSpawn) as GameObject, tileManager.gameObject.transform.position, Quaternion.identity);
                    rock.transform.parent = tileManager.gameObject.transform;
                    rock.name = "Rock";

                    tileManager.rock = rock;
                }
            }
        }

        // Generate a mesh from all the data we gathered so far
        tilesContainer.GetComponent<MeshFilter>().mesh = GenerateMesh.Generate(boardSizeX, boardSizeY, tileManagers, activeTileManagers);

        List<Material> meshMaterials = new List<Material>();
        meshMaterials.Add(Resources.Load(Constants.materialsFolder + "Earth", typeof(Material)) as Material);
        meshMaterials.Add(Resources.Load(Constants.materialsFolder + "Water", typeof(Material)) as Material);
        meshMaterials.Add(Resources.Load(Constants.materialsFolder + "Beach", typeof(Material)) as Material);
        meshMaterials.Add(Resources.Load(Constants.materialsFolder + "Snow", typeof(Material)) as Material);
        meshMaterials.Add(Resources.Load(Constants.materialsFolder + "Stone", typeof(Material)) as Material);
        meshMaterials.Add(Resources.Load(Constants.materialsFolder + "Grass1", typeof(Material)) as Material);
        meshMaterials.Add(Resources.Load(Constants.materialsFolder + "Grass2", typeof(Material)) as Material);
        meshMaterials.Add(Resources.Load(Constants.materialsFolder + "Grass3", typeof(Material)) as Material);

        tilesContainer.GetComponent<MeshRenderer>().materials = meshMaterials.ToArray();
    }
}
