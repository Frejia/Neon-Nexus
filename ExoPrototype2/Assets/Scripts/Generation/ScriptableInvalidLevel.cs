using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Saves the levels marked as invalid data
/// </summary>
[CreateAssetMenu(fileName = "InvalidLevel", menuName = "InvalidLevel", order = 1)]
public class ScriptableInvalidLevel : ScriptableObject
{
    public int chunkSizeI, chunkSizeZI, offsetI; // LEGACY
    public List<int> chunkSize, chunkSizeZ, offset;

    public List<List<GameObject>> wayPoints;
    public List<bool> raceMode, withCurve, sphere, meshSmoothing;
}
