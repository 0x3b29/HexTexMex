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

    private Player owner;
    bool destroyed;
    bool foundFinalTarget;

    public void Initialize(GameObject trader, Tile tile, Player owner)
    {
        visitedTiles = new List<Tile>();
        currentTile = tile;
        traderGameObject = trader;
        this.owner = owner;
        destroyed = false;
        foundFinalTarget = false;
    }

    public void FixedUpdate()
    {
        // Return if Target is not yet set
        if (targetTile == null || destroyed)
        {
            return;
        }

        // Check if trader is close enough to its target
        if (Vector3.Distance(targetTile.hexagonGameObject.transform.position, traderGameObject.transform.position) < minDistanceToTarget)
        {
            // If the trader arrived at the targetTile, and the tile has a house, his job is done
            if (targetTile.woodhouse)
            {
                CollectCoins(visitedTiles.Count);
                Disappear();
            }

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
        if (destroyed || foundFinalTarget)
        {
            return;
        }

        // For the first walk, the target is not yet set
        if (targetTile != null)
        {
            currentTile = targetTile;
        }

        // Never go back to this tile
        visitedTiles.Add(currentTile);

        // Find new target tile
        List<Tile> walkableTiles = currentTile.GetWalkableNeighbours();

        // Remove visited tiles from walkable tiles
        walkableTiles = walkableTiles.Except(visitedTiles).ToList<Tile>();

        // If there is no where to go, dissapear
        if (walkableTiles.Count == 0)
        {
            Disappear();
            return;
        }

        // Find new target
        targetTile = walkableTiles[Random.Range(0, walkableTiles.Count)];

        // If new target is a woodhouse, do not walk() again.
        if (targetTile.woodhouse)
        {
            foundFinalTarget = true;
        }
    }

    private void CollectCoins(int coins)
    {
        targetTile.owner.AddCoins(coins);

        if (owner == GameManager.Instance.turnManager.GetCurrentPlayer())
        {
            GameManager.Instance.uiManager.UpdateResources(targetTile.owner.GetStone(),
                targetTile.owner.GetWood(),
                targetTile.owner.GetWheat(),
                targetTile.owner.GetCoins());
        }
    }

    private void Disappear()
    {
        destroyed = true;
        traderGameObject.SetActive(false);

        // TODO: add destroy() to function outside the foreach loop to avoid modifying collection while still in loop
    }
}
