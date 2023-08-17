using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachGoal : MonoBehaviour
{
    private int checkPoints;
    public delegate void RaceEvent(GameObject player);
    
    public static event RaceEvent ReachedGoal;
    
    // Check if player reached goal first 
    private bool reachedGoal = false;

    private void Start()
    {
        checkPoints = PerlinNoiseGen.Instance.waypoints.Count - 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!reachedGoal)
        {
            // check if player one has enough points and is the first to reach the goal
            if (GameModeManager.Instance.points1 > GameModeManager.Instance.pointsToWin  && other.gameObject.CompareTag("Player"))
            {
                ReachedGoal(other.gameObject);
                reachedGoal = true;
            } else if (GameModeManager.Instance.points2 > GameModeManager.Instance.pointsToWin && other.gameObject.CompareTag("Player2"))
            {
                ReachedGoal(other.gameObject);
                reachedGoal = true;
            }
        }
        else
        {
            // A Player already won
        }
    }
}
