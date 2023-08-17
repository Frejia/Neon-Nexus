using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCheckpoint : MonoBehaviour
{
    private List<GameObject> enteredPlayers = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // check if player entered
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Player2"))
        {
            // check if portal has already been entered
            if (enteredPlayers != null)
            {
                // check if player has already entered
                foreach (var player in enteredPlayers)
                {
                    if (player == other.gameObject) return;
                }
            }
            
            // add player to entered players
            enteredPlayers.Add(other.gameObject);
            
            // add score to player
            if (other.gameObject.CompareTag("Player"))
            {
                GameModeManager.Instance.points1 += 100;
                GameModeManager.Instance.points1Text.text = GameModeManager.Instance.points1 + " Points";

            }
            else if (other.gameObject.CompareTag("Player2"))
            {
                GameModeManager.Instance.points2 += 100;
                GameModeManager.Instance.points2Text.text = GameModeManager.Instance.points2 + " Points";
            }
        }
    }
}
