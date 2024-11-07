/*****************************************************************************
// File Name :         PatrolEnemySpawner.cs
// Author :            Tommy Roberts
// Contributors:       Andrea Swihart-DeCoster
// Creation Date :     10/2/2024
//
// Brief Description : spawns patrol enemies in set rooms
*****************************************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// contains functionality for the patrol enemy spawning
/// </summary>
public class LockdownAttackEnemySpawner : MonoBehaviour
{
    /// <summary>
    /// Invoked when an enemy is spawned
    /// </summary>
    public static Action<LockdownAttackEnemyBehavior> OnEnemySpawned;
   
    /// <summary>
    /// Initializes the enemy patrol data when an enemy is spawned
    /// </summary>
    public static UnityEvent<PatrolLocation> InitializeEnemyData = new();

    [Tooltip("Prefab of patrol enemy")]
    [SerializeField] private GameObject _patrolEnemyPrefab;

    private GameObject _instantiatedPatrolEnemy;

    private void OnEnable()
    {
        LockdownAttack.SpawnPatrolEnemies += SpawnEnemy;
    }

    private void OnDisable()
    {
        LockdownAttack.SpawnPatrolEnemies -= SpawnEnemy;
    }

    /// <summary>
    /// Spawns a patrol enemy.
    /// </summary>
    /// <param name="patrolLocation"> patrolLocation data </param>
    private void SpawnEnemy(PatrolLocation patrolLocation)
    {
        _instantiatedPatrolEnemy = 
            Instantiate(_patrolEnemyPrefab, patrolLocation.EnemySpawnPoint.position, Quaternion.identity);

        LockdownAttackEnemyBehavior patrolEnemyBehavior = _instantiatedPatrolEnemy.GetComponent<LockdownAttackEnemyBehavior>();

        //Initializes the enemies patrol data information
        InitializeEnemyData.AddListener(patrolEnemyBehavior.InitializeAttackInformation);
        InitializeEnemyData?.Invoke(patrolLocation);
        InitializeEnemyData.RemoveListener(patrolEnemyBehavior.InitializeAttackInformation);

        OnEnemySpawned?.Invoke(patrolEnemyBehavior);
    }
}
