using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object Bullet Pattern with Settings for different variations and settings
/// </summary>
[CreateAssetMenu(fileName = "new Bulletpattern", menuName = "Bulletpattern")]
public class BulletPatterns : ScriptableObject
{
    public float Cooldown;
    public float patternDuration;
    
    public string patternName;
    public BulletPatternEnum.BulletPatternsEnum patternType;
    public BulletHell.BulletBehaviour.BulletBehaviours bulletBehaviour;
    public int bulletAmount;

    public float startAngle, endAngle;
    public float FireRate;
    public float BulletSpeed = 10f;
    public bool isAiming;
    
    public Vector2 direction;
}

