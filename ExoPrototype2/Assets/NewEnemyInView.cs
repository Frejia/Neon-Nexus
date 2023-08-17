using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemyInView : Aim
{
    public delegate void EnemyInScreen(GameObject enemy);

    public delegate void EnemySeenSound(int index);
    public static event EnemyInScreen OnEnemyInScreen;
    public static event EnemyInScreen OnEnemyNotInScreen;
    public static event EnemySeenSound OnEnemySeenSound;

    [SerializeField] private List<GameObject> targets;
    [SerializeField] private GameObject target;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if(!targets.Contains(other.gameObject))
            {
                targets.Add(other.gameObject);
            }

            if(targets.Count > 1)
            {
                target = GetClosestEnemy();
                OnEnemySeenSound(5);
            }
            else
            {
                target = other.gameObject;
                OnEnemySeenSound(5);
            }
            Debug.Log(gameObject.name + " is visible");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (targets.Contains(other.gameObject))
            {
                targets.Remove(other.gameObject);
            }
        }
        
        if (targets.Count > 0)
        {
            target = GetClosestEnemy();
        }
        else
        {
            target = null;
        }
    }
    
    public GameObject GetClosestEnemy()
    {
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;
        Vector3 thisPosition = transform.position;

        foreach (GameObject obj in targets)
        {
            float distance = Vector3.Distance(thisPosition, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

       return closestObject;
    }
    
}
