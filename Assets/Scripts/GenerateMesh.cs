using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerateMesh
{
    // Hexagon Vertice Count
    private const int HexagonVertexOffset = 12;
    public static Mesh Generate(int boardSizeX, int boardSizeY, 
        TileManager[,] tileManagers, 
        List<TileManager> activeTileManagers)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> dirtTriangles = new List<int>();
        List<int> waterTriangles = new List<int>();
        List<int> sandTriangles = new List<int>();
        List<int> snowTriangles = new List<int>();
        List<int> rockTriangles = new List<int>();
        List<int> grass1Triangles = new List<int>();
        List<int> grass2Triangles = new List<int>();
        List<int> grass3Triangles = new List<int>();

        int meshIndex = 0;
    
        // Generate all the top and bottom faces
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                TileManager tileManager = tileManagers[x, y];

                // For active tiles
                if (!tileManager.gameObject.activeSelf)
                    continue;

                Vector3 position = tileManager.gameObject.transform.position;
                tileManager.gameObject.transform.position = position;
                tileManager.topTileMeshIndex = meshIndex;

                // Add the top surface
                for (int i = 0; i < HexagonVertexOffset; i++)
                {
                    Vector3 topVertice = Hexagon.topVertices[i];
                    topVertice.x += tileManager.xOffset;
                    topVertice.z += tileManager.zOffset;
                    topVertice.y = tileManager.height;

                    vertices.Add(topVertice);
                }

                // Sort the top faces into the right list for submesh generation
                for (int i = 0; i < 12; i++)
                {
                    if (tileManager.isWater)
                        waterTriangles.Add(Hexagon.topTriangles[i] + meshIndex * HexagonVertexOffset);
                    else if (tileManager.isSand)
                        sandTriangles.Add(Hexagon.topTriangles[i] + meshIndex * HexagonVertexOffset);
                    else if (tileManager.isSnow)
                        snowTriangles.Add(Hexagon.topTriangles[i] + meshIndex * HexagonVertexOffset);
                    else if (tileManager.rock)
                        rockTriangles.Add(Hexagon.topTriangles[i] + meshIndex * HexagonVertexOffset);
                    else if (tileManager.isGrass1)
                        grass1Triangles.Add(Hexagon.topTriangles[i] + meshIndex * HexagonVertexOffset);
                    else if (tileManager.isGrass2)
                        grass2Triangles.Add(Hexagon.topTriangles[i] + meshIndex * HexagonVertexOffset);
                    else if (tileManager.isGrass3)
                        grass3Triangles.Add(Hexagon.topTriangles[i] + meshIndex * HexagonVertexOffset);
                }

                meshIndex++;

                tileManager.lowerTileMeshIndex = meshIndex;

                // Add the bottom surface
                for (int i = 0; i < HexagonVertexOffset; i++)
                {
                    Vector3 lowerVertice = Hexagon.lowerVertices[i];
                    lowerVertice.x += tileManager.xOffset;
                    lowerVertice.z += tileManager.zOffset;

                    // -1 because the scale is positive, but we want to bottom tile to go towards -y
                    // *2 because of the pivot of the tile, that is at the top. *1 would only go half way down
                    lowerVertice.y = tileManager.gameObject.transform.localScale.y * -1 * 2;

                    vertices.Add(lowerVertice);
                }

                for (int i = 0; i < 12; i++)
                {
                    dirtTriangles.Add(Hexagon.lowerTriangles[i] + meshIndex * HexagonVertexOffset);
                }

                meshIndex++;
            }
        }

        // Some statistics on connections
        int leftConnections = 0;
        int rightConnections = 0;
        int frontRightConnections = 0;
        int frontLeftConnections = 0;
        int backRightConnections = 0;
        int backLeftConnections = 0;

        int leftBottomConnections = 0;
        int rightBottomConnections = 0;
        int frontRightBottomConnections = 0;
        int frontLeftBottomConnections = 0;
        int backRightBottomConnections = 0;
        int backLeftBottomConnections = 0;

        // Generate the sides (Tile to tile connections and world sides)
        // Note that the sides are not using the same vertices as the top and bottom faces to avoid shading issues
        foreach (TileManager tileManager in activeTileManagers)
        {
            // Connect to the left
            if (tileManager.leftTileManager != null
                && tileManager.leftTileManager.gameObject.activeSelf == true
                && !tileManager.connectedLeftTileMesh)
            {
                // This code is never executed because of the way we iterate over the array since the side already gets generated by 
                // Top neighbour tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int topNeighbourIndex = tileManager.leftTileManager.topTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFrontLeft + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackRight + topNeighbourIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBackRight + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontRight + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + topTileIndex * HexagonVertexOffset });

                // Bottom neighbour tile
                int bottomTileIndex = tileManager.lowerTileMeshIndex;
                int bottomNeighbourIndex = tileManager.leftTileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFrontLeft + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontRight + bottomNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + bottomTileIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFrontRight + bottomNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBackRight + bottomNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + bottomTileIndex * HexagonVertexOffset });

                tileManager.connectedLeftTileMesh = true;
                tileManager.leftTileManager.connectedRightTileMesh = true;

                leftConnections++;
            }
            else if (tileManager.leftTileManager == null ||
                    (tileManager.leftTileManager != null && tileManager.leftTileManager.gameObject.activeSelf == false) &&
                    !tileManager.connectedLeftTileMesh)
            {
                // No neighbour to the left. Connect to the bottom tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int bottomTileIndex = tileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFrontLeft + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + bottomTileIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFrontLeft + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + bottomTileIndex * HexagonVertexOffset });

                leftBottomConnections++;
            }
            
            // Connect to the right
            if (tileManager.rightTileManager != null
                && tileManager.rightTileManager.gameObject.activeSelf == true
                && !tileManager.connectedRightTileMesh)
            {
                // Top neighbour tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int topNeighbourIndex = tileManager.rightTileManager.topTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFrontRight + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBackRight + topTileIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBackRight + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + topNeighbourIndex * HexagonVertexOffset });

                // Bottom neighbour tile
                int bottomTileIndex = tileManager.lowerTileMeshIndex;
                int bottomNeighbourIndex = tileManager.rightTileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFrontRight + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackRight + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + bottomNeighbourIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBackRight + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + bottomNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + bottomNeighbourIndex * HexagonVertexOffset });

                tileManager.connectedRightTileMesh = true;
                tileManager.rightTileManager.connectedLeftTileMesh = true;

                rightConnections++;
            }
            else if (tileManager.rightTileManager == null ||
                    (tileManager.rightTileManager != null && tileManager.rightTileManager.gameObject.activeSelf == false) &&
                    !tileManager.connectedRightTileMesh)
            {
                // No neighbour to the right. Connect to the bottom tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int bottomTileIndex = tileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFrontRight + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontRight + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackRight + topTileIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBackRight + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontRight + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackRight + bottomTileIndex * HexagonVertexOffset });

                rightBottomConnections++;
            }
            
            
            // Connect to the front right
            if (tileManager.frontRightTileManager != null
                && tileManager.frontRightTileManager.gameObject.activeSelf == true
                && !tileManager.connectedFrontRightTileMesh)
            {
                // Top neighbour tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int topNeighbourIndex = tileManager.frontRightTileManager.topTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFront + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontRight + topTileIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFrontRight + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + topNeighbourIndex * HexagonVertexOffset });

                // Bottom neighbour tile
                int bottomTileIndex = tileManager.lowerTileMeshIndex;
                int bottomNeighbourIndex = tileManager.frontRightTileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFront + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontRight + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + bottomNeighbourIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFrontRight + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + bottomNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + bottomNeighbourIndex * HexagonVertexOffset });

                tileManager.connectedFrontRightTileMesh = true;
                tileManager.frontRightTileManager.connectedBackLeftTileMesh = true;

                frontRightConnections++;
            }
            else if (tileManager.frontRightTileManager == null ||
                (tileManager.frontRightTileManager != null && tileManager.frontRightTileManager.gameObject.activeSelf == false) &&
                !tileManager.connectedFrontRightTileMesh)
            {
                // No neighbour to the front right. Connect to the bottom tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int bottomTileIndex = tileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { Hexagon.SideFront + topTileIndex * HexagonVertexOffset, Hexagon.SideFront + bottomTileIndex * HexagonVertexOffset, Hexagon.SideFrontRight + topTileIndex * HexagonVertexOffset });
                dirtTriangles.AddRange(new int[] { Hexagon.SideFrontRight + topTileIndex * HexagonVertexOffset, Hexagon.SideFront + bottomTileIndex * HexagonVertexOffset, Hexagon.SideFrontRight + bottomTileIndex * HexagonVertexOffset });

                frontRightBottomConnections++;
            }
            
            
            // Connect to the front left
            if (tileManager.frontLeftTileManager != null
                && tileManager.frontLeftTileManager.gameObject.activeSelf == true
                && !tileManager.connectedFrontRightTileMesh)
            {
                // Top neighbour tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int topNeighbourIndex = tileManager.frontLeftTileManager.topTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFront + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + topNeighbourIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFront + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBackRight + topNeighbourIndex * HexagonVertexOffset });

                // Bottom neighbour tile
                int bottomTileIndex = tileManager.lowerTileMeshIndex;
                int bottomNeighbourIndex = tileManager.frontLeftTileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFront + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackRight + bottomNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + bottomNeighbourIndex * HexagonVertexOffset });
                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFront + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + bottomNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + bottomTileIndex * HexagonVertexOffset });

                tileManager.connectedFrontLeftTileMesh = true;
                tileManager.frontLeftTileManager.connectedBackRightTileMesh = true;

                frontLeftConnections++;
            }
            else if (tileManager.frontLeftTileManager == null ||
                    (tileManager.frontLeftTileManager != null && tileManager.frontLeftTileManager.gameObject.activeSelf == false) &&
                    !tileManager.connectedFrontLeftTileMesh)
            {
                // No neighbour to the front left. Connect to the bottom tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int bottomTileIndex = tileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFront + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + bottomTileIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFrontLeft + bottomTileIndex * HexagonVertexOffset,
                    Hexagon.SideFront + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFront + topTileIndex * HexagonVertexOffset });

                tileManager.connectedFrontLeftTileMesh = true;

                frontLeftBottomConnections++;
            }
            
            
            // Connect to the back right
            if (tileManager.backRightTileManager != null
                && tileManager.backRightTileManager.gameObject.activeSelf == true
                && !tileManager.connectedBackRightTileMesh)
            {
                // Top neighbour tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int topNeighbourIndex = tileManager.backRightTileManager.topTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBackRight + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFront + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + topTileIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideFront + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + topTileIndex * HexagonVertexOffset });

                // Bottom neighbour tile
                int bottomTileIndex = tileManager.lowerTileMeshIndex;
                int bottomNeighbourIndex = tileManager.backRightTileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBackRight + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFront + bottomNeighbourIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBack + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontLeft + bottomNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideFront + bottomNeighbourIndex * HexagonVertexOffset });

                tileManager.connectedBackRightTileMesh = true;
                tileManager.backRightTileManager.connectedFrontLeftTileMesh = true;

                backRightConnections++;
            }
            else if (tileManager.backRightTileManager == null ||
                    (tileManager.backRightTileManager != null && tileManager.backRightTileManager.gameObject.activeSelf == false) &&
                    !tileManager.connectedBackRightTileMesh)
            {
                // No neighbour to the back right. Connect to the bottom tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int bottomTileIndex = tileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBackRight + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackRight + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + topTileIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBack + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackRight + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + bottomTileIndex * HexagonVertexOffset });

                tileManager.connectedBackRightTileMesh = true;

                backRightBottomConnections++;
            }
            
            
            // Connect to the back left
            if (tileManager.backLeftTileManager != null
                && tileManager.backLeftTileManager.gameObject.activeSelf == true
                && !tileManager.connectedBackLeftTileMesh)
            {
                // This code is never executed because of the way we iterate over the array since the side already gets generated by 
                // Top neighbour tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int topNeighbourIndex = tileManager.backLeftTileManager.topTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBack + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontRight + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideFront + topNeighbourIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBack + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFront + topNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + topTileIndex * HexagonVertexOffset });

                // Bottom neighbour tile 
                int bottomTileIndex = tileManager.lowerTileMeshIndex;
                int bottomNeighbourIndex = tileManager.backLeftTileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBack + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFront + bottomNeighbourIndex * HexagonVertexOffset, 
                    Hexagon.SideFrontRight + bottomNeighbourIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBack + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideFront + bottomNeighbourIndex * HexagonVertexOffset });

                tileManager.connectedBackLeftTileMesh = true;
                tileManager.backLeftTileManager.connectedFrontRightTileMesh = true;

                backLeftConnections++;
            }
            else if (tileManager.backLeftTileManager == null ||
                    (tileManager.backLeftTileManager != null && tileManager.backLeftTileManager.gameObject.activeSelf == false) &&
                    !tileManager.connectedBackLeftTileMesh)
            {
                // No neighbour to the back left. Connect to the bottom tile
                int topTileIndex = tileManager.topTileMeshIndex;
                int bottomTileIndex = tileManager.lowerTileMeshIndex;

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBack + topTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBack + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + topTileIndex * HexagonVertexOffset });

                dirtTriangles.AddRange(new int[] { 
                    Hexagon.SideBack + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + bottomTileIndex * HexagonVertexOffset, 
                    Hexagon.SideBackLeft + topTileIndex * HexagonVertexOffset });

                backLeftBottomConnections++;
            }
        }

        Debug.Log("leftConnections: " + leftConnections + " leftBottomConnections: " + leftBottomConnections);
        Debug.Log("rightConnections : " + rightConnections + " rightBottomConnections : " + rightBottomConnections);
        Debug.Log("frontRightConnections : " + frontRightConnections + " frontRightBottomConnections : " + frontRightBottomConnections);
        Debug.Log("frontLeftConnections : " + frontLeftConnections + " frontLeftBottomConnections : " + frontLeftBottomConnections);
        Debug.Log("backRightConnections : " + backRightConnections + " backRightBottomConnections : " + backRightBottomConnections);
        Debug.Log("backLeftConnections : " + backLeftConnections + " backLeftBottomConnections : " + backLeftBottomConnections);

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = grass1Triangles.ToArray();

        mesh.subMeshCount = 8;

        mesh.SetTriangles(dirtTriangles, 0);
        mesh.SetTriangles(waterTriangles, 1);
        mesh.SetTriangles(sandTriangles, 2);
        mesh.SetTriangles(snowTriangles, 3);
        mesh.SetTriangles(rockTriangles, 4);
        mesh.SetTriangles(grass1Triangles, 5);
        mesh.SetTriangles(grass2Triangles, 6);
        mesh.SetTriangles(grass3Triangles, 7);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
}
