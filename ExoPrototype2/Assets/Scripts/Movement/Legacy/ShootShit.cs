using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootShit : MonoBehaviour
{
    public GameObject firePoint;
    public List<GameObject> vfx = new List<GameObject>();
    private GameObject effectToSpawn;
    
    public Camera cam;
    public float maximumLength;

    private Ray rayMouse;
    private Vector3 pos;
    private Vector3 direction;
    private Quaternion rotation;

    private float delayBetweenPresses = 0.25f;
    private bool pressedFirstTime = false;
    private float lastPressedTime;
    public float maxValue = 10f;

    private bool keyHeld;
    [SerializeField] private float holdDuration;
    [SerializeField] private float calculatedValue;
    [SerializeField] private float chargeCooldown = 100f;

    // Start is called before the first frame update
    void Start()
    {
        effectToSpawn = vfx[0];
    }


    void RotateToMouseDirection(GameObject obj, Vector3 destination)
    {
        direction = destination - obj.transform.position;
        rotation = Quaternion.LookRotation(direction);
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }
    
    void SpawnVFX()
    {
        GameObject vfx;
        if (firePoint != null)
        {
            vfx = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity);
            vfx.transform.localRotation = Quaternion.Euler(this.GetComponent<ShipControl>().Getmousepos());
        }
        else
        {
            Debug.Log("No Fire Point");
        }
    }
    private void Update()
    {

        /*if (cam != null)
        {
            RaycastHit hit;
            var mousePos = Input.mousePosition;
            rayMouse = cam.ScreenPointToRay(mousePos);
            if (Physics.Raycast(rayMouse.origin, rayMouse.direction, out hit, maximumLength))
            {
                pos = hit.point;
                RotateToMouseDirection(this.gameObject, pos);
            }
            else
            {
                pos = rayMouse.GetPoint(maximumLength);
                RotateToMouseDirection(this.gameObject, pos);
            }
        }
        else
        {
            Debug.Log("No Camera");
        }*/

        // Long Click Shot/Charged Shot
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            keyHeld = true;
            holdDuration = 0f;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            //holdDuration += Time.deltaTime;
            holdDuration += Time.deltaTime + 1 / effectToSpawn.GetComponent<ShootMove>().fireRate;
            calculatedValue = Mathf.Clamp(holdDuration, 0f, maxValue);
            StartCoroutine(ChargedShot(calculatedValue));
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            keyHeld = false;
            holdDuration = 0f;
            calculatedValue = 0f;
        }
        
        //Double and single Click Shot
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (pressedFirstTime) // we've already pressed the button a first time, we check if the 2nd time is fast enough to be considered a double-press
            {
                bool isDoublePress = Time.time - lastPressedTime <= delayBetweenPresses;
 
                if (isDoublePress)
                {
                    // write double click command here
                    SpawnVFX();
                    Debug.Log("shoot big");
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
            SpawnVFX();
            Debug.Log("shoot");
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
    }

    private IEnumerator ChargedShot(float charge)
    {
        SpawnVFX();
        Debug.Log("Charged Shot");
        
        yield return new WaitForSeconds(chargeCooldown);
    }
}
