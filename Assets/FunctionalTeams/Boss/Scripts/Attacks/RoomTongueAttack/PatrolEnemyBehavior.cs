/*****************************************************************************
// File Name :         PatrolEnemyBehavior.cs
// Author :            Tommy Roberts
// Contributor:        Andrea Swihart-DeCoster
// Creation Date :     10/10/2024
//
// Brief Description : Controls functionality for the spawned patrol enemy
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// contains behavior for the spawned patrol enemy
/// </summary>
public class PatrolEnemyBehavior : MonoBehaviour
{
    #region Patrol Settings

    [Header("Patrol Settings")]
    [Tooltip("Speed at which enemy patrols around room")]
    [SerializeField] private float _patrolSpeed = 2f;

    [Tooltip("Speed at which enemy chases the player")]
    [SerializeField] private float _seekPlayerSpeed = 5f;

    [Tooltip("How long the enemy patrols before despawning")]
    [SerializeField] private float _attackDuration = 15f;

    [Tooltip("Adds a delay before starting to patrol the room")]
    [SerializeField] private float _timeToWaitBeforePatroling = .5f;

    #endregion Patrol Settings

    /// <summary>
    /// Current waypoint target of patrol enemy
    /// </summary>
    private Transform _targetPoint;
    
    private bool _playerInAttackRange = false;
    private int _currentTargetIndex;

    private PatrolLocation _patrolLocationData;

    private Transform _playerTransform;
    private Coroutine _patrolCoroutine;

    /// <summary>
    /// Attack will despawn when lifetime >= attackDurations
    /// </summary>
    private float _lifetime;

    /// <summary>
    /// Starts the patrol
    /// </summary>
    private void Start() 
    {
        BeginPatrolling();
        InitializeLifetime();
        InitializePlayerTransform();
    }

    /// <summary>
    /// Initializes _playerTransform
    /// </summary>
    private void InitializePlayerTransform()
    {
        _playerTransform = PlayerMovementController.Instance.transform;
    }

    /// <summary>
    /// Initializes _lifetime
    /// </summary>
    private void InitializeLifetime()
    {
        _lifetime = 0.0f;
    }

    /// <summary>
    /// Caches the playerTransform
    /// </summary>
    private void BeginPatrolling()
    {
        _patrolCoroutine = StartCoroutine(PatrolRoom());
    }

    private void OnDestroy()
    {
        StopCoroutine(_patrolCoroutine);
    }

    /// <summary>
    /// Initializes Attack Information
    /// </summary>
    /// <param name="patrolLocation"></param>
    public void InitializeAttackInformation(PatrolLocation patrolLocation)
    {
        _patrolLocationData = patrolLocation;
    }

    /// <summary>
    /// Checks if player is within the range to be attacked
    /// </summary>
    private void CheckPlayerInAttackRoom()
    {
        // Attack is invalid if there is not proper room data
        if(_patrolLocationData.RoomBorder1 == null || _patrolLocationData.RoomBorder2 == null)
        {
            Destroy(gameObject);
            return;
        }

        _playerInAttackRange = IsPlayerInAttackRange();
    }

    /// <summary>
    /// Checks if player is within bounds of the room to be attacked
    /// </summary>
    /// <returns> T if player is within attack range </returns>
    private bool IsPlayerInAttackRange()
    {
        return (_playerTransform.position.x > _patrolLocationData.RoomBorder1.position.x)
            && (_playerTransform.position.x < _patrolLocationData.RoomBorder2.position.x)
            && (_playerTransform.position.z > _patrolLocationData.RoomBorder1.position.z)
            && (_playerTransform.position.z < _patrolLocationData.RoomBorder2.position.z);
    }

    /// <summary>
    /// Starts patroling room going to set waypoints until detecting a player or time runs out
    /// </summary>
    /// <returns></returns>
    private IEnumerator PatrolRoom()
    {
        yield return new WaitForSeconds(_timeToWaitBeforePatroling);

        // Choose initial patrol point
        ChooseNextRandomPatrolPoint();

        // Controls enemy movement as long as it's alive
        while(gameObject != null && _lifetime < _attackDuration)
        {
            CheckPlayerInAttackRoom();

            // Only attack if player is within range
            if(!_playerInAttackRange)
            {
                MoveToTarget();
            }
            else
            {
                MoveToPlayer();
            }
            
            // Check if the GameObject has reached the target point
            if (_targetPoint != null && Vector3.Distance(gameObject.transform.position, _targetPoint.position) < 0.1f)
            {
                // Choose the next random target point when the current one is reached
                ChooseNextRandomPatrolPoint();
            }

            _lifetime += Time.deltaTime;
            yield return null;
        }

        EndLifetime();
        
    }

    /// <summary>
    /// Notifies that patrol enemy has died and destroys self
    /// </summary>
    private void EndLifetime()
    {
        RoomTongueAttack.PatrolEnemyDied?.Invoke(this);

        // Enemy dies once lifetime has expired
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Moves patrol enemy to specified target
    /// </summary>
    private void MoveToTarget()
    {
        // Move the GameObject towards the target point at a constant speed
        if(_targetPoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPoint.position, _patrolSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Moves patrol enemy to specified target
    /// </summary>
    private void MoveToPlayer()
    {
        // Move the GameObject towards the target point at a constant speed
        transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, _seekPlayerSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Chooses next random point for patroling enemy
    /// </summary>
    private void ChooseNextRandomPatrolPoint()
    {
        // Pick a random point from the list of possible transforms
        _currentTargetIndex = Random.Range(0, _patrolLocationData.WaypointTransforms.Length);
        _targetPoint = _patrolLocationData.WaypointTransforms.ElementAt(_currentTargetIndex);
    }

    /// <returns> This enemies patrol data </returns>
    public PatrolLocation GetPatrolLocationData()
    {
        return _patrolLocationData;
    }
}
