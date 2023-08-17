using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell
{
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool Instance { get; private set; }

        [SerializeField] private GameObject pooledBulletEnemy;
        [SerializeField] private GameObject pooledBulletPlayer;
        private bool notEnoughBulletsInPool = true;
        
        private List<GameObject> bulletsEnemy;
        private List<GameObject> bulletsPlayer;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            bulletsEnemy = new List<GameObject>();
            bulletsPlayer = new List<GameObject>();
        }

        public GameObject GetEnemyBulletPrefab()
        {
            return pooledBulletEnemy;
        }
        public GameObject GetPlayerBulletPrefab()
        {
            return pooledBulletPlayer;
        }
        
        public GameObject GetBulletEnemy()
        {
            if (bulletsEnemy.Count > 0)
            {
                for (int i = 0; i < bulletsEnemy.Count; i++)
                {
                    if (!bulletsEnemy[i].activeInHierarchy)
                    {
                        return bulletsEnemy[i];
                    }
                }
            }
            if (notEnoughBulletsInPool)
            {
                GameObject bullet = Instantiate(pooledBulletEnemy);
                bullet.SetActive(false);
                bulletsEnemy.Add(bullet);
                return bullet;
            }
            return null;
        }
        
        public GameObject GetBulletPlayer()
        {
            if (bulletsPlayer.Count > 0)
            {
                for (int i = 0; i < bulletsPlayer.Count; i++)
                {
                    if (!bulletsPlayer[i].activeInHierarchy)
                    {
                        return bulletsPlayer[i];
                    }
                }
            }
            if (notEnoughBulletsInPool)
            {
                GameObject bulletplayer = Instantiate(pooledBulletPlayer);
                bulletplayer.SetActive(false);
                bulletsPlayer.Add(bulletplayer);
                return bulletplayer;
            }
            return null;
        }
    }
}