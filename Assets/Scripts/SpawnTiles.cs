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

    public float perlinNoiseHfScale = 1;
    public float perlinNoiseHfFactor = 0.2f;

    public float perlinNoiseLfScale = 0.1f;
    public float perlinNoiseLfFactor = 2;

    public float verticalOffset = 0.5f;

    public float snowLevel;
    public float beachLevel;

    public int seed;

    // Hexagon Vertice Count
    private const int HVC = 12;

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

            CreateMap(mapSize, mapSize, seed, true, true);
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

        int meshIndex = 0;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

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
                TileManager topLeftTileManager = null;
                TileManager topRightTileManager = null;
                TileManager lowerLeftTileManager = null;
                TileManager lowerRightTileManager = null;

                if ((y - 1) > 0)
                    leftTileManager = tileManagers[x, (y - 1)];

                if ((y + 1) < boardSizeY)
                    rightTileManager = tileManagers[x, (y + 1)];

                if ((x + 1) < boardSizeX && ((y - 1) + (x % 2)) > 0)
                    topLeftTileManager = tileManagers[(x + 1), (y - 1) + (x % 2)];

                if ((x + 1) < boardSizeX && (y + (x % 2)) < boardSizeY)
                    topRightTileManager = tileManagers[(x + 1), (y + (x % 2))];

                if ((x - 1) > 0 && ((y - 1) + (x % 2)) > 0)
                    lowerLeftTileManager = tileManagers[(x - 1), ((y - 1) + (x % 2))];

                if ((x - 1) > 0 && (y + (x % 2)) < boardSizeY)
                    lowerRightTileManager = tileManagers[(x - 1), (y + (x % 2))];

                if (leftTileManager) currentTileManager.leftTileManager = leftTileManager;
                if (rightTileManager) currentTileManager.rightTileManager = rightTileManager;
                if (topLeftTileManager) currentTileManager.topLeftTileManager = topLeftTileManager;
                if (topRightTileManager) currentTileManager.topRightTileManager = topRightTileManager;
                if (lowerLeftTileManager) currentTileManager.lowerLeftTileManager = lowerLeftTileManager;
                if (lowerRightTileManager) currentTileManager.lowerRightTileManager = lowerRightTileManager;
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

        foreach (TileManager tileManager in tileManagersList)
        {
            if (tileManager.gameObject.activeSelf == false)
                continue;

            activeTileManagers.Add(tileManager);

            if (tileManager.leftTileManager == null || tileManager.leftTileManager.gameObject.activeSelf == false ||
                tileManager.rightTileManager == null || tileManager.rightTileManager.gameObject.activeSelf == false ||
                tileManager.topLeftTileManager == null || tileManager.topLeftTileManager.gameObject.activeSelf == false ||
                tileManager.topRightTileManager == null || tileManager.topRightTileManager.gameObject.activeSelf == false ||
                tileManager.lowerLeftTileManager == null || tileManager.lowerLeftTileManager.gameObject.activeSelf == false ||
                tileManager.lowerRightTileManager == null || tileManager.lowerRightTileManager.gameObject.activeSelf == false)
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
                    float lfHeight =
                        Mathf.PerlinNoise((i * perlinNoiseLfScale) + perlinNoiseOffset, (j * perlinNoiseLfScale) + perlinNoiseOffset)
                            * perlinNoiseLfFactor - (perlinNoiseLfFactor / 2);

                    float hfHeight =
                        Mathf.PerlinNoise((i * perlinNoiseHfScale) + perlinNoiseOffset, (j * perlinNoiseHfScale) + perlinNoiseOffset)
                            * perlinNoiseHfFactor - (perlinNoiseHfFactor / 2);

                    float height = lfHeight + hfHeight + verticalOffset;

                    tileManagers[i, j].SetHeight(height);

                    if (height > snowLevel)
                    {
                        Material[] materials = tileManagers[i, j].meshRenderer.materials;
                        materials[1] = snow;
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
            }

        }

        if (waterTiles.Count != 0)
        {
            // Add boats
            for (int i = 0; i < Random.Range(minBoatCount, maxBoatCount); i++)
            {
                // Spawn boat at a random active water tile
                TileManager randomWaterTile = waterTiles.ToArray()[Random.Range(0, waterTiles.Count - 1)];

                GameObject boat = Instantiate(Resources.Load(Constants.prefabFolder + "boatyMacBootface") as GameObject, randomWaterTile.transform.position, Quaternion.identity);
                boat.AddComponent<BoatBehaviour>().Initialize(boat, randomWaterTile.GetRandomNeighboursHavingWater(), i);
                boats.Add(boat);
            }
        }

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                TileManager tileManager = tileManagers[x, y];

                if (!tileManager.gameObject.activeSelf)
                    continue;

                Vector3 position = tileManager.gameObject.transform.position;
                position.y -= 10;
                tileManager.gameObject.transform.position = position;

                tileManager.topTileMeshIndex = meshIndex;

                // Add the top surface
                for (int i = 0; i < HVC; i++)
                {
                    Vector3 topVertice = tileManager.topVertices[i];
                    topVertice.x += tileManager.xOffset;
                    topVertice.z += tileManager.zOffset;
                    topVertice.y = tileManager.height;

                    vertices.Add(topVertice);
                }

                for (int i = 0; i < 12; i++)
                {
                    triangles.Add(tileManager.topTriangles[i] + meshIndex * HVC);
                }

                meshIndex++;

                tileManager.lowerTileMeshIndex = meshIndex;

                // Add the bottom surface
                for (int i = 0; i < HVC; i++)
                {
                    Vector3 lowerVertice = tileManager.lowerVertices[i];
                    lowerVertice.x += tileManager.xOffset;
                    lowerVertice.z += tileManager.zOffset;
                    lowerVertice.y = tileManager.gameObject.transform.localScale.y * -1;

                    vertices.Add(lowerVertice);
                }

                for (int i = 0; i < 12; i++)
                {
                    triangles.Add(tileManager.lowerTriangles[i] + meshIndex * HVC);
                }

                meshIndex++;
            }
        }

        List<int> sides = new List<int>();

        int a = 0, b = 0, c = 0, d = 0, e = 0, f = 0;

        foreach (TileManager tileManager in activeTileManagers)
        {
            if (tileManager.leftTileManager != null
                && tileManager.leftTileManager.gameObject.activeSelf == true
                && !tileManager.connectedLeftTileMesh)
            {
                int topTileIndex = tileManager.topTileMeshIndex;
                int topNeighbourIndex = tileManager.leftTileManager.topTileMeshIndex;

                int lowerTileIndex = tileManager.lowerTileMeshIndex;
                int lowerNeighbourIndex = tileManager.leftTileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 11 + topTileIndex * HVC, 10 + topTileIndex * HVC, 8 + topNeighbourIndex * HVC });
                sides.AddRange(new int[] { 8 + topNeighbourIndex * HVC, 7 + topNeighbourIndex * HVC, 11 + topTileIndex * HVC });

                sides.AddRange(new int[] { 11 + lowerTileIndex * HVC, 8 + lowerTileIndex * HVC, 10 + lowerNeighbourIndex * HVC });
                sides.AddRange(new int[] { 8 + lowerNeighbourIndex * HVC, 11 + lowerNeighbourIndex * HVC, 7 + lowerTileIndex * HVC });

                tileManager.connectedLeftTileMesh = true;
                tileManager.leftTileManager.connectedRightTileMesh = true;
            }
            else if (tileManager.leftTileManager == null ||
                    (tileManager.leftTileManager != null && tileManager.leftTileManager.gameObject.activeSelf == false) &&
                    !tileManager.connectedLeftTileMesh)
            {
                int topTileIndex = tileManager.topTileMeshIndex;
                int lowerTileIndex = tileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 10 + topTileIndex * HVC, 10 + lowerTileIndex * HVC, 11 + topTileIndex * HVC });
                sides.AddRange(new int[] { 10 + lowerTileIndex * HVC, 11 + lowerTileIndex * HVC, 11 + topTileIndex * HVC });
            }

            if (tileManager.rightTileManager != null
                && tileManager.rightTileManager.gameObject.activeSelf == true
                && !tileManager.connectedRightTileMesh)
            {
                int topTileIndex = tileManager.topTileMeshIndex;
                int topNeighbourIndex = tileManager.rightTileManager.topTileMeshIndex;

                int lowerTileIndex = tileManager.lowerTileMeshIndex;
                int lowerNeighbourIndex = tileManager.rightTileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 7 + topTileIndex * HVC, 11 + topNeighbourIndex * HVC, 8 + topTileIndex * HVC });
                sides.AddRange(new int[] { 8 + topTileIndex * HVC, 11 + topNeighbourIndex * HVC, 10 + topNeighbourIndex * HVC });

                sides.AddRange(new int[] { 7 + lowerTileIndex * HVC, 8 + lowerTileIndex * HVC, 11 + lowerNeighbourIndex * HVC });
                sides.AddRange(new int[] { 8 + lowerTileIndex * HVC, 10 + lowerNeighbourIndex * HVC, 11 + lowerNeighbourIndex * HVC });

                tileManager.connectedRightTileMesh = true;
                tileManager.rightTileManager.connectedLeftTileMesh = true;
            }
            else if (tileManager.rightTileManager == null ||
                    (tileManager.rightTileManager != null && tileManager.rightTileManager.gameObject.activeSelf == false) &&
                    !tileManager.connectedRightTileMesh)
            {
                int topTileIndex = tileManager.topTileMeshIndex;
                int lowerTileIndex = tileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 7 + topTileIndex * HVC, 7 + lowerTileIndex * HVC, 8 + topTileIndex * HVC });
                sides.AddRange(new int[] { 8 + topTileIndex * HVC, 7 + lowerTileIndex * HVC, 8 + lowerTileIndex * HVC });
            }

            if (tileManager.topRightTileManager != null
                && tileManager.topRightTileManager.gameObject.activeSelf == true
                && !tileManager.connectedTopRightTileMesh)
            {
                int toptileIndex = tileManager.topTileMeshIndex;
                int topneighbourIndex = tileManager.topRightTileManager.topTileMeshIndex;

                int lowerTileIndex = tileManager.lowerTileMeshIndex;
                int lowerNeighbourIndex = tileManager.topRightTileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 6 + toptileIndex * HVC, 10 + topneighbourIndex * HVC, 7 + toptileIndex * HVC });
                sides.AddRange(new int[] { 7 + toptileIndex * HVC, 10 + topneighbourIndex * HVC, 9 + topneighbourIndex * HVC });

                sides.AddRange(new int[] { 6 + lowerTileIndex * HVC, 7 + lowerTileIndex * HVC, 10 + lowerNeighbourIndex * HVC });
                sides.AddRange(new int[] { 7 + lowerTileIndex * HVC, 9 + lowerNeighbourIndex * HVC, 10 + lowerNeighbourIndex * HVC });

                tileManager.connectedTopRightTileMesh = true;
                tileManager.topRightTileManager.connectedLowerLeftTileMesh = true;
            }
            else if (tileManager.topRightTileManager == null ||
                (tileManager.topRightTileManager != null && tileManager.topRightTileManager.gameObject.activeSelf == false) &&
                !tileManager.connectedTopRightTileMesh)
            {
                int topTileIndex = tileManager.topTileMeshIndex;
                int lowerTileIndex = tileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 6 + topTileIndex * HVC, 6 + lowerTileIndex * HVC, 7 + topTileIndex * HVC });
                sides.AddRange(new int[] { 7 + topTileIndex * HVC, 6 + lowerTileIndex * HVC, 7 + lowerTileIndex * HVC });
            }

            if (tileManager.topLeftTileManager != null
                && tileManager.topLeftTileManager.gameObject.activeSelf == true
                && !tileManager.connectedTopRightTileMesh)
            {
                int toptileIndex = tileManager.topTileMeshIndex;
                int topneighbourIndex = tileManager.topLeftTileManager.topTileMeshIndex;

                int lowerTileIndex = tileManager.lowerTileMeshIndex;
                int lowerNeighbourIndex = tileManager.topLeftTileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 6 + toptileIndex * HVC, 11 + toptileIndex * HVC, 9 + topneighbourIndex * HVC });
                sides.AddRange(new int[] { 6 + toptileIndex * HVC, 9 + topneighbourIndex * HVC, 8 + topneighbourIndex * HVC });

                sides.AddRange(new int[] { 6 + lowerTileIndex * HVC, 8 + lowerNeighbourIndex * HVC, 9 + lowerNeighbourIndex * HVC });
                sides.AddRange(new int[] { 6 + lowerTileIndex * HVC, 9 + lowerNeighbourIndex * HVC, 11 + lowerTileIndex * HVC });

                tileManager.connectedTopLeftTileMesh = true;
                tileManager.topLeftTileManager.connectedLowerRightTileMesh = true;
            }
            else if (tileManager.topLeftTileManager == null ||
                    (tileManager.topLeftTileManager != null && tileManager.topLeftTileManager.gameObject.activeSelf == false) &&
                    !tileManager.connectedTopLeftTileMesh)
            {
                int topTileIndex = tileManager.topTileMeshIndex;
                int lowerTileIndex = tileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 6 + topTileIndex * HVC, 11 + topTileIndex * HVC, 11 + lowerTileIndex * HVC });
                sides.AddRange(new int[] { 11 + lowerTileIndex * HVC, 6 + lowerTileIndex * HVC, 6 + topTileIndex * HVC });

                tileManager.connectedTopLeftTileMesh = true;
            }

            if (tileManager.lowerRightTileManager != null
                && tileManager.lowerRightTileManager.gameObject.activeSelf == true
                && !tileManager.connectedLowerRightTileMesh)
            {
                int toptileIndex = tileManager.topTileMeshIndex;
                int topneighbourIndex = tileManager.lowerRightTileManager.topTileMeshIndex;

                int lowerTileIndex = tileManager.lowerTileMeshIndex;
                int lowerNeighbourIndex = tileManager.lowerRightTileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 8 + toptileIndex * HVC, 6 + topneighbourIndex * HVC, 9 + toptileIndex * HVC });
                sides.AddRange(new int[] { 6 + topneighbourIndex * HVC, 11 + topneighbourIndex * HVC, 9 + toptileIndex * HVC });

                sides.AddRange(new int[] { 8 + lowerTileIndex * HVC, 9 + lowerTileIndex * HVC, 6 + lowerNeighbourIndex * HVC });
                sides.AddRange(new int[] { 9 + lowerTileIndex * HVC, 11 + lowerNeighbourIndex * HVC, 6 + lowerNeighbourIndex * HVC });

                tileManager.connectedLowerRightTileMesh = true;
                tileManager.lowerRightTileManager.connectedTopLeftTileMesh = true;
            }
            else if (tileManager.lowerRightTileManager == null ||
                    (tileManager.lowerRightTileManager != null && tileManager.lowerRightTileManager.gameObject.activeSelf == false) &&
                    !tileManager.connectedLowerRightTileMesh)
            {
                int topTileIndex = tileManager.topTileMeshIndex;
                int lowerTileIndex = tileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 8 + topTileIndex * HVC, 8 + lowerTileIndex * HVC, 9 + topTileIndex * HVC });
                sides.AddRange(new int[] { 9 + topTileIndex * HVC, 8 + lowerTileIndex * HVC, 9 + lowerTileIndex * HVC });

                tileManager.connectedLowerRightTileMesh = true;
            }

            if (tileManager.lowerLeftTileManager != null
                && tileManager.lowerLeftTileManager.gameObject.activeSelf == true
                && !tileManager.connectedLowerLeftTileMesh)
            {
                int toptileIndex = tileManager.topTileMeshIndex;
                int topneighbourIndex = tileManager.lowerLeftTileManager.topTileMeshIndex;

                int lowerTileIndex = tileManager.lowerTileMeshIndex;
                int lowerNeighbourIndex = tileManager.lowerLeftTileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 9 + toptileIndex * HVC, 7 + topneighbourIndex * HVC, 10 + toptileIndex * HVC });
                sides.AddRange(new int[] { 7 + topneighbourIndex * HVC, 6 + topneighbourIndex * HVC, 10 + toptileIndex * HVC });

                sides.AddRange(new int[] { 8 + lowerTileIndex * HVC, 9 + lowerTileIndex * HVC, 6 + lowerNeighbourIndex * HVC });
                sides.AddRange(new int[] { 9 + lowerTileIndex * HVC, 11 + lowerNeighbourIndex * HVC, 6 + lowerNeighbourIndex * HVC });

                tileManager.connectedLowerLeftTileMesh = true;
                tileManager.lowerLeftTileManager.connectedTopRightTileMesh = true;
            } 
            else if(tileManager.lowerLeftTileManager == null ||
                    (tileManager.lowerLeftTileManager != null && tileManager.lowerLeftTileManager.gameObject.activeSelf == false) &&
                    !tileManager.connectedLowerLeftTileMesh)
            {
                int topTileIndex = tileManager.topTileMeshIndex;
                int lowerTileIndex = tileManager.lowerTileMeshIndex;

                sides.AddRange(new int[] { 9 + topTileIndex * HVC, 9 + lowerTileIndex * HVC, 10 + topTileIndex * HVC });
                sides.AddRange(new int[] { 9 + lowerTileIndex * HVC, 10 + lowerTileIndex * HVC, 10 + topTileIndex * HVC });
            }

        }

        Debug.Log(a + " " + b + " " + c + " " + d + " " + e + " " + f);

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.subMeshCount = 2;

        mesh.SetTriangles(triangles, 0);
        mesh.SetTriangles(sides, 1);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        tilesContainer.GetComponent<MeshFilter>().mesh = mesh;

        List<Material> meshMaterials = new List<Material>();
        meshMaterials.Add(Resources.Load(Constants.materialsFolder + "Grass1", typeof(Material)) as Material);
        meshMaterials.Add(Resources.Load(Constants.materialsFolder + "Earth", typeof(Material)) as Material);

        tilesContainer.GetComponent<MeshRenderer>().materials = meshMaterials.ToArray();

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

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                tileManagers[x, y].gameObject.SetActive(false);
            }
        }
    }
}
