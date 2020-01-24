using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBehaviour : MonoBehaviour
{
    private GameObject dragonGameObject;
    private ParticleSystem fire;
    private Tile targetTile;
    private float movementSpeed = 10f;
    private bool movingTowardsTarget = true;
    private float lastDistance = float.MaxValue;

    public void Initialize(GameObject dragonGameObject, Tile targetTile)
    {
        this.dragonGameObject = dragonGameObject;
        this.targetTile = targetTile;

        // Stop the fire breath
        fire = dragonGameObject.transform.Find("Head").transform.Find("Fire").GetComponent<ParticleSystem>();
        fire.Stop();

        // Set target to the same hight as dragon is flying
        Vector3 dragonAttackPosition = targetTile.transform.position;
        dragonAttackPosition.y = 5;

        // Make the dragon facing its target
        dragonGameObject.transform.rotation = Quaternion.LookRotation(dragonAttackPosition - dragonGameObject.transform.position);

        // Move the dragon back by a bit
        dragonGameObject.transform.position -= dragonGameObject.transform.forward * 25f;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the dragon in its facing direction
        dragonGameObject.transform.position += dragonGameObject.transform.forward * Time.deltaTime * movementSpeed;

        // Distance to target is absolute (No negative)
        float distanceToTarget = Vector3.Distance(targetTile.transform.position, dragonGameObject.transform.position);

        // Determine if we are before or behind the target
        if (distanceToTarget <= lastDistance)
        {
            lastDistance = distanceToTarget;
        } 
        else
        {
            movingTowardsTarget = false;
        }

        // Only spit fire if distance smaller than 15m && dragon is approacing target
        if (distanceToTarget < 15f && movingTowardsTarget)
        {
            fire.Play();

            // Destroy the stuff build on the target tile, and its neighbours
            targetTile.Invoke("DestroyFeature", Random.Range(1, 4f));
            foreach(Tile tile in targetTile.GetNeighbours())
            {
                tile.Invoke("DestroyFeature", Random.Range(1, 4f));
            }
        }
        else
        {
            fire.Stop();
        }

        // Despawn if realy far away and going further
        if (distanceToTarget > 150f && !movingTowardsTarget)
        {
            Destroy(dragonGameObject);
            dragonGameObject = null;
        }
    }
}
