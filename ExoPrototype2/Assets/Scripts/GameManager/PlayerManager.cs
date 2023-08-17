using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerManager handles the Multiplayer and spawns the players at the correct spawnpoints.
/// Works with the PlayerInputManager and GameStateManager
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [Header("Player Manager Settings")] [SerializeField]
    public GameObject playerPrefab1;
    public GameObject playerPrefab2;
    [SerializeField] private List<Transform> startingPoints;
    [SerializeField] private List<LayerMask> playerLayers;

    private PlayerInputManager playerInputManager;
    private List<PlayerInput> players = new List<PlayerInput>();
    
    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    // Add Player to correct position
    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);
        Transform playerParent = player.transform;
        playerParent.position = startingPoints[players.Count - 1].position;
    }
}