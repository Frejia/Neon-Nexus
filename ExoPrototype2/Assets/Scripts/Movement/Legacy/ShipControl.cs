using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class ShipControl : MonoBehaviour
{
    [Header("Ship Movement Settings")]
    [SerializeField] public float forwardSpeed = 25f, strafeSpeed = 7.5f, hoverSpeed = 5f;
    [SerializeField] private float activeForwardSpeed, activeStrafeSpeed, activeHoverSpeed;

    private float forwardAcceleration = 2.5f, strafeAcceleration = 2f, hoverAcceleration = 2f;

    public float lookRotateSpeed = 90f;
    private Vector2 lookInput, screenCenter, mouseDistance;

    private float rollInput;
    public float rollSpeed = 90f, rollAcceleration = 3.5f;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 25f;
    private float dashingTime = 0.3f;
    private float dashingCooldown = 2f;
    private Vector3 dashdirection;
    
    private bool canDodge = true;
    private bool isDodging;
    private float dodgePower = 30f;
    private float dodgeTime = 0.2f;
    private float dodgeCooldown = 1f;
    private Vector3 dodgedirection;
    [SerializeField] private TrailRenderer trail;
    
    //public enum ShipState { Idle, Accelerating, Decelerating, Strafing, Hovering, Rolling, Locked };
    public bool Ball = false;
    ShipState currentShip;
    public enum ShipState
    {
        Ball,
        Hover
    };
    
    public void StateCheck()
    {
        switch (Ball)
        {
            case true:
                currentShip = ShipState.Ball;
                InitBall();
                break;
            case false:
                currentShip = ShipState.Hover;
                InitHover();
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        screenCenter.x = Screen.width * .5f;
        screenCenter.y = Screen.height * .5f;

        Cursor.lockState = CursorLockMode.Confined;
        trail.emitting = false;
        currentShip = new ShipState();
        currentShip = ShipState.Hover;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 forward = transform.forward * 10;
        Gizmos.DrawLine(transform.position, lookInput * 10f);
        Vector3 right = transform.right * 10;
       // Gizmos.DrawLine(transform.position, right * 10f);
    }

    public Vector3 Getmousepos()
    {
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;
        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;
        
        return mouseDistance;
    }
    // Update is called once per frame
    void Update()
    {

        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);

        if (Input.GetKeyDown(KeyCode.E) && canDodge || Input.GetKeyDown(KeyCode.Q) && canDodge)
        {
            StartCoroutine(Dodge());
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        
        rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);
        transform.Rotate(-mouseDistance.y * lookRotateSpeed * Time.deltaTime, mouseDistance.x * lookRotateSpeed * Time.deltaTime, rollInput * rollSpeed * Time.deltaTime, Space.Self);
        
        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, forwardAcceleration * Time.deltaTime);
        activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed, strafeAcceleration * Time.deltaTime);
        activeHoverSpeed =  Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, hoverAcceleration * Time.deltaTime);

        transform.position += transform.forward * activeForwardSpeed * Time.deltaTime;
        transform.position += (transform.right * activeStrafeSpeed * Time.deltaTime) + (transform.up * activeHoverSpeed * Time.deltaTime);

        /*Debug.Log(this.GetComponent<Rigidbody>().velocity.x == 0);
        // I want to get the speed of the rigidbody how do I do this?
        if (this.GetComponent<Rigidbody>().velocity.x == 0)
        {
            currentShip = ShipState.Ball;
            StateCheck();
        }*/
    }
    
    /*
    For those wondering how to add collisions, I did it like this:
    *Add a Rigidbody component, then replace:
    transform.position += transform.forward * activeForwardSpeed * Time.deltaTime;
    transform.position += (transform.right * activeStrafeSpeed * Time.deltaTime) + (transform.up * activeHoverSpeed * Time.deltaTime);
    *with this (in a Fixed Update):
    Vector3 forward = transform.forward * activeForwardSpeed * Time.fixedDeltaTime;
    Vector3 strafe = transform.right * activeStrafeSpeed * Time.fixedDeltaTime;
    Vector3 hover = transform.up * activeHoverSpeed * Time.fixedDeltaTime;

    Vector3 movement = forward + strafe + hover;
    gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + movement);
    Of course, you could reference the Rigidbody, so it could be anywhere in the scene, but this should work nonetheless
    */

    float delayBetweenPresses = 0.25f;
    bool pressedFirstTime = false;
    float lastPressedTime;
    private void LateUpdate()
    {
        //Geht so nicht, weil latestposition und gravity nicht gehen zusammen
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pressedFirstTime) // we've already pressed the button a first time, we check if the 2nd time is fast enough to be considered a double-press
            {
                bool isDoublePress = Time.time - lastPressedTime <= delayBetweenPresses;
 
                if (isDoublePress)
                {
                    Ball = !Ball;
                    pressedFirstTime = false;
                }
            }
            else // we've not already pressed the button a first time
            {
                pressedFirstTime = true; // we tell this is the first time
            }
 
            lastPressedTime = Time.time;
        }
 
        if (pressedFirstTime && Time.time - lastPressedTime > delayBetweenPresses) // we're waiting for a 2nd key press but we've reached the delay, we can't consider it a double press anymore
        {
            // note that by checking first for pressedFirstTime in the condition above, we make the program skip the next part of the condition if it's not true,
            // thus we're avoiding the "heavy computation" (the substraction and comparison) most of the time.
            // we're also making sure we've pressed the key a first time before doing the computation, which avoids doing the computation while lastPressedTime is still uninitialized
            pressedFirstTime = false;
        }
        /*if(Mathf.Approximately(this.GetComponent<Rigidbody>().velocity.magnitude, 0))
        {
            currentShip = ShipState.Ball;
            
        }
        else
        {
            currentShip = ShipState.Hover;
            
            lastPosition = transform.position;
        }*/
        StateCheck();
    }

    public void InitBall()
    {
        transform.localScale = new Vector3(1, 1, 1);
        //transform.localPosition = new Vector3(-5.15999985f, 0, -3.79999995f);
        Debug.Log("Ball");
        Ball = true;
        this.GetComponent<Rigidbody>().useGravity = true;
    }
    
    public void InitHover()
    {
        transform.localScale = new Vector3(1, 0.39537f, 1);
        //transform.localPosition = new Vector3(-5.15999985f, 0, -3.79999995f);
        Ball = false;
        this.GetComponent<Rigidbody>().useGravity = false;
    }

    private IEnumerator Dodge()
    {
        canDodge = false;
        isDodging = true;
        trail.emitting = true;
        //var scaledForward = this.transform.forward * 10f;
        //var left = Vector3.Cross(this.transform.up, scaledForward);
        var right = transform.right;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            this.GetComponent<Rigidbody>().velocity = right * dodgePower;
        }
        else
        {
            this.GetComponent<Rigidbody>().velocity = (right * -1) * dodgePower;
        }
        
        
        yield return new WaitForSeconds(dodgeTime);
        isDodging = false;
        trail.emitting = false;
        transform.Rotate(0, 0, 0);
        this.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        trail.emitting = true;
        var forward = this.transform.forward;
        this.GetComponent<Rigidbody>().velocity = forward * dashingPower;
        
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        trail.emitting = false;
        this.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
