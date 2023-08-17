using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses Aim abstract class, but overrides the UpdateTarget method to use the EnemySeesPlayer event
/// </summary>
public class EnemyAim : Aim
{
    private void OnEnable()
    {
        EnemySeesPlayer.GoFindPlayer += UpdateTarget;
    }
    
    private void OnDisable()
    {
        EnemySeesPlayer.GoFindPlayer -= UpdateTarget;
    }

    private void UpdateTarget(GameObject enemy, GameObject player)
    {
        enemy.GetComponent<EnemyAim>().target = player;
        enemy.GetComponent<EnemyAim>().user = enemy;
    }
}
