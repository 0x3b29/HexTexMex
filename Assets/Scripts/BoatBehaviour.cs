using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatBehaviour : MonoBehaviour
{
    private GameObject boatGameObject;
    private Tile targetTile;
    private int boatNumber;

    private const float turnRate = 2f;
    private const float moveRate = 2f;
    private const float minDistanceToTarget = 0.01f; // 10cm
    private const float minRotationError = 1f; // 1°

    public void Initialize(GameObject boatGameObject, Tile targetTile, int boatNumber)
    {
        this.boatGameObject = boatGameObject;
        this.targetTile = targetTile;
        this.boatNumber = boatNumber;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Check if boat is close enough to its target
        if (Vector3.Distance(targetTile.hexagonGameObject.transform.position, boatGameObject.transform.position) < minDistanceToTarget)
        {
            targetTile = targetTile.getRandomWaterNeighbour();
        }

        // Only move while pointing at target
        if (Vector3.Angle(boatGameObject.transform.forward, targetTile.transform.position - boatGameObject.transform.position) > minRotationError)
        {
            // Turn as nessesary
            boatGameObject.transform.rotation = Quaternion.Slerp(boatGameObject.transform.rotation, Quaternion.LookRotation(targetTile.transform.position - boatGameObject.transform.position), Time.deltaTime * turnRate);
        }
        else
        {
            // Move to target
            transform.position = Vector3.Slerp(transform.position, targetTile.hexagonGameObject.transform.position, Time.deltaTime * moveRate);
        }
    }
}
