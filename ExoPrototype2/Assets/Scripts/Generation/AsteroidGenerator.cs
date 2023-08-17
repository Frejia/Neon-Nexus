using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AsteroidGenerator, originially from the PathFinding project, but modified and commented by Fanny
/// avoids area of Procedurally spawned level to spawn asteroids around it, asteroids are Meshes applied with Perlin Noise 
/// </summary>
public class AsteroidGenerator : MonoBehaviour
{
    [Header("Asteroid Spawning")]
    public float spawnRange;
    public float amountToSpawn;
    private Vector3 spawnPoint;
    public GameObject asteroid;
    public float startSafeRange;
    private List<GameObject> objectsToPlace = new List<GameObject>();

    private void SpawnAsteroids()
    {
        // Pick a Spawn Point for each asteroid
        for (int i = 0; i < amountToSpawn; i++)
        {
            PickSpawnPoint();

            //pick new spawn point if too close to player start
            while (Vector3.Distance(spawnPoint, Vector3.zero) < startSafeRange)
            {
                PickSpawnPoint();
            }
            
            objectsToPlace.Add(Instantiate(asteroid, spawnPoint, Quaternion.Euler(Random.Range(0f,360f), Random.Range(0f, 360f), Random.Range(0f, 360f))));
            objectsToPlace[i].GetComponent<MeshSmoothing>().ApplyPerlin_Hard(objectsToPlace[i]);
            objectsToPlace[i].transform.parent = this.transform;
        }

        asteroid.SetActive(false);
    }

    // Generate random Spawn Point
    public void PickSpawnPoint()
    {
        spawnPoint = new Vector3(
            Random.Range(-1f,1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f));

        if(spawnPoint.magnitude > 1)
        {
            spawnPoint.Normalize();
        }

        spawnPoint *= spawnRange;
    }
}

