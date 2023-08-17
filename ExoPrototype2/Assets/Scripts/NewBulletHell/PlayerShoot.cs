using System;
using System.Collections;
using System.Collections.Generic;
using BulletHell;
using Sirenix.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player Shooting makes the player shoot bullets in given patterns
/// </summary>
public class PlayerShoot : MonoBehaviour
{
    [Header("Shoot Control Settings")] 
    [SerializeField] private float cooldown1, cooldown2;
    public bool shooting;
    public bool shotSpecial;
    public bool shotSpecial2;
    public bool isFiring = false;

    // Pattern Reference
    private PatternManager patternManager;
    public delegate void Shoot(int i);
    public static event Shoot Shot;

    // Start is called before the first frame update
    void Start()
    {
        patternManager = this.gameObject.GetComponent<PatternManager>();
    }
    
    void FixedUpdate()
     {
         HandleShooting();
     }

     // Start Pattern based on Input
     private void HandleShooting()
     {
         if (shooting)
         {
             Shot(0);
             StartCoroutine(StartPattern(0));
         }

         if (shotSpecial)
         {
             Shot(1);
             StartCoroutine(StartPattern(1));
         }

         if (shotSpecial2)
         {
             Shot(2);
             StartCoroutine(StartPattern(2));
         }
         
     }

    // Start Pattern and send events to sound manager
     private IEnumerator StartPattern(int shot)
     {
         isFiring = true;
         if (shot == 0)
         {
             // Straight Pattern Example
             patternManager.SetBulletPattern(BulletPatternEnum.BulletPatternsEnum.Straight, BulletBehaviour.BulletBehaviours.None, 0,0,5f, false, 1, 10f);

             if (GetComponent<PlayerAim>().target != null)
             {
                 patternManager.SetBulletPattern(BulletPatternEnum.BulletPatternsEnum.Straight, BulletBehaviour.BulletBehaviours.None, 0,0,5f, true, 1, 10f);

             }
             
         }
         else if(shot == 1)
         {
             // Cone Pattern Example
            
             patternManager.SetBulletPattern(BulletPatternEnum.BulletPatternsEnum.Cone, BulletBehaviour.BulletBehaviours.None, 40,90, 2f, true, 10, 10f);
         }
         else
         {
             // Circle Pattern Example
             patternManager.SetBulletPattern(BulletPatternEnum.BulletPatternsEnum.Circle, BulletBehaviour.BulletBehaviours.None, 0,360, 2f, true, 20, 10f);
         }
         
         yield return new WaitForSeconds(0.3f);
         patternManager.SetBulletPatternNone();
         isFiring = false;
     }
     
     // ------ INPUT METHODS -------
    public void OnShoot(InputAction.CallbackContext context)
    {
        shooting = context.performed;
    }
    public void OnShootSpecial(InputAction.CallbackContext context)
    {
        shotSpecial = context.performed;
    }
    public void OnShootSpecial2(InputAction.CallbackContext context)
    {
        shotSpecial2 = context.performed;
    }
    
}
