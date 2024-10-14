/*****************************************************************************
// File Name :         PatrolEnemyBehavior.cs
// Author :            Tommy Roberts
// Creation Date :     10/10/2024
//
// Brief Description : controls functionality for the spawned patrol enemy
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// contains behavior for the spawned patrol enemy
/// </summary>
public class PatrolEnemyBehavior : MonoBehaviour
{
    [Tooltip("Speed at which enemy patrols around room")]
    [SerializeField] private float _patrolSpeed = 2f;
    [Tooltip("Speed at which enemy hunts player when in same room")]
    [SerializeField] private float _seekPlayerSpeed = 5f;
    [Tooltip("How long the attack patrols for before despawning and starting another attack")]
    [SerializeField] private float _attackDuration = 15f;
    [Tooltip("Set this string to whatever the player attacks tag is")]
    [SerializeField] private string _playerAttackTag = "Player";
    [Tooltip("How long to destory patrol enemy after it collides with player")]
    [SerializeField] private float _destroyAttackDelay = .1f;
    [Tooltip("Adds a delay before starting to patrol the room")]
    [SerializeField] private float _timeToWaitBeforePatroling = .5f;

    private Transform _targetPoint; // current waypoint target of patrol enemy
    private bool _playerInAttackRange = false;
    private int _currentTargetIndex;
    private Transform _attackRoomBorderOne;
    private Transform _attackRoomBorderTwo;
    private Transform[] _roomWaypoints;
    private Transform _playerTransform;
    private Coroutine _patrolCoroutine;

    /// <summary>
    /// starts the patrol
    /// </summary>
    private void Start() 
    {
        _patrolCoroutine = StartCoroutine(PatrolRoom());
    }

    /// <summary>
    /// stops the errors from happening
    /// </summary>
    private void OnDestroy()
    {
        StopCoroutine(_patrolCoroutine);
    }

    /// <summary>
    /// when player collides with this attack destory the attack
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.CompareTag(_playerAttackTag))
        {
            if(gameObject != null)
            {
                //add destory attack delay here in future if needed but for now it could cause bugs
                Destroy(gameObject);
            }  
        }
    }

    /// <summary>
    /// checks if player is in room for attack 2
    /// </summary>
    private void CheckPlayerInAttackRoom()
    {
        //check if gameobject is null
        if(gameObject == null) return;
        //check if player is in attack range for patrol enemy
        if(_playerTransform.position.x < _attackRoomBorderOne.position.x && _playerTransform.position.x > _attackRoomBorderTwo.position.x)
        {
            if(_playerTransform.position.z < _attackRoomBorderOne.position.z && _playerTransform.position.z > _attackRoomBorderTwo.position.z)
            {
                _playerInAttackRange = true;
            }
            else
            {
                _playerInAttackRange = false;
            }
            
        }
        else
        {
            _playerInAttackRange = false;
        }
    }

    /// <summary>
    /// starts patroling room going to set waypoints until detecting a player or time runs out
    /// </summary>
    /// <returns></returns>
    private IEnumerator PatrolRoom()
    {
        yield return new WaitForSeconds(_timeToWaitBeforePatroling);
        ChooseNextRandomPatrolPoint();
        float elapsedTime = 0f;
        while(gameObject != null && elapsedTime < _attackDuration)
        {
            CheckPlayerInAttackRoom();
            if(!_playerInAttackRange)
            {
                MoveToTarget();
            }
            else
            {
                MoveToPlayer();
            }
            
            // Check if the GameObject has reached the target point
            if (Vector3.Distance(gameObject.transform.position, _targetPoint.position) < 0.1f)
            {
                // Choose the next random target point when the current one is reached
                ChooseNextRandomPatrolPoint();
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //if enemy is killed or destroyed then move on to next attack
        BossAttacksManager.Instance.AttackInProgress = false;

        //if timer for attack runs out destory attack and start a new one
        if(gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// moves patrol enemy to specified target
    /// </summary>
    private void MoveToTarget()
    {
        // Move the GameObject towards the target point at a constant speed
        transform.position = Vector3.MoveTowards(transform.position, _targetPoint.position, _patrolSpeed * Time.deltaTime);
    }

    /// <summary>
    /// moves patrol enemy to specified target
    /// </summary>
    private void MoveToPlayer()
    {
        // Move the GameObject towards the target point at a constant speed
        transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, _seekPlayerSpeed * Time.deltaTime);
    }

    /// <summary>
    /// choosees next random point for patroling enemy
    /// </summary>
    private void ChooseNextRandomPatrolPoint()
    {
        // Pick a random point from the array
        _currentTargetIndex = Random.Range(0, _roomWaypoints.Length);
        _targetPoint = _roomWaypoints[_currentTargetIndex];
    }

    public Transform AttackRoomBorderOne
    {
        get { return _attackRoomBorderOne; }
        set { _attackRoomBorderOne = value;}
    }

    public Transform AttackRoomBorderTwo
    {
        get { return _attackRoomBorderTwo;}
        set { _attackRoomBorderTwo = value;}
    }

    public Transform[] RoomWaypoints
    {
        get { return _roomWaypoints;}
        set { _roomWaypoints = value;}
    }

    public Transform PlayerTransform
    {
        get { return _playerTransform; }
        set { _playerTransform = value; }
    }
}
