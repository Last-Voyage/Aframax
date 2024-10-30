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
    public Transform RoomBorder1 { get; private set; }

    [Tooltip("Needs to be set to bottom left corner of room")]
    public Transform RoomBorder2 { get; private set; }
    
    public Transform[] WaypointTransforms { get; private set;}

    public PatrolLocation(Transform spawnPoint, Transform roomBorder1, Transform roomBorder2, Transform[] waypoints)
    {
        EnemySpawnPoint = spawnPoint;
        RoomBorder1 = roomBorder1;
        RoomBorder2 = roomBorder2;
        WaypointTransforms = waypoints;
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

    private PatrolLocation[] _patrolLocations;

    #endregion

    private List<PatrolEnemyBehavior> _activePatrolEnemies;

    #region Enable & Action/Event Subscriptions

    private void OnEnable()
    {
        SubscribeToEvents();   
    }

    /// <summary>
    /// unlinks attack to manager
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    protected override void SubscribeToEvents()
    {
        _beginAttack.AddListener(BeginAttack);

        PatrolEnemySpawner.EnemySpawned += PatrolEnemySpawned;

        PatrolEnemyDied.AddListener(PatrolEnemyDespawned);
    }

    protected override void UnsubscribeToEvents()
    {
        _beginAttack.RemoveListener(BeginAttack);

        PatrolEnemySpawner.EnemySpawned -= PatrolEnemySpawned;

        PatrolEnemyDied.RemoveListener(PatrolEnemyDespawned);
    }

    #endregion

    private void Start()
    {
        _isAttackActive = false;
        InitializePatrolLocations();
    }

    /// <summary>
    /// Loops through children and initializes patrol locations
    /// </summary>
    private void InitializePatrolLocations()
    {
        // Parent game object with the childed rooms
        GameObject roomsParent = transform.GetChild(0).gameObject;

        // all parent room objects
        List<Transform> allRooms = new List<Transform>();
        // Loop through roomsParent root children to populate allRooms
        for (int i = 0; i < roomsParent.transform.childCount; i++)
        {
            allRooms.Add(roomsParent.transform.GetChild(i));
        }
        
        // initialize _patrolLocations with the size of all possible attack rooms
        _patrolLocations = new PatrolLocation[allRooms.Count];
        
        // Loop through allRooms to populate _patrolLocations with room data
        for(int i = 0; i < allRooms.Count; i++)
        {
            Transform roomBorder1 = allRooms.ElementAt(i).GetChild(0);
            Transform roomBorder2 = allRooms.ElementAt(i).GetChild(1);
            Transform spawnLocation = allRooms.ElementAt(i).GetChild(2);

            // Parent Waypoint Object
            Transform waypointsParent = allRooms.ElementAt(i).GetChild(3);

            // Remove first element of waypoints (parent obj)
            List<Transform> waypointsList = waypointsParent.GetComponentsInChildren<Transform>().ToList();
            waypointsList.RemoveAt(0);

            // Create array from the waypoints list to pass into the new patrol location
            Transform[] waypoints = waypointsList.ToArray();

            // Create new Patrol Location with the cached data and add it to _patrolLocations
            PatrolLocation patrolLocation = new PatrolLocation(roomBorder1, roomBorder2, spawnLocation, waypoints);
            _patrolLocations[i] = patrolLocation;
        }
    }

    /// <summary>
    /// Destroys all outstanding enemies and calls the endAttack event
    /// </summary>
    protected override void EndAttack()
    {
        foreach(PatrolEnemyBehavior patrolEnemyBehavior in _activePatrolEnemies)
        {
            _activePatrolEnemies.Remove(patrolEnemyBehavior);
            Destroy(patrolEnemyBehavior.gameObject);
        }
        base.EndAttack();
    }

    #region Enemy Spawning

    /// <summary>
    /// Begins the enemy spawning
    /// </summary>
    protected override void BeginAttack()
    {
        base.BeginAttack();
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
}
