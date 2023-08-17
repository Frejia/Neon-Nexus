using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMove : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public float fireRate;

    private void Update()
    {
        if (speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        else
        {
            Debug.Log("No Speed");
        }
    }

    void Start()
    {
        StartCoroutine(projectileTime());
    }

    [SerializeField] public float projectileDespawnTime = 10f;

    private IEnumerator projectileTime()
    {
        Debug.Log("Projectile Shot");
        yield return new WaitForSeconds(projectileDespawnTime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        speed = 0;
        Destroy(gameObject);
    }
}
