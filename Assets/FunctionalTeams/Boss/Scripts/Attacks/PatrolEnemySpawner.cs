/*****************************************************************************
// File Name :         PatrolEnemySpawner.cs
// Author :            Tommy Roberts
// Creation Date :     10/2/2024
//
// Brief Description : spawns patrol enemies in set rooms
*****************************************************************************/
using System.Collections;
using UnityEngine;

/// <summary>
/// contains functionality for the patrol enemy spawning
/// </summary>
public class PatrolEnemySpawner : MonoBehaviour
{
    //attack 2 ref
    [Header("Attack 2")]
    [Space]
    [Tooltip("where the enemy can move to in the room")]
    [SerializeField] private Transform[] _roomWaypoints;
    [Tooltip("where attack two spawns")]
    [SerializeField] private Transform _roomStartPoint;
    [Tooltip("prefab of patrol enemy")]
    [SerializeField] private GameObject _patrolEnemyPrefab;
    [Tooltip("Needs to be set to top right corner of room")]
    [SerializeField] private Transform _attackRoomBorderOne;
    [Tooltip("Needs to be set to bottom left corner of room")]
    [SerializeField] private Transform _attackRoomBorderTwo;
    [Tooltip("tranform to target attached to player")]
    [SerializeField] private Transform _playerTransform;
    private GameObject _instantiatedPatrolEnemy;

    /// <summary>
    /// links attack to manager
    /// </summary>
    private void OnEnable() 
    {
        BossAttacksManager.PatrolRoomAttack += SpawnEnemy;
    }

    /// <summary>
    /// unlinks attack to manager
    /// </summary>
    private void OnDisable()
    {
        BossAttacksManager.PatrolRoomAttack -= SpawnEnemy;
    }

    /// <summary>
    /// spawns enemy that will begin to patrol the room
    /// </summary>
    /// <returns></returns>
    private void SpawnEnemy()
    {
        BossAttacksManager.Instance.AttackInProgress = true;
        //choose random start point
        _instantiatedPatrolEnemy = Instantiate(_patrolEnemyPrefab, _roomStartPoint.position, Quaternion.identity);
        var patrolEnemyBehavior = _instantiatedPatrolEnemy.GetComponent<PatrolEnemyBehavior>();
        patrolEnemyBehavior.AttackRoomBorderOne = _attackRoomBorderOne;
        patrolEnemyBehavior.AttackRoomBorderTwo = _attackRoomBorderTwo;
        patrolEnemyBehavior.RoomWaypoints = _roomWaypoints;
        patrolEnemyBehavior.PlayerTransform = _playerTransform;

    }
}
