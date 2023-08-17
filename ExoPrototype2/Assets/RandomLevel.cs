using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a number of random values in ranges for the Perlin Noise Generator to use
/// </summary>
public class RandomLevel : MonoBehaviour
{
    [Header("RandomValues")] [SerializeField]
    private GameObject perlin;
    private int chunkSize, chunkSizeZ, offset;

    private List<GameObject> wayPoints;
    private bool raceMode, withCurve, sphere, meshSmoothing;
    
    // Generate Random Values for the Perlin Noise Generator
    // Executed when pressing Generate Random Level Button
    public void GenerateRandomLevel()
    {
        GenerateChunkSize();
      //  GenerateRandomWayPoints();
        
        perlin.GetComponent<PerlinNoiseGen>().PerlinSetter(chunkSize,chunkSizeZ, offset, raceMode,
            withCurve, sphere, meshSmoothing, wayPoints);
    }

    // Set if Level for Racing Mode or not
    public void SetRaceMode(bool raceModeOn)
    {
        raceMode = raceModeOn;
        withCurve = raceModeOn;
        chunkSizeZ = 200;
    }

    // Generate a random ChunkSize, ChunkSizeZ and Offset
    private void GenerateChunkSize()
    {
        chunkSize = Random.Range(20, 100);
        chunkSizeZ = Random.Range(20, 100);
        offset = Random.Range(0, 30);
    }

    // Random waypoints for portals and the racing line to follow
    private void GenerateRandomWayPoints()
    {
        //Generate 6 Random Waypoints in the grid area
        for (int i = 0; i < 6; i++)
        {
            int x = Random.Range(0, chunkSize);
            int y = Random.Range(0, chunkSize);
            int z = Random.Range(0, chunkSizeZ);
            Vector3 pos = new Vector3(x, y, z);
            GameObject point = new GameObject("Waypoint" + i);
            point.transform.position = pos;
            wayPoints.Add(point);
        }
    }
    
}
