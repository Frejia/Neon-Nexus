using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


/// <summary>
/// Changes Controls for the players
///
/// Used in Main Menu
/// </summary>
public class SetControls : MonoBehaviour
{
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private Material mat1;
    [SerializeField] private Material mat2;
    //[SerializeField] private GameObject player2Prefab;

    [SerializeField] private Slider player1Sensitivity;
    //[SerializeField] private Slider player2Sensitivity;

    // Set Control Scheme on Player Prefab
    public void SetScheme(bool chosen)
    {
        if (chosen)
        {
            //Keyboard
            Debug.Log("Made a Keyboard player");
            // var p1 = PlayerInput.Instantiate(playerPrefab, controlScheme: "Keyboard");
            player1Prefab.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard", Keyboard.current);
            //player2Prefab.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Controller", Gamepad.current);
          
            
        }
        else
        {
            //Controller
            Debug.Log("Made a Controller player");
            // var p2 = PlayerInput.Instantiate(playerPrefab, controlScheme: "Controller");
            player1Prefab.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Controller", Gamepad.current);
           // player2Prefab.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard", Keyboard.current);
        }
    }

    // Change Sensitivity Settings for pitch and yaw
    public void SetSensitivity()
    {
        player1Prefab.GetComponent<ShipMovement>().yawTorque = player1Sensitivity.value;
        player1Prefab.GetComponent<ShipMovement>().pitchTorque = player1Sensitivity.value;
       // player2Prefab.GetComponent<ShipMovement>().yawTorque = sensitivity;
       // player2Prefab.GetComponent<ShipMovement>().pitchTorque = sensitivity;
    }

    public void SetModeColor1(Material newMat)
    {
        mat1 = newMat;
    }

    public void SetModeColor2(Material newMat)
    {
        mat2 = newMat;
    }

    public void SetPlayerModel(GameObject model)
    {
        player1Prefab.transform.GetChild(0).GetComponent<MeshFilter>().mesh = model.GetComponent<MeshFilter>().mesh;
        player1Prefab.transform.GetChild(0).GetComponent<MeshRenderer>().materials = model.GetComponent<MeshRenderer>().materials;
    }

    private GameObject TrailObj;
    
    public void PlayTrail(GameObject trail)
    {
        TrailObj = trail;
        
        // Move the object slowly left and right
        Vector3 A = new Vector3(-98.6999969f, 0f, -97.0999985f);
        Vector3 B = new Vector3(-266.600006f, -2.9000001f, -378.799988f);
        float timeElapsed = 0f;
        
        while(timeElapsed < 20f)
        {
            timeElapsed += Time.deltaTime * 0.001f;
            Debug.Log("Moving Trail");
            TrailObj.transform.position = Vector3.Lerp(A, B, timeElapsed);
        }
    }
    
}