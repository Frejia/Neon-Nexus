using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCard : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera cam;

     void Start()
     {
         cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
     }
     void Update()
    {
        Vector3 mouse = Input.mousePosition;
        Ray mouseRay = cam.ScreenPointToRay(mouse);

        float middlePoint = (transform.position - cam.transform.position).magnitude * 0.5f;
        transform.LookAt(mouseRay.origin + mouseRay.direction * middlePoint);
    }
}
