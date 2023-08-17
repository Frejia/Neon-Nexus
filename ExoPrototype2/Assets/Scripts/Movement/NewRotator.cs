using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRotator : MonoBehaviour
{
    public enum RotationDirection
    {
        Left,
        Right,
        Neutral,
        Other,
        None
    }
    
    private RotationDirection currentDirection;
    private float strafe1D;
    
    private void FixedUpdate()
    {
        HandleRotation();
        //Debug.Log(Time.fixedDeltaTime);
    }

    private void HandleRotation()
    {
        switch (strafe1D)
        {
            case > 0.1f:
                transform.Rotate(Vector3.right * 100f * Time.fixedDeltaTime);
                break;
            case < -0.1f:
                transform.Rotate(Vector3.left * 100f * Time.fixedDeltaTime);
                break;
            default:
                //rotate back to neutral rot
                break;
        }
    }
    
    public void SetStrafe(float strafe)
    {
        strafe1D = strafe;
    }
}
