using System.Collections.Generic;
using UnityEngine;

public class SpawnTiles : MonoBehaviour
{
    private const int x = 60;
    private const int y = 60;

    private const float newWaterProbability = 0.007f;
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

    private const float maxTileHealth = 100f;
    private const float tileHealthSlope = 5f;

    private const int minBoatCount = 1;
    private const int maxBoatCount = 15;

    private Tile[] tiles;
    private List<Tile> waterTiles;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new Tile[x * y];
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
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject newTile;
                
                newTile = Instantiate(Resources.Load(Constants.prefabFolder + "Hex Parent") as GameObject, new Vector3(j * 1.7f + (i % 2 * 0.85f), 0, i * 1.5f), Quaternion.identity);
                newTile.name = "Tile" + i + "-" + j;

                GameObject hexagonGameObject = newTile.transform.Find("Hexagon").gameObject;

                int grassKind = Mathf.RoundToInt(Random.Range(1f, 3f));
                Material[] tileMats = hexagonGameObject.transform.GetComponent<Renderer>().materials;
                tileMats[1] = Resources.Load(Constants.materialsFolder + "Grass" + grassKind, typeof(Material)) as Material;
                newTile.transform.Find("Hexagon").GetComponent<Renderer>().materials = tileMats;

                // Attach tile script
                tiles[tileCounter] = newTile.AddComponent<Tile>(); 
                tiles[tileCounter].setInitialValues(i, j, newTile, hexagonGameObject);

                tileCounter++;
            }
        }

        // Connect tiles 
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
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
        GameObject.Find("Tile" + Mathf.RoundToInt(x / 2) + "-" + Mathf.RoundToInt(y / 2)).GetComponent<Tile>().setHealth(maxTileHealth, tileHealthSlope);
        for (int i = 0; i < x * y; i++)
        {
            tiles[i].checkHealth();
        }

        // Set Water
        for (int i = 0; i < x * y; i++)
        {
            Tile tile = tiles[i];

            float waterProbability = newWaterProbability + (adjacentWaterProbability * tile.neighboursWaterCount());

            if (Random.Range(0f, 1f) < waterProbability && tile.isActive)
            {
                tile.tileGameObject.transform.Find("Hexagon").GetComponent<Renderer>().materials = waterTile;
                tile.isWater = true;
                waterTiles.Add(tile);
            }
        }

        // Add boats
        for (int i = 0; i < Random.Range(minBoatCount, maxBoatCount); i++)
        {
            // Spawn boat at a random active water tile
            Tile randomWaterTile = waterTiles.ToArray()[Random.Range(0, waterTiles.Count - 1)];

            GameObject boat = Instantiate(Resources.Load(Constants.prefabFolder + "boatyMacBootface") as GameObject, randomWaterTile.transform.position, Quaternion.identity);
            boat.AddComponent<BoatBehaviour>().Initialize(boat, randomWaterTile.getRandomWaterNeighbour(), i);
        }

        // Add Mountains
        int mountainCount = Mathf.RoundToInt(Random.Range(minMountainCount, maxMountainCount));
        Debug.Log("Attempting to add " + mountainCount + " mountains");
        
        for (int i = 0; i < mountainCount; i++)
        {
            tiles[Mathf.RoundToInt(Random.Range(0, x * y))].setHeight(Random.Range(0, maxMountainHeight), Random.Range(minMountainSlope, maxMountainSlope));
        }
        
        // Set Trees
        for (int i = 0; i < x * y; i++)
        {
            Tile tile = tiles[i];

            float treeProbability = newTreeProbability + (adjacentTreeProbability * tile.neighboursTreeCount());

            if (!tile.isWater && Random.Range(0f, 1f) < treeProbability)
            {
                tile.tileGameObject.transform.Find("Hexagon").GetComponent<Renderer>().materials = earthTile;
                tile.isForest = true;

                GameObject newTree;
                string treeToSpawn;
                int neighboursTreeCount = tile.neighboursTreeCount();

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
            }
        }

        // Set Wells
        for (int i = 0; i < x * y; i++)
        {
            Tile tile = tiles[i];

            if (!tile.isWater && !tile.isForest && Random.Range(0f, 1f) < wellProbability)
            {
                GameObject well;
                well = Instantiate(Resources.Load(Constants.prefabFolder + "Well") as GameObject, tile.tileGameObject.transform.position, Quaternion.identity);
                well.transform.parent = tile.tileGameObject.transform;
                well.name = "Well";
            }
        }

        // Set Wheat
        for (int i = 0; i < x * y; i++)
        {
            Tile tile = tiles[i];

            if (!tile.isWater && !tile.isForest && !tile.isWell && Random.Range(0f, 1f) < wheatProbability)
            {
                tile.tileGameObject.transform.Find("Hexagon").GetComponent<Renderer>().materials = earthTile;
                tile.isWheat = true;

                GameObject WheatParent;
                WheatParent = Instantiate(Resources.Load(Constants.prefabFolder + "WheatParent") as GameObject, tile.tileGameObject.transform.position, Quaternion.identity);
                WheatParent.transform.parent = tile.tileGameObject.transform;
                WheatParent.name = "WheatParent";
            }
        }

        // Set Rocks
        for (int i = 0; i < x * y; i++)
        {
            Tile tile = tiles[i];
            float RockProbability = newRockProbability + (adjacentRockProbability * tile.neighboursRockCount());

            if (!tile.isWater && !tile.isForest && !tile.isWell && !tile.isWheat && Random.Range(0f, 1f) < RockProbability)
            {
                tile.tileGameObject.transform.Find("Hexagon").GetComponent<Renderer>().materials = stoneTile;
                tile.isRock = true;
                
                GameObject Rock;
                string rockToSpawn;
                int neighboursRockCount = tile.neighboursRockCount();

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

                Rock = Instantiate(Resources.Load(Constants.prefabFolder + rockToSpawn) as GameObject, tile.tileGameObject.transform.position, Quaternion.identity);
                Rock.transform.parent = tile.tileGameObject.transform;
                Rock.name = "Rock";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
