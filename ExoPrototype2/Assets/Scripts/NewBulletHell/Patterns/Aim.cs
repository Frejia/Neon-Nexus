using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

/// <summary>
/// Aim Abstract Class for Aiming of Player and Enemy for Patterns
///
/// used by EnemyAim and PlayerAim
/// </summary>
public abstract class Aim : MonoBehaviour
{
    //public static Aim Instance { get; private set; }
    public GameObject target { get; set; }
    public GameObject user { get; set; }
    
    // get the direction of the target from the user
    // used in the PatternManager to get the direction of the pattern
    public Vector3 Aiming()
    {
        Vector3 direction;
        //Calculate Angle for Direction of player from this gameobject position in a 3D Space, including the z axis
        direction = target.transform.position - user.transform.position;

            return direction.normalized;
    }

    // get a random direction to shoot in if Aiming is off --> LEGACY mostly
    public Vector3 RandomAim()
    {
        float angle = Random.Range(0, 360);
        
        float bulDirX = user.transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
        float bulDirY = user.transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

        Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
        
        Vector3 bulDir = (bulMoveVector - user.transform.position).normalized;
        return bulDir;
    }
}
