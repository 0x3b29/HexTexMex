using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Hexagon
{
    public static int Front = 0;
    public static int FrontRight = 1;
    public static int BackRight = 2;
    public static int Back = 3;
    public static int BackLeft = 4;
    public static int FrontLeft = 5;
    public static int SideFront = 6;
    public static int SideFrontRight = 7;
    public static int SideBackRight = 8;
    public static int SideBack = 9;
    public static int SideBackLeft = 10;
    public static int SideFrontLeft = 11;

    public static Vector3[] topVertices = new Vector3[12]
    { 
        // Blender X = Sidewards
        // Blender Y = Forwards
        // Blender Z = Height

        // Unity X = Sidewards
        // Unity Z = forward
        // Unity Y = Height

        // I choose to double the vertices and connect the sides and the top/bottom faces individually.
        // This was done to avoid weird shading because normals would point all over the place. 
        // By not having connected sides, the top faces normals are pointing straigt up.

        // These are the vertices for the top faces
        new Vector3(0, 0, 1),
        new Vector3(0.866025f, 0, 0.5f),
        new Vector3(0.866025f, 0, -0.5f),
        new Vector3(0, 0, -1),
        new Vector3(-0.866025f, 0, -0.5f),
        new Vector3(-0.866025f, 0, 0.5f),

        // These are the vertices for the side faces
        new Vector3(0, 0, 1),
        new Vector3(0.866025f, 0, 0.5f),
        new Vector3(0.866025f, 0, -0.5f),
        new Vector3(0, 0, -1),
        new Vector3(-0.866025f, 0, -0.5f),
        new Vector3(-0.866025f, 0, 0.5f),
    };

    public static int[] topTriangles = new int[]
    {
        Front, FrontRight, BackRight,
        BackRight, Back, BackLeft,
        BackLeft, FrontLeft, Front,
        Front, BackRight, BackLeft
    };

    public static Vector3[] lowerVertices = new Vector3[12]
    { 
        // Blender X = Sidewards
        // Blender Y = Forwards
        // Blender Z = Height

        // Unity X = Sidewards
        // Unity Z = forward
        // Unity Y = Height

        new Vector3(0, 0, 1),
        new Vector3(0.866025f, 0, 0.5f),
        new Vector3(0.866025f, 0, -0.5f),
        new Vector3(0, 0, -1),
        new Vector3(-0.866025f, 0, -0.5f),
        new Vector3(-0.866025f, 0, 0.5f),

        new Vector3(0, 0, 1),
        new Vector3(0.866025f, 0, 0.5f),
        new Vector3(0.866025f, 0, -0.5f),
        new Vector3(0, 0, -1),
        new Vector3(-0.866025f, 0, -0.5f),
        new Vector3(-0.866025f, 0, 0.5f),
    };

    public static int[] lowerTriangles = new int[]
    {
        BackRight, FrontRight, Front,
        BackLeft, Back, BackRight,
        Front, FrontLeft, BackLeft,
        BackLeft ,BackRight, Front
    };
}