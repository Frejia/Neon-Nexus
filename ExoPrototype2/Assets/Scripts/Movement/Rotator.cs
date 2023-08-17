using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotationDirection
{
    Left,
    Right,
    Neutral,
    Other,
    None
}

public class Rotator : MonoBehaviour
{
    //Rotator Rotation sides
    private Quaternion leftRotation { get; set; } // Set your left rotation
    private Quaternion rightRotation { get; set; } // Set your right rotation
    private Quaternion neutralRotation { get; set; }
    private Quaternion otherRotation { get; set; }
    private Quaternion startRotation { get; set; }
    private Quaternion endRotation { get; set; }

    private RotationDirection currentDirection;
    
    [Header("Rotator Settings")]
    [SerializeField] private float rotationSpeed = 90.0f;
    public bool debug;
    [SerializeField] private bool isAimPoint = false;
    [SerializeField] private bool isEnemy = false;
    [SerializeField] private Transform target;
    
    [SerializeField] private bool isRotating = false;

    private void OnEnable()
    {
        EnemySeesPlayer.GoFindPlayer += FaceToTurn;
    }
    
    private void OnDisable()
    {
        EnemySeesPlayer.GoFindPlayer -= FaceToTurn;
    }

    private void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    private void PerformRotation(RotationDirection direction)
    {
        switch (direction)
        {
            case RotationDirection.Left:
                StartRotating(transform.rotation, leftRotation);
                break;
            case RotationDirection.Right:
                StartRotating(transform.rotation, rightRotation);
                break;
            case RotationDirection.Neutral:
                StartRotating(transform.rotation, neutralRotation);
                break;
            case RotationDirection.Other:
                StartRotating(transform.rotation, otherRotation);
                break;
            case RotationDirection.None:
                StopRotating();
                break;
            default:
                break;
        }
    }

    public void SetRotState(RotationDirection rotationDirection)
    {
        //First rotate back to neutral
        if (currentDirection != RotationDirection.None || currentDirection != RotationDirection.Neutral)
        {
            StopRotating();
        }
        currentDirection = rotationDirection;
        PerformRotation(currentDirection);
    }

    public void SetRotations(Quaternion neutralRot, Quaternion leftRot, Quaternion rightRot)
    {
        neutralRotation = neutralRot;
        leftRotation = leftRot;
        rightRotation = rightRot;
    }
    

    public void StartRotating(Quaternion startRot, Quaternion endRot)
    {
        if (!isRotating)
        {
            startRotation = startRot;
            endRotation = endRot;
            isRotating = true;
        }
    }
    
    private void FaceToTurn(GameObject enemy, GameObject player)
    {
        target = player.transform;
        enemy.GetComponent<Rotator>().isEnemy = true;
        if (this.gameObject.tag == "Enemy" && isEnemy)
        {
            directionToTarget = target.position - transform.position;
            if (isAimPoint)
            {
                directionToTarget = target.position - transform.parent.transform.position;
            }
            endRotation = Quaternion.LookRotation(-directionToTarget);
            StartRotating(transform.rotation, endRotation);
        }
        enemy.GetComponent<Rotator>().isEnemy = false;
    }
    
    public void StopRotating()
    {
        currentDirection = RotationDirection.Neutral;
        PerformRotation(currentDirection);
        isRotating = false;
    }
    
    private void FixedUpdate()
    {
        if (debug)
        {
            Quaternion endRotation = Quaternion.Euler(90f, 0f, 0f);
            StartRotating(transform.rotation, endRotation);
        }
        

        if (isRotating)
        {
            float step = rotationSpeed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, endRotation, step);
            // Check if the rotation has reached the endRotation
            /*if (transform.rotation == endRotation)
            {
                isRotating = false;
            }*/
        }

        if (!isRotating)
        {
            StopRotating();
        }
    }

    private Vector3 directionToTarget;
    
    private void Update()
    {
        if (isAimPoint)
        {
            if (GameObject.FindGameObjectWithTag("Player2"))
            {
                float distanceToPlayer1 = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
                float distanceToPlayer2 = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player2").transform.position);
                if (distanceToPlayer1 < distanceToPlayer2)
                {
                    FaceToTurn(this.gameObject, GameObject.FindGameObjectWithTag("Player"));
                }
                else
                {
                    FaceToTurn(this.gameObject, GameObject.FindGameObjectWithTag("Player2"));
                }
            }
            else
            {
                FaceToTurn(this.gameObject, GameObject.FindGameObjectWithTag("Player"));
            }
        }
    }

}
