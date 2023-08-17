using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using PathCreation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

/// <summary>
/// PerlinNoiseGen handles full Perlin Noise Terrain Generation for Level Generation
/// used with Game Mode Manager, needs to run and be done before the Pathfinding is initialized
/// Resource Video: https://youtu.be/TZFv493D7jo
/// Changed aspects by Fanny: Sphere Settings, Commenting and code refactoring,
/// Additional Gizmo and Race Mode (including PointGen and RemoveCubesWithinRadius),
/// MeshSmoothing, Collider, Waypoints, Prism instead of Cube Prefab, Save Mesh, Invalid Levels
///
/// Legacy: Repeated Attempts at trying to make this into Marching Cubes --> failed and not in final version
/// Reason for using this and not other Marching Cubes: see paper, section "problems"
/// </summary>
public class PerlinNoiseGen : MonoBehaviour
{
    public static PerlinNoiseGen Instance { get; private set; }
    public bool isDone { get; private set; }
    
    [Header("PerlinNoise Settings")]
    [SerializeField] public int offset = 0;
    [SerializeField] private float noiseScale = .05f;
    [SerializeField, Range(0, 1)] private float threshold = .5f;
    
    [Header("Generator Settings")]
    [SerializeField] private GameObject blockPrefab;//use a unit cube (1x1x1 like unity's default cube)
    [SerializeField] private Material material;
    
    [SerializeField] public int chunkSize = 50;
    [SerializeField] public int chunkSizeZ = 50;
    
    [SerializeField] public bool sphere = false;
    [SerializeField] public bool meshSmoothing;
    [SerializeField] private bool collider = false;
    
    [Header("RaceMode")]
    [SerializeField] public bool raceMode = false;
    [SerializeField] public bool withCurve;
    [SerializeField] private int segments;
    [SerializeField] private float pathradius = 10f;
    [SerializeField] public List<GameObject> waypoints; 
    private List<Vector3> interpolatedPoints = new List<Vector3>();
    
    // Other Variables
    private List<Mesh> meshes = new List<Mesh>(); //used to avoid memory issues
    private float timePassed = 0f; // measuring how long generation time is
    private InvalidLevelSafe invalidLevelSafe; // Get marked Invalid Levels
    private List<CombineInstance> blockData;//this will contain the data for the final mesh
    private List<List<CombineInstance>> blockDataLists;
    
    /// <summary>
    /// Shows bounds of Grids and Lines, for Debugging and Editor Mode
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(48.4290009f,53.2000008f,49.9000015f), new Vector3(chunkSize, chunkSize, chunkSizeZ));
        Gizmos.DrawWireCube(new Vector3(214f,229f,438.75f), new Vector3(chunkSize * 5, chunkSize * 5, chunkSizeZ * 5));
       
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i+1].transform.position);
            Gizmos.DrawLine(waypoints[i].transform.position * 5, waypoints[i+1].transform.position * 5);
        }
    }
    
    
    private void Awake()
    {
        // Check if there are Invalid Levels and compare if currently generated values is one of the invalid ones.
        /*invalidLevelSafe = GetComponent<InvalidLevelSafe>();
        invalidLevelSafe.LoadAndCompareCustomData();*/
        
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }

    /// <summary>
    /// For loading Saved Levels and generating Random Level Values
    /// </summary>
    public void PerlinSetter(int chunkSize, int chunkSizeZ, int offset, bool raceMode, 
        bool withCurve, bool sphere, bool meshSmoothing, List<GameObject> wayPoints)
    {
        this.chunkSize = chunkSize;
        this.chunkSizeZ = chunkSizeZ;
        this.offset = offset;
        this.raceMode = raceMode;
        this.withCurve = withCurve;
        this.sphere = sphere;
        this.meshSmoothing = meshSmoothing;
       /* foreach(GameObject point in wayPoints)
        {
            this.waypoints.Add(point);
        }*/
        Debug.Log("Setted Saved Level Stats");
       // invalidLevelSafe.LoadAndCompareCustomData();
    }

    /// <summary>
    /// Generates the terrain mesh based on Perlin noise and settings.
    /// Used for entire Level Generation
    /// </summary>
    public void Generate()
    {
        Debug.Log("Generating Terrain!");
        isDone = false;
        float startTime = Time.realtimeSinceStartup;

        // Uses Perlin Noise Data to generate Grid and values within chunk 
        #region Create Mesh Data

        blockData = new List<CombineInstance>();//this will contain the data for the final mesh
        MeshFilter blockMesh = Instantiate(blockPrefab, transform.position, Quaternion.identity).GetComponent<MeshFilter>();//create a unit cube and store the mesh from it

        //go through each block position
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                for (int z = 0; z < chunkSizeZ; z++) {

                    float noiseValue = Perlin3D((x + offset) * noiseScale, (y + offset) * noiseScale, (z + offset) * noiseScale);//get value of the noise at given x, y, and z.
                    if (noiseValue >= threshold) {//is noise value above the threshold for placing a block?

                        //ignore this block if it's a sphere and it's outside of the radius (ex: in the corner of the chunk, outside of the sphere)
                        //distance between the current point with the center point. if it's larger than the radius, then it's not inside the sphere.
                        float radius = chunkSize / 2;
                        if (sphere && Vector3.Distance(new Vector3(x, y, z), Vector3.one * radius) > radius)
                            continue;

                        blockMesh.transform.position = new Vector3(x, y, z);//move the unit cube to the intended position
                        CombineInstance ci = new CombineInstance {//copy the data off of the unit cube
                            mesh = blockMesh.sharedMesh,
                            transform = blockMesh.transform.localToWorldMatrix,
                        };
                        blockData.Add(ci);//add the data to the list
                    }

                }
            }
        }
        // Fanny: RACE MODE --- Get Points around which blocks are deleted before making the final mesh
        if (raceMode)
        {
            //NOTE: Normal Generation takes ~15 Seconds, RaceTrack Generation up to 1:30 Minutes (Without enemies, and 
            
            // If curve is true, then generate the Bezier points, if not, just use the given points
            if (withCurve)
            {
                for (int i = 0; i < waypoints.Count - 1; i++)
                {
                    GenerateLinearPoints(waypoints[i].transform.position, waypoints[i+1].transform.position, segments);
                }
            }
            else
            {
                foreach (GameObject point in waypoints)
                {
                    interpolatedPoints.Add(point.transform.position);
                }
            }
            
            //Remove Cubes
            RemoveCubesWithinRadius(interpolatedPoints, pathradius);
        }
        
        Destroy(blockMesh.gameObject);//original unit cube is no longer needed. we copied all the data we need to the block list.

        #endregion
        
        #region Separate Mesh Data

        //divide meshes into groups of 65536 vertices. Meshes can only have 65536 vertices so we need to divide them up into multiple block lists.

        blockDataLists = new List<List<CombineInstance>>();//we will store the meshes in a list of lists. each sub-list will contain the data for one mesh. same data as blockData, different format.
        int vertexCount = 0;
        blockDataLists.Add(new List<CombineInstance>());//initial list of mesh data
        for (int i = 0; i < blockData.Count; i++) {//go through each element in the previous list and add it to the new list.
            vertexCount += blockData[i].mesh.vertexCount;//keep track of total vertices
            if (vertexCount > 65536) {//if the list has reached it's capacity. if total vertex count is more then 65536, reset counter and start adding them to a new list.
                vertexCount = 0;
                blockDataLists.Add(new List<CombineInstance>());
                i--;
            } else {//if the list hasn't yet reached it's capacity. safe to add another block data to this list 
                blockDataLists.Last().Add(blockData[i]);//the newest list will always be the last one added
            }
        }

        #endregion

        //Create final Meshes
        #region Create Mesh

        Transform container = new GameObject("Meshys").transform;//create container object
        foreach (List<CombineInstance> data in blockDataLists) {//for each list (of block data) in the list (of other lists)
            GameObject g = new GameObject("Meshy");//create gameobject for the mesh
            g.transform.parent = container;//set parent to the container we just made
            MeshFilter mf = g.AddComponent<MeshFilter>();//add mesh component
            MeshRenderer mr = g.AddComponent<MeshRenderer>();//add mesh renderer component
            mr.material = material;//set material to avoid evil pinkness of missing texture
           
            mr.transform.localScale = new Vector3(5,5,5);// Fanny: scale up the mesh so it's visible
            mf.mesh.CombineMeshes(data.ToArray());//set mesh to the combination of all of the blocks in the list
            mf.GameObject().layer = 14;//set layer to "ShinyOutline"
            
            //Fanny: Use the Mesh Smoothing Script for adding additional noise to the meshes
            if (meshSmoothing)
            {
                mf.AddComponent<MeshSmoothing>();
                mf.GetComponent<MeshSmoothing>().ApplyPerlin_Hard(mf.gameObject);
            }
            meshes.Add(mf.mesh);//keep track of mesh so we can destroy it when it's no longer needed
            if(collider) g.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;//setting colliders takes more time. disabled for testing.
        }
        #endregion

        isDone = true;
        Debug.Log("Generated Mesh in " + timePassed + "seconds.");
    }

    // Update is called once per frame
    private void Update() {
        
        timePassed += Time.deltaTime;
        
    }

    /// <summary>
    /// Generates numbers of Interpolated Points in segments between given points to "make a curve" 
    /// Used for Racing Mode and Portal Placement
    /// </summary>
    private void GenerateLinearPoints(Vector3 startPoint, Vector3 endPoint, int segments)
    {
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector3 point = Vector3.Lerp(startPoint, endPoint, t);
            interpolatedPoints.Add(point);
        }
    }

    /// <summary>
    /// Removes cubes that are within a specified radius from given points.
    /// Used for Racing Mode and Portal Placement
    /// </summary>
    private void RemoveCubesWithinRadius(List<Vector3> points, float radius)
    {
        // Go through all block data and get the position of it. If the cube is within the radius of the point --> Delete it
        // NOTE: THIS COULD BE OPTIMIZED
        for (int i = blockData.Count - 1; i >= 0; i--)
        {
            Vector3 cubePosition = blockData[i].transform.MultiplyPoint(Vector3.zero); // Get the position of the cube in world space.

            foreach (Vector3 point in points)
            {
                if (Vector3.Distance(cubePosition, point) < radius)
                {
                    Destroy(blockData[i].mesh.GameObject()); // Destroy the cube game object directly.
                    blockData.RemoveAt(i);
                    break; // Remove the cube and move on to the next one.
                }
            }
        }
        Debug.Log("Finished Removing Cubes after " + timePassed + " seconds.");
    }

    /// <summary>
    /// Calculates 3D Perlin noise at the given coordinates, uses Unity's build in Noise.
    /// used in Generation Method
    /// </summary>
    public float Perlin3D(float x, float y, float z)
    {
        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);
        
        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);
        
        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }
    
    /// <summary>
    /// Calculates 3D Perlin noise at the given coordinates.
    /// used in Legacy Marching Cubes
    /// </summary>
    /// <param name="coordinates">3D coordinates.</param>
    public static float Perlin(Vector3 coordinates)
    {
        float ab = Mathf.PerlinNoise(coordinates.x, coordinates.y);
        float bc = Mathf.PerlinNoise(coordinates.y, coordinates.z);
        float ac = Mathf.PerlinNoise(coordinates.x, coordinates.z);
        
        float ba = Mathf.PerlinNoise(coordinates.y, coordinates.x);
        float cb = Mathf.PerlinNoise(coordinates.z, coordinates.y);
        float ca = Mathf.PerlinNoise(coordinates.z, coordinates.x);
        
        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }
}
