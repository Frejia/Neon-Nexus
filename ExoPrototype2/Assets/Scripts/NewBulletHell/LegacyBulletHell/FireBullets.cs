using System.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using BulletHell;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.ProBuilder.MeshOperations;
using Random = UnityEngine.Random;


public class FireBullets : MonoBehaviour
{
    private int bulletsAmount;
    private float startAngle, endAngle;
    private float patternDuration;
    private Vector3 bulletMoveDirection;
    public BulletPatternEnum.BulletPatternsEnum activebulletPattern;
    private BulletHell.BulletBehaviour.BulletBehaviours activebulletBehaviour;
    private float fireRate;
    private Vector3 bulDir;
    
    private float cooldown;
    public bool debug = false;

    private GameObject player;
    public bool isAiming = false;

    private float bulletSpeed;

    public void SetBulletPattern(BulletPatternEnum.BulletPatternsEnum bulletPattern, BulletHell.BulletBehaviour.BulletBehaviours bulletBehaviour, 
        int bullets, float startAngle, float endAngle, bool Aiming, float fireRate, float BulletSpeed)
    {
        activebulletPattern = bulletPattern;
        activebulletBehaviour = bulletBehaviour;
        bulletsAmount = bullets;
        this.startAngle = startAngle;
        this.endAngle = endAngle;
        isAiming = Aiming;
        this.fireRate = fireRate;
        this.bulletSpeed = BulletSpeed;
        PatternSwitch();
    }

    public void SetBulletPatternNone()
    {
        Debug.Log("Bullet Pattern is None");
        CancelInvoke("CircleFire");
        CancelInvoke("Cone");
        CancelInvoke("Straight");
        CancelInvoke("SpiralFire");
        CancelInvoke("DoubleSpiralFire");
        CancelInvoke("Pyramid");
        CancelInvoke("WaveDecel");
        CancelInvoke("WayAllRange");
        activebulletPattern = BulletPatternEnum.BulletPatternsEnum.None;
    }

    public void PatternSwitch()
    {
        switch (activebulletPattern)
        {
            case BulletPatternEnum.BulletPatternsEnum.None:
                //Cooldown
                bulletsAmount = 0;
                fireRate = 0;
                break;
            case BulletPatternEnum.BulletPatternsEnum.Circle:
                //Can Aim
                InvokeRepeating("CircleFire", 0f, fireRate);
                break;
            case BulletPatternEnum.BulletPatternsEnum.Cone:
                //Can Aim
                InvokeRepeating("Cone", 0f, fireRate);
                break;
            case BulletPatternEnum.BulletPatternsEnum.Burst:
                // Not Implemented
                InvokeRepeating("CircleFire", 0f, fireRate);
                break;
            case BulletPatternEnum.BulletPatternsEnum.Straight:
                //Can Aim
                InvokeRepeating("Straight", 0f, fireRate);
                break;
            case BulletPatternEnum.BulletPatternsEnum.Spiral:
                //Cannot Aim
                InvokeRepeating("SpiralFire", 0f, fireRate);
                break;
            case BulletPatternEnum.BulletPatternsEnum.DoubleSpiral:
                //Cannot Aim
                InvokeRepeating("DoubleSpiralFire", 0f, fireRate);
                break;
            case BulletPatternEnum.BulletPatternsEnum.Pyramid:
                //Can Aim
                InvokeRepeating("Pyramid", 0f, fireRate);
                break;
            case BulletPatternEnum.BulletPatternsEnum.WaveDecel:
                InvokeRepeating("WaveDecel", 0f, fireRate);
                break;
            case BulletPatternEnum.BulletPatternsEnum.WayAllRange:
                InvokeRepeating("WayAllRange", 0f, fireRate);
                break;
            /*case BulletPatterns.Aimed:
                bulletsAmount = 10;
                startAngle = 0f;
                endAngle = 360f;
                InvokeRepeating("Aimed", 0f, fireRate);
                break;*/
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        if (debug)
        {
            fireRate = 0.1f;
            PatternSwitch();
        }
    }

    private void Cone()
    {
        float angleStep = (endAngle - startAngle) / bulletsAmount;
        float angle = startAngle;

        for (int i = 0; i < bulletsAmount + 1; i++)
        {
            // Calculate the direction towards the player using Aimed() method
            Vector3 bulDir = isAiming ? (Aim()) : RandomAim();

            // Rotate the bullet direction by the cone pattern angle around the y-axis
            Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
            bulDir = rotation * bulDir;

            // Rotate the bullet direction around the x-axis
            float rotatedX = bulDir.x * Mathf.Cos(rotation.eulerAngles.x * Mathf.Deg2Rad) - bulDir.y * Mathf.Sin(rotation.eulerAngles.x * Mathf.Deg2Rad);
            float rotatedY = bulDir.x * Mathf.Sin(rotation.eulerAngles.x * Mathf.Deg2Rad) + bulDir.y * Mathf.Cos(rotation.eulerAngles.x * Mathf.Deg2Rad);
            bulDir.x = rotatedX;
            bulDir.y = rotatedY;

            // Create a new vector with the rotated direction
           // bulDir.Normalize();

            // Spawn and set direction for the bullet
            GameObject bul = BulletPool.Instance.GetBulletEnemy();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<Bullet>().SetSpeed(bulletSpeed);
            bul.GetComponent<Bullet>().SetDirection(bulDir);
            
            angle += angleStep;
        }
    }
    
    private void PwettyPattern()
    {
        float angleStep = (endAngle - startAngle) / bulletsAmount;
        float angle = startAngle;

        for (int i = 0; i < bulletsAmount + 1; i++)
        {
            // Calculate the direction towards the player using Aimed() method
            Vector3 bulDir = isAiming ? (Aim()) : RandomAim();

            // Rotate the bullet direction by the cone pattern angle
            float rotatedAngle = angle * Mathf.Deg2Rad;
            float cosAngle = Mathf.Cos(rotatedAngle);
            float sinAngle = Mathf.Sin(rotatedAngle);

            // Rotate the direction vector
            float rotatedX = bulDir.x * cosAngle - bulDir.y * sinAngle;
            float rotatedY = bulDir.x * sinAngle + bulDir.y * cosAngle;

            // Create a new vector with the rotated direction
            bulDir = new Vector3(rotatedX, rotatedY);

            // Spawn and set direction for the bullet
            GameObject bul = BulletPool.Instance.GetBulletEnemy();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<Bullet>().SetDirection(bulDir);
            bul.GetComponent<Bullet>().SetSpeed(bulletSpeed);
            bul.GetComponent<BulletHell.BulletBehaviour>().SetBehaviour(BulletHell.BulletBehaviour.BulletBehaviours.SineCurve, bulDir);
            //bul.GetComponent<Bullet>().SetDirection(bulDir);
            
            angle += angleStep;
        }
    }
    
    private void CircleFire()
    {
        float angleStep = (endAngle - startAngle) / bulletsAmount;
        float angle = startAngle;

        for (int i = 0; i < bulletsAmount + 1; i++)
        {
            // Calculate the direction towards the player using Aimed() method
            Vector3 bulDir = isAiming ? (Aim()) : RandomAim();

            // Rotate the bullet direction by the cone pattern angle
            float rotatedAngle = angle * Mathf.Deg2Rad;
            float cosAngle = Mathf.Cos(rotatedAngle);
            float sinAngle = Mathf.Sin(rotatedAngle);

            // Rotate the direction vector
            float rotatedX = bulDir.x * cosAngle - bulDir.y * sinAngle;
            float rotatedY = bulDir.x * sinAngle + bulDir.y * cosAngle;

            // Create a new vector with the rotated direction
            bulDir = new Vector3(rotatedX, rotatedY);

            // Spawn and set direction for the bullet
            GameObject bul = BulletPool.Instance.GetBulletEnemy();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<Bullet>().SetSpeed(bulletSpeed);
            bul.GetComponent<Bullet>().SetDirection(bulDir);
            bul.GetComponent<BulletHell.BulletBehaviour>().SetBehaviour(activebulletBehaviour, bulDir);
            //bul.GetComponent<Bullet>().SetDirection(bulDir);
            
            angle += angleStep;
        }
    }

    private float angle = 0f;

    private void SpiralFire()
    {
        float bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
        float bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

        Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
        bulDir = (bulMoveVector - transform.position).normalized;

        GameObject bul = BulletPool.Instance.GetBulletEnemy();
        bul.transform.position = transform.position;
        bul.transform.rotation = transform.rotation;
        bul.SetActive(true);
        bul.GetComponent<Bullet>().SetSpeed(bulletSpeed);
        bul.GetComponent<Bullet>().SetDirection(bulDir);

        angle += 10f;
    }

    private void DoubleSpiralFire()
    {
        for (int i = 0; i <= 1; i++)
        {
            float bulDirX = transform.position.x + Mathf.Sin(((angle + 180f * i) * Mathf.PI) / 180f);
            float bulDirY = transform.position.y + Mathf.Cos(((angle + 180f * i) * Mathf.PI) / 180f);

            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
             bulDir = (bulMoveVector - transform.position).normalized;

             GameObject bul = BulletPool.Instance.GetBulletEnemy();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<Bullet>().SetSpeed(bulletSpeed);
            bul.GetComponent<Bullet>().SetDirection(bulDir);
        }

        angle += 10f;

        if (angle >= 360f) angle = 0f;
    }

    private float pyramidSize = 2f;

    // Ripple Wave Effect
    public void Pyramid()
    {
        int numRows = Mathf.CeilToInt(Mathf.Sqrt(bulletsAmount));

        // Calculate the spacing between each row and column
        float rowSpacing = pyramidSize / (numRows - 1);
        float colSpacing = pyramidSize / 2f;
        int bulletsInBottomRow = 5;

        float rotationAngle = -Aim().x * Mathf.Rad2Deg;
        
        // Calculate the total number of bullets in the pyramid
        int totalBullets = 0;
        for (int row = 0; row < numRows; row++)
        {
            totalBullets += bulletsInBottomRow - row;
        }

        // Calculate the rotation angle for the entire pyramid
        Quaternion rotation = Quaternion.Euler(0f, 0f, rotationAngle);

        int bulletCount = 0;
        for (int row = 0; row < numRows; row++)
        {
            int bulletsInRow = bulletsInBottomRow - row;
            float xOffset = -(bulletsInRow - 1) * colSpacing * 0.5f;
            float yOffset = row * rowSpacing;

            for (int col = 0; col < bulletsInRow; col++)
            {
                float xPosition = col * colSpacing + xOffset;
                float yPosition = yOffset;

                Vector3 position = rotation * new Vector3(xPosition, yPosition, 0f) + transform.position; ;
                
                GameObject bul = BulletPool.Instance.GetBulletEnemy();
                bul.transform.position = position;
                bul.transform.rotation = transform.rotation;
                bul.SetActive(true);
                bul.GetComponent<Bullet>().SetSpeed(bulletSpeed);
                bul.GetComponent<Bullet>().SetDirection((position - transform.position).normalized);
                bul.GetComponent<BulletHell.BulletBehaviour>().SetBehaviour(activebulletBehaviour, bulDir);
                
                bulletCount++;

                if (bulletCount >= totalBullets)
                {
                    // If we've spawned all bullets, exit the loop early
                    return;
                }
            }
        }
    }

    private void Pyramid2()
    {
        // Calculate the total number of rows in the pyramid
        int numRows = Mathf.CeilToInt(Mathf.Sqrt(bulletsAmount));

        // Calculate the spacing between each row and column
        float rowSpacing = pyramidSize / (numRows - 1);
        float colSpacing = pyramidSize / 2f;
        float rotationAngle = -Aim().x * Mathf.Rad2Deg;
        
        int bulletCount = 0;
        for (int row = 0; row < numRows; row++)
        {
            int bulletsInRow = bulletsAmount - bulletCount < numRows ? bulletsAmount - bulletCount : numRows - row;
            float xOffset = -(bulletsInRow - 1) * colSpacing * 0.5f;
            float yOffset = row * rowSpacing;

            for (int col = 0; col < bulletsInRow; col++)
            {
                float xPosition = col * colSpacing + xOffset;
                float yPosition = yOffset;

                Quaternion rotation = Quaternion.Euler(0f, 0f, rotationAngle);
                Vector3 position = rotation * new Vector3(xPosition, yPosition, 0f) + transform.position;

                GameObject bul = BulletPool.Instance.GetBulletEnemy();
                bul.transform.position = position;
                bul.transform.rotation = transform.rotation;
                bul.SetActive(true);
                bul.GetComponent<Bullet>().SetSpeed(bulletSpeed);
                bul.GetComponent<Bullet>().SetDirection((position - transform.position).normalized);

                bulletCount++;

                if (bulletCount >= bulletsAmount)
                {
                    // If we've spawned all bullets, exit the loop early
                    return;
                }
            }
        }
    }

    private float waveAmplitude = 40f;

    private void WaveDecel()
    {
        float angleStep = (endAngle - startAngle) / bulletsAmount;
        float angle = startAngle;

        for (int i = 0; i < bulletsAmount + 1; i++)
        {
            // Calculate the direction towards the player using Aimed() method
            bulDir = isAiming ? (Aim()) : RandomAim();

            // Rotate the bullet direction by the cone pattern angle
            float rotatedAngle = angle * Mathf.Deg2Rad;
            float cosAngle = Mathf.Cos(rotatedAngle);
            float sinAngle = Mathf.Sin(rotatedAngle);

            // Rotate the direction vector
            float rotatedX = bulDir.x * cosAngle - bulDir.y * sinAngle;
            float rotatedY = bulDir.x * sinAngle + bulDir.y * cosAngle;

            // Create a new vector with the rotated direction
            bulDir = new Vector3(rotatedX, rotatedY);
            
            /*float bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f) * waveAmplitude;
            float bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f) * waveAmplitude;

            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            bulDir = (bulMoveVector - transform.position).normalized;*/

            GameObject bul = BulletPool.Instance.GetBulletEnemy();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<Bullet>().SetSpeed(bulletSpeed);
            bul.GetComponent<Bullet>().SetDirection(bulDir);
            bul.GetComponent<BulletHell.BulletBehaviour>().SetBehaviour(activebulletBehaviour, bulDir);
            angle += angleStep;
        }
    }

    private float rangeRadius = 2f;
    [SerializeField] private float turnAllRange = 200f;

    private void WayTurnAllRange()
    {
        float angleStep = (endAngle - startAngle) / bulletsAmount;
        float angle = startAngle;

        for (int i = 0; i < bulletsAmount + 1; i++)
        {
            // Calculate the rotation angle for each bullet
            float rotationAngle = angle + (turnAllRange * i);

            // Calculate the position of the bullet in the turned range
            float bulDirX = transform.position.x + Mathf.Cos((rotationAngle * Mathf.PI) / 180f) * rangeRadius;
            float bulDirY = transform.position.y + Mathf.Sin((rotationAngle * Mathf.PI) / 180f) * rangeRadius;

            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            Vector3 bulDir = (bulMoveVector - transform.position).normalized;

            GameObject bul = BulletPool.Instance.GetBulletEnemy();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<Bullet>().SetSpeed(bulletSpeed);
            bul.GetComponent<Bullet>().SetDirection(bulDir);

            angle += angleStep;
        }
    }

    void WayAllRange()
    {
        float angle = 0;
        while (angle > -2 * Mathf.PI)
        {
            float bulDirX = transform.position.x + Mathf.Cos(angle) * 20;
            float bulDirY = transform.position.y + Mathf.Sin(angle) * 20;
            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            bulDir = (bulMoveVector - transform.position).normalized;

            GameObject bul = BulletPool.Instance.GetBulletEnemy();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<Bullet>().SetSpeed(bulletSpeed);
            bul.GetComponent<Bullet>().SetDirection(bulDir);
            bul.GetComponent<BulletHell.BulletBehaviour>().SetBehaviour(activebulletBehaviour, bulDir);
            angle = (float)(angle - Mathf.PI / 2f);
        }
    }

    private Vector3 Aim()
    {
        //Calculate Angle for Direction of player from this gameobject position in a 3D Space, including the z axis
        Vector3 direction = player.transform.position - transform.position;
        return direction.normalized;
    }

    private Vector3 RandomAim()
    {
        float bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
        float bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

        Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
        
        bulDir = (bulMoveVector - transform.position).normalized;
        return bulDir;
    }

    private void Straight()
    {
        float angle = (float)Math.Atan2(player.transform.position.y - transform.position.y,
            player.transform.position.x - transform.position.x);
        for (int i = 0; i <= bulletsAmount; i++)
        {
            float bulDirX = transform.position.x + Mathf.Cos(angle) * 10;
            float bulDirY = transform.position.y + Mathf.Sin(angle) * 10;
            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            Vector3 bulDir = (bulMoveVector - transform.position).normalized;
            GameObject bul = BulletPool.Instance.GetBulletEnemy();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<Bullet>().SetSpeed(bulletSpeed);
            bul.GetComponent<Bullet>().SetDirection(bulDir);
            bul.GetComponent<BulletHell.BulletBehaviour>().SetBehaviour(activebulletBehaviour, bulDir);

        }
    }


}
