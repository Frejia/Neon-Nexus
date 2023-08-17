using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;

/// <summary>
/// Handles full controls of ShipMovement and Player Input
///
/// Requires Rigidbody and PlayerInput
/// Reference: https://youtu.be/fZvJvZA4nhY
/// Extended by Fanny: rotation, boosting, shooting, aiming and changed settings
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ShipMovement : MonoBehaviour
{
    [Header("Ship Movement Settings")] 
    [SerializeField]
    public float yawTorque = 500f;
    [SerializeField] public float pitchTorque = 1000f;
    [SerializeField]
    public float rollTorque = 500f;
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float upThrust = 50f;
    [SerializeField]
    private float strafeThrust = 50f;

    [Header("Glide Offset Settings")]
    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.5f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftRightGlideReduction = 0.111f;

    private float glide = 0f, horizontalGlide = 0f, verticalGlide = 0f;

    [Header("Boost Settings")] 
    [SerializeField] public bool boosting = false;
    private float currentBoostAmount;
    private float maxBoostAmount = 20f;
    [SerializeField] private float boostDepracationRate = 0.1f;
    [SerializeField] private float boostRechargeRate = 0.15f;
    [SerializeField] private float boostMultiplier = 10f;
    
    public delegate void Boost(int i);
    public static event Boost boostInit;

    [Header("Other Settings")]
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private ParticleSystem boostEffect;
    private Rigidbody rb;
    private Rotator rotator;
    private NewRotator newRotator;
    
    
    //Input Values
    private float thrust1D, upDown1D, strafe1D, roll1D;
    private Vector2 pitchYaw;

    // Prepare for input
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //rotator = gameObject.transform.GetChild(0).GetComponent<Rotator>();
        newRotator = gameObject.transform.GetChild(0).GetComponent<NewRotator>();

        Cursor.lockState = CursorLockMode.Confined;
        currentBoostAmount = maxBoostAmount;
        boostEffect.Stop();
    }

    // Handle input in Fixed Update --> Avoides FPS problems across different systems
    void FixedUpdate()
    {
        HandleBoosting();
        HandleMovement();
    }

    // ------- BOOSTING -------
    private void HandleBoosting()
    {
        if (boosting && currentBoostAmount > 0f)
        {
            currentBoostAmount -= boostDepracationRate;
            if(currentBoostAmount <= 0f)
            {
                boosting = false;
            }
        }
        else
        {
            if (currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRechargeRate;
            }
        }
    }

    // ------- MOVEMENT -------
    private void HandleMovement()
    {
        //Roll
        rb.AddRelativeTorque(Vector3.back * roll1D * rollTorque * Time.fixedDeltaTime);
        //Pitch
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchTorque * Time.fixedDeltaTime);
        //Yaw
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.fixedDeltaTime);
        
        //Thrust
        if (thrust1D > 0.1f || thrust1D < -0.1f)
        {
            float currentThrust;
            if (boosting)
            {
                boostEffect.Play();
                boostInit(3);
                currentThrust = thrust * boostMultiplier;
            }
            else
            { 
                boostEffect.Stop();
                currentThrust = thrust;
            }

            rb.AddRelativeForce(Vector3.forward * thrust1D * currentThrust * Time.fixedDeltaTime);
            glide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.fixedDeltaTime);
            glide *= thrustGlideReduction;
        }
        
        //UpDown
        if (upDown1D > 0.1f || upDown1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.up * upDown1D * upThrust * Time.fixedDeltaTime);
            verticalGlide = upDown1D + upThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.fixedDeltaTime);
            verticalGlide *= upDownGlideReduction;
        }
        
        //Strafing
        if (strafe1D > 0.1f || strafe1D < -0.1f)
        {
           // rotator.SetRotations(this.transform.rotation, new Quaternion(0f,0f,-50f, 0), new Quaternion(0f,0f,-50f, 0));
            rb.AddRelativeForce(Vector3.right * strafe1D * strafeThrust * Time.fixedDeltaTime);
            horizontalGlide = strafe1D + strafeThrust;
            //Check if strafe left or right
            //newRotator.SetStrafe(strafe1D);
            
            /*
            if (strafe1D < 0)
            {
                //left
                //rotator.SetRotState(RotationDirection.Left);
                
            }
            else
            {
                //right
                //rotator.SetRotState(RotationDirection.Right);
            }
            */
            
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= leftRightGlideReduction;
            //rotator.SetRotState(RotationDirection.Neutral);
            strafe1D = 0;
            //newRotator.SetStrafe(strafe1D);
        }
        
        newRotator.SetStrafe(strafe1D);
        
    }
    
    // ------- INPUT METHODS -------
    #region Input Methods
    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1D = context.ReadValue<float>();
    }
    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }
    public void OnUpDown(InputAction.CallbackContext context)
    {
        upDown1D = context.ReadValue<float>();
    }
    public void OnRoll(InputAction.CallbackContext context)
    {
        roll1D = context.ReadValue<float>();
    }
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
    }
    public void OnBoost(InputAction.CallbackContext context)
    {
        
        boosting = context.performed;
    }
    #endregion
    
}
