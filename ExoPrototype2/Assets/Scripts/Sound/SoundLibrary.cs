using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class SoundLibrary : MonoBehaviour
{
    [Header("Player Audio Clips")]
    [SerializeField] public List<AudioClip> EffectaudioClips;
    [SerializeField] public List<AudioClip> MusicClips;
    [SerializeField] public AudioSource playerAudioSource;
    [SerializeField] public AudioSource playerBulletAudioSource;
    [SerializeField] public AudioSource musicAudioSource;

    private void OnEnable()
    {
        //Events
        PlayerShoot.Shot += PlaySound;
        ShipMovement.boostInit += PlaySound;
        Health.PlayerHitSound += PlaySound;
        Health.EnemyHitSound += PlaySound;
        Health.PlayerDead += PlaySound;
        Health.EnemyDead += PlaySound;
        NewEnemyInView.OnEnemySeenSound += PlaySound;
    }
    
    private void OnDisable()
    {
        //Events
        PlayerShoot.Shot -= PlaySound;
        ShipMovement.boostInit -= PlaySound;
    }

    private void Start()
    {
        
        // find directory "assets/2d/audio"
        DirectoryInfo dir = new DirectoryInfo("Assets/2D/Audio/Music");
        
        // get all mp3 files in directory
        FileInfo[] info = dir.GetFiles("*.mp3");
        foreach (FileInfo f in info)
        {
            // How do I make them into AudioClips???
        }
        PlayMusic(2);
        //TODO: implement Method that displays all audio files in the directory
        
    }

    private void PlaySound(int soundIndex)
    {
        if (soundIndex < 4)
        {
            playerBulletAudioSource.PlayOneShot(EffectaudioClips[soundIndex]);
        }else
        {
            playerAudioSource.PlayOneShot(EffectaudioClips[soundIndex]);
        }

    }
    
    private void PlayMusic(int index)
    {
        //Play the first sound of the List
        musicAudioSource.clip = MusicClips[index];
        musicAudioSource.Play();
    }
}