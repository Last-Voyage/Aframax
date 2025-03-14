/*****************************************************************************
// File Name :         PatrolEnemySpawner.cs
// Author :            Tommy Roberts
// Contributors:       Andrea Swihart-DeCoster
//                     Ryan Swanson
// Creation Date :     10/2/2024
//
// Brief Description : spawns patrol enemies in set rooms
*****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Defines a patrol location. Contains the spawn point, room border information, and possible enemy waypoints
/// </summary>
[System.Serializable]
public struct PatrolLocation
{
    [Tooltip("Where the patrol enemies spawn")]
    public Transform PatrolSpawnPoint { get; private set; }

    [Tooltip("Where the core of the attack spawns")]
    public Transform CoreSpawnPoint { get; private set; }

    [Tooltip("The associated room of the enemy")]
    public LockdownAttackPatrolRoom EnemyRoom { get; private set; }

    public Transform[] WaypointTransforms { get; private set; }

    public PatrolLocation(Transform patrolSpawnPoint, Transform coreSpawnPoint,
        LockdownAttackPatrolRoom enemyRoom, Transform[] waypoints)
    {
        PatrolSpawnPoint = patrolSpawnPoint;
        CoreSpawnPoint = coreSpawnPoint;
        EnemyRoom = enemyRoom;
        WaypointTransforms = waypoints;
    }
}

/// <summary>
/// contains functionality for the patrol enemy spawning
/// </summary>
public class LockdownAttackEnemyController : MonoBehaviour
{
    private GameObject _patrolEnemyPrefab;
    private GameObject _coreEnemyPrefab;

    private GameObject _instantiatedPatrolEnemy;
    private GameObject _instantiatedCoreEnemy;

    private PatrolLocation _patrolLocation;

    private UnityEvent _onCoreDestroyed = new UnityEvent();

    /// <summary>
    /// Establishes all variables for the enemy controller
    /// </summary>
    /// <param name="patrolEnemy"> The patrol enemy to spawn </param>
    /// <param name="coreEnemy"> The core enemy to spawn </param>
    public void SetUpEnemyController(GameObject patrolEnemy, GameObject coreEnemy)
    {
        _patrolEnemyPrefab = patrolEnemy;
        _coreEnemyPrefab = coreEnemy;

        // Gets the transform of the patrol spawn location
        Transform patrolSpawnLocation = transform.GetChild(0);

        // Gets the transform of the core spawn location
        Transform coreSpawnLocation = transform.GetChild(1);

        // Gets the parent of the waypoints
        Transform waypointsParent = transform.GetChild(2);

        // Remove first element of waypoints (parent obj)
        List<Transform> waypointsList = waypointsParent.GetComponentsInChildren<Transform>().ToList();
        waypointsList.RemoveAt(0);

        // Create array from the waypoints list to pass into the new patrol location
        Transform[] waypoints = waypointsList.ToArray();

        //Sets the patrol location variables
        _patrolLocation = new PatrolLocation
            (patrolSpawnLocation, coreSpawnLocation, GetComponent<LockdownAttackPatrolRoom>(), waypoints);
    }

    /// <summary>
    /// Spawns a patrol enemy and core enemy
    /// </summary>
    public void AttackInRoom()
    {
        SpawnPatrolEnemy();
        SpawnCoreEnemy();
    }

    /// <summary>
    /// Spawns the patrol enemy
    /// </summary>
    private void SpawnPatrolEnemy()
    {
        _instantiatedPatrolEnemy =
            Instantiate(_patrolEnemyPrefab, _patrolLocation.PatrolSpawnPoint.position, 
            Quaternion.identity,transform.parent);

        //Gets the patrol enemy behavior off of the patrol enemy
        if (!_instantiatedPatrolEnemy.TryGetComponent(out LockdownAttackPatrolEnemyBehavior patrolEnemyBehavior))
        {
            return;
        }

        //Initializes the enemies patrol data information
        patrolEnemyBehavior.InitializeAttackInformation(_patrolLocation);
    }

    /// <summary>
    /// Spawns the core enemy
    /// </summary>
    private void SpawnCoreEnemy()
    {
        //Spawns the core
        _instantiatedCoreEnemy = 
            Instantiate(_coreEnemyPrefab, _patrolLocation.CoreSpawnPoint.position, 
            Quaternion.identity,transform.parent);
        PersistentAudioManager.APlayPersistentAudioOnObject?.Invoke(FmodPersistentAudioEvents.Instance.LimbIdle, _instantiatedCoreEnemy);
        RuntimeSfxManager.APlayOneShotSfx?.Invoke(
            FmodSfxEvents.Instance.LimbSpawn, _patrolLocation.CoreSpawnPoint.position);

        //Gets the weak point handler from the core
        if (!_instantiatedCoreEnemy.TryGetComponent(out WeakPointHandler weakPointHandler))
        {
            return;
        }
        //Subscribes core destroyed to the event of all weak points being destroyed
        weakPointHandler.GetOnAllWeakPointsDestroyedEvent().AddListener(CoreDestroyed);
    }

    /// <summary>
    /// Called when all weak points on the core is destroyed
    /// </summary>
    /// <param name="handler"></param>
    private void CoreDestroyed(WeakPointHandler handler)
    {
        //Invokes event for core destruction and removes its listeners
        ForceDestroy();
        OnInvokeCoreDestroyed();
    }

    /// <summary>
    /// Called externally to forcibly destroy the lockdown attack
    /// </summary>
    public void ForceDestroy()
    {
        //Destroys the patrol enemy and core
        _instantiatedPatrolEnemy.GetComponent<LockdownAttackPatrolEnemyBehavior>().DestroyEnemy();
        Destroy(_instantiatedCoreEnemy);

        _onCoreDestroyed.RemoveAllListeners();
    }

    #region Events
    private void OnInvokeCoreDestroyed()
    {
        _onCoreDestroyed?.Invoke();
    }
    #endregion

    #region Getters
    public UnityEvent GetOnCoreDestroyed() => _onCoreDestroyed;
    #endregion
}
