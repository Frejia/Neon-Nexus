using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;
    [SerializeField] public float speed = 30f;
    private float savedSpeed;
    [SerializeField] private float _bulletLifeTime = 4f;

    [SerializeField] private ParticleSystem impactEffect;

    [SerializeField, Range(10, 20)] private int _damage = 10;

    public bool friendlyFire = false;
    private GameObject attacker;

    public void SetUser(GameObject user)
    {
        attacker = user;
    }

    private void Awake()
    {
        savedSpeed = speed;
    }

    private void OnEnable()
    {
        speed = savedSpeed;
        Invoke("Destroy", _bulletLifeTime);
        impactEffect.Stop();
    }
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
    
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    /*public void SetFriendlyFire()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int player2Layer = LayerMask.NameToLayer("Player2");
        
        // renderer.enabled = true;
        if (friendlyFire && attacker.CompareTag("Player"))
        {
            foreach (Collider col in colliders)
            {
                // Set the layer collision settings for the triggerCollider
                Physics.IgnoreLayerCollision(playerLayer, player2Layer, true);
            }
        }
        else
        {
            foreach (Collider col in colliders)
            {
                // Set the layer collision settings for the triggerCollider
                Physics.IgnoreLayerCollision(player2Layer, playerLayer, true);
            }
        }
    }*/

    private void Destroy()
    {
        attacker = null;
        gameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        attacker = null;
        CancelInvoke();
    }

    /*private void OnCollisionEnter(Collision other)
    {
        impactEffect.Play();
        StartCoroutine(WaitForParticleSystem());
    } */

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13)
        {
            if (friendlyFire && attacker.gameObject.tag == "Player")
            {
                if (other.GetComponent<Health>() != null)
                {
                    other.GetComponent<Health>().GetsHit(_damage, attacker);
                }

                impactEffect.Play();
                StartCoroutine(WaitForParticleSystem());
                speed = 0f;
            }
            
            if (attacker.gameObject.tag == "Enemy")
            {
                if (other.GetComponent<Health>() != null)
                {
                    other.GetComponent<Health>().GetsHit(_damage, attacker);
                }

                impactEffect.Play();
                StartCoroutine(WaitForParticleSystem());
                speed = 0f;
            }
        }
        
        if (other.gameObject.layer == 8)
        {
            if (friendlyFire && attacker.gameObject.tag == "Player2")
            {
                if (other.GetComponent<Health>() != null)
                {
                    other.GetComponent<Health>().GetsHit(_damage, attacker);
                }

                impactEffect.Play();
                StartCoroutine(WaitForParticleSystem());
                speed = 0f;
            }

            if (attacker.gameObject.tag == "Enemy")
            {
                if (other.GetComponent<Health>() != null)
                {
                    other.GetComponent<Health>().GetsHit(_damage, attacker);
                }

                impactEffect.Play();
                StartCoroutine(WaitForParticleSystem());
                speed = 0f;
            }
        }
        
        if (other.gameObject.layer == 7)
        {
            if (other.GetComponent<Health>() != null)
            {
                other.GetComponent<Health>().GetsHit(_damage, attacker);
            }
            impactEffect.Play();
                StartCoroutine(WaitForParticleSystem());
                speed = 0f;
        }
    }

    public void SetSpeed(float newSpeed)
    {
        this.speed = newSpeed;
    }

    private IEnumerator WaitForParticleSystem()
    {
        //renderer.enabled = false;
        yield return new WaitForSeconds(impactEffect.main.duration);
        Destroy();
    }
}
