/*****************************************************************************
// File Name :         PatrolRoomAttack.cs
// Author :            Tommy Roberts
// Creation Date :     10/2/2024
//
// Brief Description : Controls collision behavior fr attack two
*****************************************************************************/
using System.Collections;
using UnityEngine;

public class PatrolRoomAttack : MonoBehaviour
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
    [Tooltip("Speed at which enemy patrols around room")]
    [SerializeField] private float _patrolSpeed = 2f;
    [Tooltip("Speed at which enemy hunts player when in same room")]
    [SerializeField] private float _seekPlayerSpeed = 5f;
    [Tooltip("Put this set to player transform so enemy will target player when in its room")]
    [SerializeField] private Transform _playerTranform;
    [Tooltip("How long the attack patrols for before despawning and starting another attack")]
    [SerializeField] private float _attackDuration = 15f;
    [Tooltip("Needs to be set to top right corner of room")]
    [SerializeField] private Transform _attackRoomTwoBorderOne;
    [Tooltip("Needs to be set to bottom left corner of room")]
    [SerializeField] private Transform _attackRoomTwoBorderTwo;
    [Tooltip("Set this string to whatever the player attacks tag is")]
    [SerializeField] private string _playerAttackTag = "Player";
    [Tooltip("How long to destory patrol enemy after it collides with player")]
    [SerializeField] private float _destroyAttackDelay = .1f;

    private Transform _targetPoint; // current waypoint target of patrol enemy
    private bool _playerInAttackRange = false;
    private int _currentTargetIndex;
    private GameObject _instantiatedPatrolEnemy; 

    /// <summary>
    /// links attack to manager
    /// </summary>
    private void OnEnable() 
    {
        BossAttacksManager.PatrolRoomAttack += CallAttackTwo;
    }

    /// <summary>
    /// unlinks attack to manager
    /// </summary>
    private void OnDisable()
    {
        BossAttacksManager.PatrolRoomAttack -= CallAttackTwo;
    }

    /// <summary>
    /// when player collides with this attack destory the attack
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.CompareTag(_playerAttackTag))
        {
            Destroy(gameObject, _destroyAttackDelay);
        }
    }

    /// <summary>
    /// checks if player is in room for attack 2
    /// </summary>
    private void CheckPlayerInAttackRoom()
    {
        //check if player is in attack range for patrol enemy
        if(_playerTranform.position.x < _attackRoomTwoBorderOne.position.x && _playerTranform.position.x > _attackRoomTwoBorderTwo.position.x)
        {
            if(_playerTranform.position.z < _attackRoomTwoBorderOne.position.z && _playerTranform.position.z > _attackRoomTwoBorderTwo.position.z)
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
    /// pretty self explanitory
    /// </summary>
    private void CallAttackTwo()
    {
        StartCoroutine(AttackTwo());
    }

    /// <summary>
    /// Enemy patrols a set room when the attack is called. Player needs to shoot enemy to get rid of it.
    /// if player enters room enemy will target player.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackTwo()
    {
        BossAttacksManager.Instance.AttackInProgress = true;
        //choose random start point
        _instantiatedPatrolEnemy = Instantiate(_patrolEnemyPrefab, _roomStartPoint.position, Quaternion.identity);
        ChooseNextRandomPatrolPoint();
        float elapsedTime = 0f;
        while(_instantiatedPatrolEnemy != null && elapsedTime < _attackDuration)
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
            if (Vector3.Distance(_instantiatedPatrolEnemy.transform.position, _targetPoint.position) < 0.1f)
            {
                // Choose the next random target point when the current one is reached
                ChooseNextRandomPatrolPoint();
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //if timer for attack runs out destory attack and start a new one
        if(_instantiatedPatrolEnemy != null)
        {
            Destroy(_instantiatedPatrolEnemy);
            _instantiatedPatrolEnemy = null;
        }

        //if enemy is killed or destroyed then move on to next attack
        BossAttacksManager.Instance.AttackInProgress = false;
    }

    /// <summary>
    /// moves patrol enemy to specified target
    /// </summary>
    private void MoveToTarget()
    {
        // Move the GameObject towards the target point at a constant speed
        _instantiatedPatrolEnemy.transform.position = Vector3.MoveTowards(_instantiatedPatrolEnemy.transform.position, _targetPoint.position, _patrolSpeed * Time.deltaTime);
    }

    /// <summary>
    /// moves patrol enemy to specified target
    /// </summary>
    private void MoveToPlayer()
    {
        // Move the GameObject towards the target point at a constant speed
        _instantiatedPatrolEnemy.transform.position = Vector3.MoveTowards(_instantiatedPatrolEnemy.transform.position, _playerTranform.position, _seekPlayerSpeed * Time.deltaTime);
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
}
