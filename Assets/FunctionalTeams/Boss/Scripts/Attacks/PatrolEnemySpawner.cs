/*****************************************************************************
// File Name :         PatrolEnemySpawner.cs
// Author :            Tommy Roberts
// Contributor :       Andrew Stapay
// Creation Date :     10/2/2024
//
// Brief Description : spawns patrol enemies in set rooms
*****************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// contains functionality for the patrol enemy spawning
/// </summary>
public class PatrolEnemySpawner : MonoBehaviour, ISerializationCallbackReceiver
{
    //attack 2 ref
    [Header("Attack 2")]
    [Space]
    [Tooltip("where the enemy can move to in the room")]
    [SerializeField] private List<RoomWaypointPackage> _roomWaypoints;
    private List<List<Transform>> _roomWaypointsProcessed;
    [Tooltip("where attack two spawns")]
    [SerializeField] private Transform[] _roomStartPoints;
    [Tooltip("prefab of patrol enemy")]
    [SerializeField] private GameObject _patrolEnemyPrefab;
    [Tooltip("Needs to be set to top right corner of room")]
    [SerializeField] private Transform[] _attackRoomBorders1;
    [Tooltip("Needs to be set to bottom left corner of room")]
    [SerializeField] private Transform[] _attackRoomBorders2;
    [Tooltip("tranform to target attached to player")]
    [SerializeField] private Transform _playerTransform;

    private GameObject _instantiatedPatrolEnemy;

    /// <summary>
    /// We want the waypoints for the movement of the enemy to be a 2D array, but Unity doesn't allow
    /// serialization of 2D arrays. To solve this, we'll make a serializable wrapper class to store the info and
    /// translate it into the 2D array
    /// </summary>
    [System.Serializable]
    private struct RoomWaypointPackage
    {
        public int roomNumber;
        public int waypointNumber;
        public Transform waypoint;

        public RoomWaypointPackage(int i1, int i2, Transform t)
        {
            roomNumber = i1;
            waypointNumber = i2;
            waypoint = t;
        }
    }

    /// <summary>
    /// Converts the unserializable 2D Transform array to a serializable 1D RoomWaypointPackage array
    /// ISerializationCallbackReceiver requires this method to be public
    /// </summary>
    public void OnBeforeSerialize()
    {
        _roomWaypoints = new List<RoomWaypointPackage>();

        for (int i = 0; i < _roomWaypointsProcessed.Count; i++)
        {
            for (int j = 0; j < _roomWaypointsProcessed[i].Count; j++)
            {
                _roomWaypoints.Add(new RoomWaypointPackage(i, j, _roomWaypointsProcessed[i][j]));
            }
        }
    }

    /// <summary>
    /// Converts the serializable 1D RoomWaypointPackage array to an unserializable 2D Transform array
    /// ISerializationCallbackReceiver requires this method to be public
    /// </summary>
    public void OnAfterDeserialize()
    {
        _roomWaypointsProcessed = new List<List<Transform>>();
        int i = 0;

        while (i <  _roomWaypoints.Count)
        {
            List<Transform> temp = new List<Transform>();

            int thisRow = _roomWaypoints[i].roomNumber;

            while (i < _roomWaypoints.Count && thisRow == _roomWaypoints[i].roomNumber)
            {
                temp.Add(_roomWaypoints[i].waypoint);
                i++;
            }

            _roomWaypointsProcessed.Add(temp);
        }
    }

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
    /// just for testing
    /// </summary>
    private void Update() 
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnEnemy();
        }
    }

    /// <summary>
    /// spawns enemy that will begin to patrol the room
    /// </summary>
    /// <returns></returns>
    private void SpawnEnemy()
    {
        BossAttacksManager.Instance.AttackInProgress = true;
        //choose random start point
        int randomRoom = Random.Range(0, _roomStartPoints.Length);

        for (int i = 0; i < _roomWaypointsProcessed[randomRoom].Count; i++)
        {
            print(_roomWaypointsProcessed[randomRoom][i]);
        }

        _instantiatedPatrolEnemy = Instantiate(_patrolEnemyPrefab, _roomStartPoints[randomRoom].position, Quaternion.identity);
        var patrolEnemyBehavior = _instantiatedPatrolEnemy.GetComponent<PatrolEnemyBehavior>();
        patrolEnemyBehavior.AttackRoomBorderOne = _attackRoomBorders1[randomRoom];
        patrolEnemyBehavior.AttackRoomBorderTwo = _attackRoomBorders2[randomRoom];
        patrolEnemyBehavior.RoomWaypoints = _roomWaypointsProcessed[randomRoom];
        patrolEnemyBehavior.PlayerTransform = _playerTransform;
    }
}
