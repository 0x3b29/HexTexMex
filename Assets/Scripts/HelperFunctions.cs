using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions
{
    public static GameObject colorizeGameObject(GameObject gameObject, Color color)
    {
        foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            Material[] materials = meshRenderer.materials;

            foreach (Material material in materials)
            {
                material.color = color;
            }
        }

        return gameObject;
    }
}
