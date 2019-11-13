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

    // Update is called once per frame
    void FixedUpdate()
    {
        // Check if boat is close enough to its target
        if (Vector3.Distance(targetTile.hexagonGameObject.transform.position, boatGameObject.transform.position) < minDistanceToTarget)
        {
            Tile newTarget = targetTile.getRandomWaterNeighbour();
            targetTile = newTarget;
        }

        // Only move while pointing at target
        if (Vector3.Angle(boatGameObject.transform.forward, targetTile.transform.position - boatGameObject.transform.position) > minRotationError)
        {
            // Check if there is a need to turn
            Vector3 newPosition = targetTile.transform.position - boatGameObject.transform.position;
            if (newPosition.magnitude > 0.001f)
            {
                // Turn as nessesary
                boatGameObject.transform.rotation = Quaternion.Slerp(boatGameObject.transform.rotation, Quaternion.LookRotation(newPosition), Time.deltaTime * turnRate);
            }
        }
        else
        {
            // Move to target
            transform.position = Vector3.Slerp(transform.position, targetTile.hexagonGameObject.transform.position, Time.deltaTime * moveRate);
        }
    }
}
