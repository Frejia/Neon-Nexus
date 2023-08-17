using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    private Material _material;
    private float _dissolve = -1.5f;
    private float endDissolve = 367.92f;
    private float dissolveSpeed;
    [SerializeField] private float dissolveTime;
    
    public bool isdissolving { get; set; }
    
    void Start()
    { 
        isdissolving = false;
        _material = GetComponent<MeshRenderer>().material;
        // Calculate the dissolve speed based on the desired dissolve time and final _dissolve value.
        dissolveSpeed = (endDissolve - _dissolve) / dissolveTime;
    }
    
     void FixedUpdate()
    {
        //Calculate 
        if (isdissolving)
        {
            _dissolve += Time.fixedDeltaTime * dissolveSpeed;
            _material.SetFloat("_Dissolve", _dissolve);
        }
    }
     
    
}
