using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// uses Aim abstract class, but overrides the target to be closest Enemy with the EnemyLockOn and RemoveFromTargets methods
/// </summary>
public class PlayerAim : Aim
{
    private List<GameObject> targets;

    private void Start()
    {
    }

    // Get Nearest Enemy if there are more than one in the proximity
    private GameObject GetNearestEnemy()
    {
        GameObject nearestObject = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy  in targets)
        {
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, this.gameObject.transform.position);

            // If the calculated distance is less than the current minimum distance, update the nearestObject and minDistance
            if (distanceToPlayer < minDistance)
            {
                nearestObject = enemy;
                minDistance = distanceToPlayer;
            }  
        }
       return nearestObject;
    }

    // Create list of which Enemies are currently in proximity and set the correct target
    private void EnemyLockOn(GameObject enemy)
    {
        Debug.Log("Enemy in screen");
        
        // If that gameobject is not already in the list, add it
            if (!targets.Contains(enemy))
            {
                targets.Add(enemy);
            }
            // If there is more than one enemy in the list, get the nearest enemy
            if(targets.Count > 1 && enemy != null)
            {
                target = GetNearestEnemy();
            }
            else
            {
                target = enemy;
                enemy.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            
            Debug.Log("Target" + target);
    }

    // When enemy leaves player proximity, then remove it from the list and update the target
    private void RemoveFromTargets(GameObject enemy)
    {
        // Remove the enemy from the list if it exists in the list
        if (targets.Contains(enemy))
        {
            targets.Remove(enemy);
        }
        else
        {
            Debug.Log("Enemy not in list");
        }
        // If there are still enemies in the list, update the nearest enemy
        if (targets.Count > 0)
        {
            target = GetNearestEnemy();
        }
        else
        {
            target = null;
        }
    }
}
