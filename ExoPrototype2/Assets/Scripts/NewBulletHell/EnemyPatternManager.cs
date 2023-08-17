using System;
using System.Collections;
using System.Collections.Generic;
using BulletHell;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages enemy bullet firing patterns.
/// Used on all Enemies
/// Needs different Scriptable Objects Bullet Patterns to call from
///
/// Starts when Player is in the Enemy Proximity
/// </summary>
public class EnemyPatternManager : MonoBehaviour
{
    // Pattern manager component
    PatternManager fireBullets;

    // Bullet pattern configurations
    [Header("Bullet Patterns")]
    [SerializeField] private List<BulletPatterns> patterns;
    [SerializeField] private float[] patternDurations;
    public bool useAlternateDurations;

    // Cooldown and firing flags
    [Header("Firing Control")]
    public float Cooldown;
    private bool isOnCooldown;
    public float patternDuration;
    public bool isFiring = false;
    public bool isPlayerClose = false;
    private BulletPool pool;
    
    public delegate void EnemyShootSound(int index);
    public static event EnemyShootSound EnemyShoot;
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize components
        fireBullets = GetComponent<PatternManager>();
        pool = BulletPool.Instance;

        // Subscribe to events
        EnemySeesPlayer.CanSee += PlayerClose;
        EnemySeesPlayer.CantSee += StopPatterns;

        // Start firing patterns
        StartFiringPatterns();
    }

    // Called by Event, if the player is close, start firing patterns
    private void PlayerClose(GameObject enemy)
    {
        if (!isPlayerClose)
        {
            enemy.GetComponent<EnemyPatternManager>().isPlayerClose = true;
            enemy.GetComponent<EnemyPatternManager>().StartFiringPatterns();
        }
    }
    
    // Only start Firing Patterns if the enemy is not already firing or on Cooldown
    private void StartFiringPatterns()
    {
        if (!isFiring && !isOnCooldown)
        {
            StartCoroutine(ReadBulletPatterns());
        }
    }
    
    /// <summary>
    /// Coroutine to read and execute bullet patterns.
    /// </summary>
    private IEnumerator ReadBulletPatterns()
    {
        while (isPlayerClose)
        {
            foreach (BulletPatterns pattern in patterns)
            {
                if (pattern.patternType == BulletPatternEnum.BulletPatternsEnum.None)
                {
                    StopCoroutine(StartPattern(pattern));
                    isFiring = false;
                    if (useAlternateDurations) patternDuration = patternDurations[patterns.IndexOf(pattern)];
                    else patternDuration = pattern.patternDuration;
                    Cooldown = pattern.Cooldown;
                    fireBullets.SetBulletPatternNone();
                    isPlayerClose = false;
                    yield return new WaitForSeconds(pattern.patternDuration);
                }
                else
                {
                    if (!isFiring)
                    {
                        StartCoroutine(StartPattern(pattern));
                    }
                }

                if (Cooldown > 0f)
                {
                    // Set the isOnCooldown flag to true and start the cooldown timer
                    isOnCooldown = true;
                    isFiring = false;
                    isPlayerClose = false;
                    yield return new WaitForSeconds(Cooldown);
                    isOnCooldown = false;
                }
            }
        }
        StopCoroutine(ReadBulletPatterns());

            isFiring = false;
            isPlayerClose = false;
    }

    // Method to start a specific given bullet pattern
    private IEnumerator StartPattern(BulletPatterns pattern)
    {
        
        isFiring = true;
        EnemyShoot(2);
        // Set the pattern duration and cooldown
        if (useAlternateDurations) patternDuration = patternDurations[patterns.IndexOf(pattern)];
        else  patternDuration = pattern.patternDuration;
       // BulletPool.Instance.GetEnemyBulletPrefab().GetComponent<Bullet>().SetSpeed(pattern.BulletSpeed);
        Cooldown = pattern.Cooldown;
        
        // Set the bullet pattern in the Pattern Manager
        fireBullets.SetBulletPattern(pattern.patternType, pattern.bulletBehaviour, pattern.startAngle, pattern.endAngle, 
            pattern.FireRate, pattern.isAiming, pattern.bulletAmount, pattern.BulletSpeed);
        
        // Wait for Pattern to finish Duration
        yield return new WaitForSeconds(pattern.patternDuration);
        
        // Stop Pattern
        fireBullets.SetBulletPatternNone();
    }

    // Stop all patterns in case player is out of proximity or enemy is dead
    private void StopPatterns(GameObject enemy)
    {
        enemy.GetComponent<EnemyPatternManager>().StartFiringPatterns();
        StopCoroutine(enemy.GetComponent<EnemyPatternManager>().ReadBulletPatterns());
        enemy.GetComponent<EnemyPatternManager>().isFiring = false;
        enemy.GetComponent<EnemyPatternManager>().isPlayerClose = false;
    }
}
