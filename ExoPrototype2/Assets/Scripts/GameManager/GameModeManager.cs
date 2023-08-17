using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

/// <summary>
/// Handles the different Game Mode Instantiations and the Win Conditions
/// </summary>
public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;
    
    // ------- SHOOTER MODE ---------
    [SerializeField] private GameObject playerBullet;
    [SerializeField] private List<GameObject> respawnPoints;
    private bool friendlyFire = false;
    public int points1;
    public int points2;
    //true if win, false if lose
    public bool win;
    
    [Header("InGameUI References")]
    public TextMeshProUGUI points1Text;
    public TextMeshProUGUI points2Text;

    [Header("WorldGen References")]
    [SerializeField] WorldManager world;
    [SerializeField] PerlinNoiseGen noiseGen;
    public bool debug;
    
    // Events
    public delegate void Win();
    public static event Win Player1Win;
    public static event Win Player2Win;

    // ------- RACER MODE ---------
    public int pointsToWin;
    [SerializeField] private Transform goal, start;
    [SerializeField] private List<Transform> checkPoints;
    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject goalParticles;


    /// <summary>
    /// ------- GAME MODES ---------
    ///Player vs AI Shooter
    ///Player can fly through a level and fight enemies like in a bullet hell shooter
    ///
    ///Player vs Player Shooter
    /// Players can fight one another in bullet hell style
    ///
    ///Player vs Player Racer
    /// There is a start and goal and the players have to race one another there, Enemies try to stop them
    /// </summary>
    ///
    private void Awake()
    {
        Instance = this;
        world = WorldManager.Instance;
        //noiseGen = PerlinNoiseGen.Instance;
    }

    private void Start()
    {
        // Get All References when going to new Scene from Main Menu Scene
        /*points1Text = GameObject.Find("Points1").GetComponent<TextMeshProUGUI>();
        points2Text = GameObject.Find("Points2").GetComponent<TextMeshProUGUI>();*/
        
        // Racer Mode References
        start = noiseGen.waypoints[0].transform;
        goal = noiseGen.waypoints[noiseGen.waypoints.Count - 1].transform;
        
        // Shooter Mode Point Handling
        Health.EnemyGotHit += CountPoints;
        Health.PlayerGotHit += CountPoints;
        ReachGoal.ReachedGoal += EndRace;
        
        // Race Mode Point Handling
        PlayerInputManager.instance.onPlayerJoined += PlacePlayer;

        /*points1 = 0;
        points2 = 0;
        points1Text.text = points1.ToString();
        points2Text.text = points2.ToString();*/
     
    }
    
    // Count points when Enemy is eliminated and show in UI
    private void CountPoints(GameObject enemy, GameObject player)
    {
        int points = 0;
        
        if (enemy.gameObject.tag == "Player" || enemy.gameObject.tag == "Player2")
        {
            points = 3;
            // Respawn dead player if he was not killed by an enemy
            if (player.gameObject.tag == "Player" || player.gameObject.tag == "Player2")
            {
                RespawnPlayer(enemy);
            }
            else
            {
                Debug.Log("Player Died by Enemy");
                GameManager.Instance.SetLose();
            }
            
        }
        else if(enemy.name.Equals("BigShip"))
        {
            points = 4;
        }
        else
        {
            points = 2;
        }

        if (player.tag == "Player")
        {
            points1 += points;
        }
        else if (player.tag == "Player2")
        {
            points2 += points;
        }
        else
        {
            Debug.Log("Player Died by Enemy");
            GameManager.Instance.SetLose();
        }
        
        if(points1 >= 10 || points2 >= 10)
        {
            WinCheck();
        }
        
        points1Text.text = points1.ToString();
        points2Text.text = points2.ToString();
    }

    // Check which player has won and send Win Event to change GameState
    private void WinCheck()
    {
        if (points1 > points2)
        {
            win = true;
            Debug.Log("Player 1 Won");
            Player1Win();
        }
        else
        {
            win = true;
            Debug.Log("Player 2 Won");
            Player2Win();
        }
    }

    private void RespawnPlayer(GameObject player)
    {
        int random = Random.Range(0, respawnPoints.Count);
        
        player.transform.position = respawnPoints[random].transform.position;
    }

    // -------- SHOOTER INIT ------------
    
    public void SetFriendlyFire()
    {
        // Toggle Friendly Fire
        friendlyFire = !friendlyFire;
        playerBullet.GetComponent<Bullet>().friendlyFire = friendlyFire;
    }
    
    public void InitShooter()
    {
        // Scene Loaded
        Generate();
    }
    
    // Generate Terrain
    private void Generate()
    {
             for (int i = 0; i < noiseGen.waypoints.Count - 1; i++){
                 //Get Direction to previous point
                Vector3 dir = (noiseGen.waypoints[i].transform.position - noiseGen.waypoints[i+1].transform.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
                
                Instantiate(portal, noiseGen.waypoints[i].transform.position * 5, rotation);
            }
            Instantiate(portal, noiseGen.waypoints[noiseGen.waypoints.Count-1].transform.position * 5, Quaternion.identity);
        
    }
    
    // -------- RACE MODE INIT ------------
    public void InitRace()
    {
        PlayerInputManager.instance.EnableJoining();
        //PlayerInputManager.instance.JoinPlayer();

        for(int i = 1; i < noiseGen.waypoints.Count - 2; i++)
        {
            // Do not add Start and Finish to Checkpoints
            checkPoints.Add(noiseGen.waypoints[i].transform);
            Vector3 dir = (noiseGen.waypoints[i].transform.position - noiseGen.waypoints[i - 1].transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            Instantiate(portal, noiseGen.waypoints[i].transform.position * 5, rotation);
        }
        
        // Get Start and End Point, place a goal/Start there
        start = noiseGen.waypoints[0].transform;
        goal = noiseGen.waypoints[noiseGen.waypoints.Count-1].transform;
        Vector3 dir2 = (noiseGen.waypoints[noiseGen.waypoints.Count-1].transform.position - noiseGen.waypoints[noiseGen.waypoints.Count - 2].transform.position).normalized;
        Quaternion rotation2 = Quaternion.LookRotation(dir2, Vector3.up);
        Instantiate(goalParticles, goal.position, rotation2);

        // Spawn players at Start of Race
        GameManager.Instance.player1.transform.position = start.position;
    }
    
    // Generate Terrain
    private void GenerateRace()
    {
        noiseGen.raceMode = true;
        noiseGen.withCurve = true;
       // noiseGen.Generate();
        //if(noiseGen.isDone)world.InitializeGrid();
    }

    private void PlacePlayer(PlayerInput player)
    {
        player.gameObject.transform.position = new Vector3(start.position.x + 10, start.position.y + 10, start.position.z + 10);
    }

    private void StartRace()
    {
       
    }

    // Check which player has won and send Win Event to change GameState
    // Connected to the ReachGoal Event on the Goal Object
    private void EndRace(GameObject player)
    {
        if(player.tag == "Player")
        {
            win = true;
            Debug.Log("Player 1 Won");
            Player1Win();
        }
        else
        {
            win = true;
            Debug.Log("Player 2 Won");
            Player2Win();
        }
    }

}
