using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; 

/// <summary>
/// CameraInitalizer handles the Cinemachine Cameras when using a second player,
/// handles the correct layering of the players as well as the correct culling mask
/// this solves the Problem was that the camera would not render the correct player when using a second player
///
/// Used on Player Prefab
/// </summary>
public class CameraInitalizer : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private GameObject virtualPlayerCam;
    
    // Is called when second player spawns scene
    void Start()
    {
        ShipMovement[] characterMovements = FindObjectsOfType<ShipMovement>();
        int layer;
        if (characterMovements.Length == 1)
        {
            // When there is 1 player, then the layer is the first player
            layer = 8;
            cam.gameObject.transform.parent.gameObject.layer = 8;
        }
        else
        {
            //When there is a 2nd player, then the layer is the second player
            layer = 13;
            cam.gameObject.transform.parent.gameObject.layer = 13;
            cam.gameObject.transform.parent.gameObject.tag = "Player2";
        }
        
        virtualPlayerCam.layer = layer;
        var bitMask = (1 << layer)
                      | (1 << 0)
                      | (1 << 1)
                      | (1 << 2)
                      | (1 << 3)
                      | (1 << 4)
                      | (1 << 5)
                      | (1 << 6)
                      | (1 << 7)
                      | (1 << 8)
                      | (1 << 9)
                      | (1 << 10)
                      | (1 << 11);
        
        cam.cullingMask = bitMask;
        cam.gameObject.layer = layer;
    }
    
}
