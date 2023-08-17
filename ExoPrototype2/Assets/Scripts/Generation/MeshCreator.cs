using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Creates a prefab from the procedurally generated Mesh
///
/// Originally created for UnityEditor Level Save Processes, changed later to work within Runtime Builds
/// </summary>
public class MeshCreator : MonoBehaviour
{
    public static MeshCreator Instance;
    [SerializeField] private ScriptableInvalidLevel savedLevels;
    private PerlinNoiseGen perlin;

    private void Awake()
    {
        Instance = this;
    }

    public void EditLevelSave()
    {
        perlin = PerlinNoiseGen.Instance;
        
        // Create a new Instance of the Scriptable Object
        //ScriptableInvalidLevel invalidLevel = ScriptableObject.CreateInstance<ScriptableInvalidLevel>();

        // Set the values of the Scriptable Object
        savedLevels.chunkSize.Add(perlin.chunkSize);
        savedLevels.chunkSizeZ.Add(perlin.chunkSizeZ);
        savedLevels.offset.Add(perlin.offset);
        savedLevels.raceMode.Add(perlin.raceMode);
        savedLevels.withCurve.Add(perlin.withCurve);
        savedLevels.sphere.Add(perlin.sphere);
        savedLevels.meshSmoothing.Add(perlin.meshSmoothing);
        List<GameObject> points = new List<GameObject>();
        foreach(GameObject point in perlin.waypoints)
        {
            points.Add(point);
            
            
        }
        savedLevels.wayPoints.Add(points);
        
        string filePath = Path.Combine(Application.persistentDataPath, "SavedLevel.asset");
        SaveScriptableObject(savedLevels, filePath);
    }

    private void SaveScriptableObject(ScriptableInvalidLevel level, string filePath)
    {
        string json = JsonUtility.ToJson(level);
        File.WriteAllText(filePath, json);
    }
    
    public void LoadSavedLevel(int levelIndex)
    {
        perlin = PerlinNoiseGen.Instance;
        
        string filePath = Path.Combine(Application.persistentDataPath, "SavedLevel.asset");
        if (File.Exists(filePath))
        {
            // Access Data
            string json = File.ReadAllText(filePath);
            savedLevels = JsonUtility.FromJson<ScriptableInvalidLevel>(json);

            // Set the values of the PerlinNoiseGen
            perlin.PerlinSetter(savedLevels.chunkSize[levelIndex],savedLevels.chunkSizeZ[levelIndex], savedLevels.offset[levelIndex], savedLevels.raceMode[levelIndex],
                savedLevels.withCurve[levelIndex], savedLevels.sphere[levelIndex], savedLevels.meshSmoothing[levelIndex], savedLevels.wayPoints[levelIndex]);
            
        }
        else
        {
            Debug.LogWarning("Saved data file not found.");
        }
    }
    
    
    #region Unity Editor Save Process
    #if UNITY_EDITOR
    // https://unitycoder.com/blog/2013/01/26/save-mesh-created-by-script-in-editor-playmode/
    public KeyCode saveKey = KeyCode.F12;
    public bool saveOnStart = false;
    public string saveName = "Meshys";
    [SerializeField] public Transform selectedGameObject;

    void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            Debug.Log("Pressed to Save");
            MakePrefab();
        }
    }

    void SaveAsset(int numofmesh)
    {
        Debug.Log(folderPath);
        // Get How many children are in the selectedGameObject
        int childCount = selectedGameObject.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var mf = selectedGameObject.transform.GetChild(i).GetComponent<MeshFilter>();
            if (mf)
            {
                var savePath = folderPath + saveName + i + ".asset";
                #if UNITY_EDITOR
                AssetDatabase.CreateAsset(mf.mesh, savePath);
                #endif
               
            }
        }
    }
    
    [SerializeField] public string folderPath;
    
    public void MakePrefab()
    {
        //Get Meshy Parent Object
        selectedGameObject = GameObject.Find("Meshys").transform;
        GameObject mesh = selectedGameObject.gameObject;
        //Generate Random Level Number
        int i = Random.Range(0, 1000);
        //Create new Mesh Folder
        AssetDatabase.CreateFolder("Assets/Prefabs/Levels", "Meshes" + i.ToString());
        folderPath = "Assets/Prefabs/Levels/Meshes" + i.ToString() + "/";
        //Create Meshes
        SaveAsset(i);
        
        //If Prefab already exists, do not create it again
        if (AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Levels/" + mesh.name + i + ".prefab", typeof(GameObject)))
        {
            Debug.Log("Prefab already exists");
            return;
        }
        else
        {
            // Create a prefab at the specified path
            string prefabPath = "Assets/Prefabs/Levels/" + mesh.name + i + ".prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(mesh, prefabPath);
            if (prefab != null)
            {
                Debug.Log("Prefab created at: " + prefabPath);
            }
            else
            {
                Debug.LogError("Failed to create prefab.");
            }
        }

    }
    #endif
    #endregion
}