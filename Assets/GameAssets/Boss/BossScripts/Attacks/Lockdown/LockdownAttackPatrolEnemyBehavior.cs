/*****************************************************************************
// File Name :         PatrolEnemyBehavior.cs
// Author :            Tommy Roberts
// Contributor:        Andrea Swihart-DeCoster
//                     Ryan Swanson
// Creation Date :     10/10/2024
//
// Brief Description : Controls functionality for the spawned patrol enemy
*****************************************************************************/
using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// contains behavior for the spawned patrol enemy
/// </summary>
public class LockdownAttackPatrolEnemyBehavior : MonoBehaviour
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
    
    private bool _isPlayerInAttackRange = false;
    private int _currentTargetIndex;

    private PatrolLocation _patrolLocationData;

    private Transform _playerTransform;
    private Coroutine _patrolCoroutine;

    /// <summary>
    /// Starts the patrol
    /// </summary>
    private void Start() 
    {
        SubscribeToEvents();
        CheckPlayerAlreadyInRoom();
        BeginPatrolling();
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
    /// Caches the playerTransform
    /// </summary>
    private void BeginPatrolling()
    {
        _patrolCoroutine = StartCoroutine(PatrolRoom());
    }

    private void OnDestroy()
    {
        StopCoroutine(_patrolCoroutine);
        UnsubscribeToEvents();
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
    /// Listens for room event of player entering room
    /// </summary>
    private void PlayerEnteredRoom()
    {
        _isPlayerInAttackRange = true;
    }

    /// <summary>
    /// Listens for room event of player leaving room
    /// </summary>
    private void PlayerExitedRoom()
    {
        _isPlayerInAttackRange = false;
    }

    /// <summary>
    /// Checks if the player entered the room before the enemy spawned
    /// </summary>
    private void CheckPlayerAlreadyInRoom()
    {
        if (_patrolLocationData.EnemyRoom.IsPlayerInRoom())
        {
            PlayerEnteredRoom();
        }
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
        while(gameObject != null)
        {
            // Only attack if player is within range
            if(!_isPlayerInAttackRange)
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

            yield return null;
        }
    }

    /// <summary>
    /// Notifies that patrol enemy has died and destroys self
    /// </summary>
    private void DestroyEnemy()
    {
        
        // Destroys enemy
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

    /// <summary>
    /// Subscribes to the room event
    /// </summary>
    private void SubscribeToEvents()
    {
        _patrolLocationData.EnemyRoom.GetOnPlayerRoomEnterEvent().AddListener(PlayerEnteredRoom);
        _patrolLocationData.EnemyRoom.GetOnPlayerRoomExitEvent().AddListener(PlayerExitedRoom);
        LockdownAttackController.DestroyAllEnemies.AddListener(DestroyEnemy);
    }

    /// <summary>
    /// Unsubscribes to the room event
    /// </summary>
    private void UnsubscribeToEvents()
    {
        _patrolLocationData.EnemyRoom.GetOnPlayerRoomEnterEvent().RemoveListener(PlayerEnteredRoom);
        _patrolLocationData.EnemyRoom.GetOnPlayerRoomExitEvent().RemoveListener(PlayerExitedRoom);
        LockdownAttackController.DestroyAllEnemies.RemoveListener(DestroyEnemy);
    }
}