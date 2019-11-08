using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TraderBehaviour : MonoBehaviour
{
    private const float turnRate = 2f;
    private const float moveRate = 2f;
    private const float minDistanceToTarget = 0.01f; // 10cm
    private const float minRotationError = 1f; // 1°

    public List<Tile> visitedTiles;
    public Tile currentTile;
    public Tile targetTile;
    public GameObject traderGameObject;

    public void Initialize(GameObject trader, Tile tile)
    {
        visitedTiles = new List<Tile>();
        currentTile = tile;
        traderGameObject = trader;
    }

    public void Update()
    {
        // Return if Target is not yet set
        if (targetTile == null)
        {
            return;
        }

        // Check if trader is close enough to its target
        if (Vector3.Distance(targetTile.hexagonGameObject.transform.position, traderGameObject.transform.position) < minDistanceToTarget)
        {
            return;
        }

        // Only move while pointing at target
        if (Vector3.Angle(traderGameObject.transform.forward, targetTile.transform.position - traderGameObject.transform.position) > minRotationError)
        {
            // Check if there is a need to turn
            Vector3 newPosition = targetTile.transform.position - traderGameObject.transform.position;
            if (newPosition.magnitude > 0.001f)
            {
                // Turn as nessesary
                traderGameObject.transform.rotation = Quaternion.Slerp(traderGameObject.transform.rotation, Quaternion.LookRotation(targetTile.transform.position - traderGameObject.transform.position), Time.deltaTime * turnRate);
            }
        }
        else
        {
            // Move to target
            transform.position = Vector3.Slerp(transform.position, targetTile.hexagonGameObject.transform.position, Time.deltaTime * moveRate);
        }
    }

    public void Walk()
    {
        if (targetTile != null)
        {
            currentTile = targetTile;
        }

        Debug.Log("currentTile: " + currentTile.name);
        visitedTiles.Add(currentTile);

        // Find new target tile
        List<Tile> walkableTiles = currentTile.GetWalkableNeighbours();

        // Remove visited tiles from walkable tiles
        walkableTiles = walkableTiles.Except(visitedTiles).ToList<Tile>();

        // There is no where to go, dissapear
        if (walkableTiles.Count == 0)
        {
            Disappear();
            return;
        }

        targetTile = walkableTiles[Random.Range(0, walkableTiles.Count)];
        Debug.Log("targetTile: " + targetTile.name);

        if (targetTile == null || targetTile.woodhouse)
        {
            CollectCoins();
            Disappear();
        }
    }

    private void CollectCoins()
    {
        // TODO: Implement functionality
    }

    private void Disappear()
    {
        // TODO: Wait until arrived befor Destroy 
        Destroy(traderGameObject);
        traderGameObject = null;
    }
}
