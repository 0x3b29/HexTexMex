using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatBehaviour : MonoBehaviour
{
    private GameObject boatGameObject;
    private Tile targetTile;

    public void Initialize(GameObject boatGameObject, Tile targetTile)
    {
        this.boatGameObject = boatGameObject;
        this.targetTile = targetTile;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("I'm a Boat!");
    }

    // Update is called once per frame
    void Update()
    {
        // Check if boat has arrived
        if (Vector3.Distance(targetTile.hexagonGameObject.transform.position, boatGameObject.transform.position) < .01f)
        {
            Debug.Log("Boat arrived at " + targetTile.xCoordinate + " " + targetTile.yCoordinate);
            targetTile = targetTile.getRandomWaterNeighbour();
            Debug.Log("Boat leaving for " + targetTile.xCoordinate + " " + targetTile.yCoordinate);
        }

        // Turn as nessesary
        if (Vector3.Angle(boatGameObject.transform.forward, targetTile.transform.position - boatGameObject.transform.position) > 1f)
        {
            boatGameObject.transform.rotation = Quaternion.Slerp(boatGameObject.transform.rotation, Quaternion.LookRotation(targetTile.transform.position - boatGameObject.transform.position), Time.deltaTime * 2f);
        }
        else
        {
            transform.position = Vector3.Slerp(transform.position, targetTile.hexagonGameObject.transform.position, Time.deltaTime * 2f);
        }
    }
}
