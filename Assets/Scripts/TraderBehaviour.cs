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

    public List<TileManager> visitedTiles;
    public TileManager currentTileManager;
    public TileManager targetTileManager;
    public GameObject traderGameObject;

    private Player owner;
    bool destroyed;
    bool foundFinalTarget;

    public void Initialize(GameObject trader, TileManager tileManager, Player owner)
    {
        visitedTiles = new List<TileManager>();
        currentTileManager = tileManager;
        traderGameObject = trader;
        this.owner = owner;
        destroyed = false;
        foundFinalTarget = false;
    }

    public void FixedUpdate()
    {
        // Return if Target is not yet set
        if (targetTileManager == null || destroyed)
        {
            return;
        }

        // Check if trader is close enough to its target
        if (Vector3.Distance(targetTileManager.gameObject.transform.position, traderGameObject.transform.position) < minDistanceToTarget)
        {
            // If the trader arrived at the targetTile, and the tile has a house, his job is done
            if (targetTileManager.woodhouse)
            {
                CollectCoins(visitedTiles.Count);
                Disappear();
            }

            return;
        }

        // Only move while pointing at target
        if (Vector3.Angle(traderGameObject.transform.forward, targetTileManager.transform.position - traderGameObject.transform.position) > minRotationError)
        {
            // Check if there is a need to turn
            Vector3 newPosition = targetTileManager.transform.position - traderGameObject.transform.position;
            if (newPosition.magnitude > 0.001f)
            {
                // Turn as nessesary
                traderGameObject.transform.rotation = Quaternion.Slerp(traderGameObject.transform.rotation, Quaternion.LookRotation(targetTileManager.transform.position - traderGameObject.transform.position), Time.deltaTime * turnRate);
            }
        }
        else
        {
            // Move to target
            transform.position = Vector3.Slerp(transform.position, targetTileManager.gameObject.transform.position, Time.deltaTime * moveRate);
        }
    }

    public void Walk()
    {
        if (destroyed || foundFinalTarget)
        {
            return;
        }

        // For the first walk, the target is not yet set
        if (targetTileManager != null)
        {
            currentTileManager = targetTileManager;
        }

        // Never go back to this tile
        visitedTiles.Add(currentTileManager);

        // Find new target tile
        List<TileManager> walkableTileManagers = currentTileManager.GetWalkableNeighbours();

        // Remove visited tiles from walkable tiles
        walkableTileManagers = walkableTileManagers.Except(visitedTiles).ToList<TileManager>();

        // If there is no where to go, dissapear
        if (walkableTileManagers.Count == 0)
        {
            Disappear();
            return;
        }

        // Find new target
        targetTileManager = walkableTileManagers[Random.Range(0, walkableTileManagers.Count)];

        // If new target is a woodhouse, do not walk() again.
        if (targetTileManager.woodhouse)
        {
            foundFinalTarget = true;
        }
    }

    private void CollectCoins(int coins)
    {
        targetTileManager.owner.AddCoins(coins);

        if (owner == GameManager.Instance.turnManager.GetCurrentPlayer())
        {
            GameManager.Instance.uiManager.UpdateResources(targetTileManager.owner.GetStone(),
                targetTileManager.owner.GetWood(),
                targetTileManager.owner.GetWheat(),
                targetTileManager.owner.GetCoins());
        }
    }

    private void Disappear()
    {
        destroyed = true;
        traderGameObject.SetActive(false);

        // TODO: add destroy() to function outside the foreach loop to avoid modifying collection while still in loop
    }
}
