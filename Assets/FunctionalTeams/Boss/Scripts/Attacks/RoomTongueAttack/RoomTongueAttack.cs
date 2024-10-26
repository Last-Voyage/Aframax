/*****************************************************************************
// File Name :         RoomTongueAttack.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     10/26/24
//
// Brief Description : Controls the Aggressive Room 1 Attack Behavior
*****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[Tooltip("Defines a patrol location. Contains the spawn point, room border information, and possible enemy waypoints")]
public struct PatrolLocation
{
    [Tooltip("Where attack two spawns")]
    public Transform EnemySpawnPoint { get; private set; }

    [Tooltip("Needs to be set to top right corner of room")]
    public Transform AttackBorder1 { get; private set; }

    [Tooltip("Needs to be set to bottom left corner of room")]
    public Transform AttackBorder2 { get; private set; }

    [Tooltip("Game Object with children defining where the enemy can move to in the room")]
    private GameObject _waypoints;
    
    public List<Transform> WaypointTransforms { get; private set;}

    /// <summary>
    /// Collects all transforms from Waypoints and stores it in a list
    /// </summary>
    public void InitializeWaypointTransforms()
    {
        WaypointTransforms = new List<Transform>();
        WaypointTransforms = _waypoints.GetComponentsInChildren<Transform>().ToList();
        WaypointTransforms.RemoveAt(0); // Remove parent transform
    }
}

/// <summary>
/// Manages the Aggressive Room Tongue Attack.
/// </summary>
public class RoomTongueAttack : BaseBossAttack
{
    public static Action<PatrolLocation> SpawnPatrolEnemies;

    public static UnityEvent<PatrolEnemyBehavior> PatrolEnemyDied = new UnityEvent<PatrolEnemyBehavior>();

    #region Attack Settings

    [Header("Attack Settings")]

    [Tooltip("Number of enemies spawned in an instance")]
    [SerializeField] int _numEnemiesPerSpawnSet;

    [Tooltip("Time between Num Enemies Per Spawn in seconds.")]
    [SerializeField] float _timeBetweenIndividualSpawns;

    [Space]
    [Tooltip("Time between each set of enemies spawned")]
    [SerializeField] float _timeBetweenSpawnSets;

    [Space]
    [Tooltip("Maximum numbers of enemies that can be alive at a time")]
    [SerializeField] int _maxEnemiesSpawned;

    [Header("Attack Locations")]
    [SerializeField] PatrolLocation[] _patrolLocations;

    #endregion

    private bool _isAttackActive;

    private List<PatrolEnemyBehavior> _activePatrolEnemies;

    #region Enable & Action/Event Subscriptions

    /// <summary>
    /// links attack to manager
    /// </summary>
    private void OnEnable()
    {
        BossAttackManager.BeginInteriorTongueAttack += StartAttack;
        PatrolEnemySpawner.EnemySpawned += PatrolEnemySpawned;

        PatrolEnemyDied.AddListener(PatrolEnemyDespawned);
    }

    /// <summary>
    /// unlinks attack to manager
    /// </summary>
    private void OnDisable()
    {
        BossAttackManager.BeginInteriorTongueAttack -= StartAttack;
        PatrolEnemySpawner.EnemySpawned -= PatrolEnemySpawned;

        PatrolEnemyDied.RemoveListener(PatrolEnemyDespawned);
    }

    #endregion

    private void Start()
    {
        _isAttackActive = false;
    }

    #region Enemy Spawning

    /// <summary>
    /// Begins the enemy spawning
    /// </summary>
    private void StartAttack()
    {
        _isAttackActive = true;
        StartCoroutine(EnemySpawning());
    }

    /// <summary>
    /// Spawns enemies until the max amount have been spawned.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemySpawning()
    {
        _activePatrolEnemies = new List<PatrolEnemyBehavior>();

        while(_isAttackActive)
        {
            // Only spawn an enemy if the max amount has not been reached
            if(_activePatrolEnemies.Count < _maxEnemiesSpawned)
            {
                // Get a random patrol room
                int patrolRoomIndex = UnityEngine.Random.Range(0, _patrolLocations.Length);

                for(int i = 0; i < _numEnemiesPerSpawnSet; i++)
                {
                    SpawnPatrolEnemies?.Invoke(_patrolLocations[patrolRoomIndex]);
                    yield return new WaitForSeconds(_timeBetweenIndividualSpawns);
                }
            } 
            yield return new WaitForSeconds(_timeBetweenSpawnSets);
        }
    }

    /// <summary>
    /// Adds the new spawned patrol enemies to the list of active patrol enemies
    /// </summary>
    /// <param name="patrolEnemyBehavior"> enemy that just spawned </param>
    private void PatrolEnemySpawned(PatrolEnemyBehavior patrolEnemyBehavior)
    {
        _activePatrolEnemies.Add(patrolEnemyBehavior);
    }

    /// <summary>
    /// Adds the new spawned patrol enemies to the list of active patrol enemies
    /// </summary>
    /// <param name="patrolEnemyBehavior"> enemy that just spawned </param>
    private void PatrolEnemyDespawned(PatrolEnemyBehavior patrolEnemyBehavior)
    {
        _activePatrolEnemies.Remove(patrolEnemyBehavior);
    }

    #endregion Enemy Spawning

    /// <summary>
    /// Stops the attack from playing
    /// </summary>
    private void EndAttack()
    {
        _isAttackActive = false;
    }
}
