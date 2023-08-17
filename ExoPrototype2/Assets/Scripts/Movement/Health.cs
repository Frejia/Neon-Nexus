using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Health Stats of Enemies and Players
///
/// When the Object gets hit, it will take damage and check if it is dead
/// Sends to GameManager whether player is dead or an enemy is dead to count points
/// </summary>
public class Health : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public float currentHealth;

    [SerializeField] private GameObject attacker;
    
    public delegate void Hit(GameObject enemy, GameObject attacker);

    public delegate void HitSound(int index);
    public static event Hit EnemyGotHit;
    public static event Hit PlayerGotHit;
    public static event HitSound PlayerHitSound;
    public static event HitSound PlayerDead;
    public static event HitSound EnemyHitSound;
    public static event HitSound EnemyDead;
    
    
    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void GetsHit(int damage, GameObject attacker)
    {
        this.attacker = attacker;
        currentHealth -= damage;
        if (currentHealth > 0){
        if (this.gameObject.tag == "Enemy")
        {
            EnemyHitSound(8);
        }
        else
        {
            PlayerHitSound(8);
        }
        }
        
        if (currentHealth < 0) currentHealth = -1;
            // Debug.Log(currentHealth);
        CheckDeath();
    }

    private void CheckDeath()
    {
        if(currentHealth <= 0)
        {
            //Dissolve Shield
            if (this.gameObject.tag == "Enemy")
            {
                this.GetComponent<Dissolve>().isdissolving = true;
                EnemyGotHit(this.transform.parent.gameObject, attacker);
                EnemyDead(7);
                //Destroy Object
                Destroy(this.transform.parent, 5f);
            }
            
            if (this.gameObject.tag == "Player")
            {
                PlayerGotHit(this.gameObject, attacker);
                PlayerDead(4);
                // GameManager.Instance.SetLose();
            }
            if (this.gameObject.tag == "Player2")
            {
                PlayerGotHit(this.gameObject, attacker);
                PlayerDead(4);
                // GameManager.Instance.SetLose();
            }
           
        }
    }
}
