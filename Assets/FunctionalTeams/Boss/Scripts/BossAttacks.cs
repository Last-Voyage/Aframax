/*****************************************************************************
// File Name :         BossAttacks.cs
// Author :            Tommy Roberts
// Creation Date :     10/2/2024
//
// Brief Description : Controls first two boss attacks for our first playable build
*****************************************************************************/
using System.Collections;
using UnityEngine;

public class BossAttacks : MonoBehaviour
{
    //general
    private bool _attackInProgress = false;
    [SerializeField] private float _timeBetweenAttacks = 5f;
    [Tooltip("this is how long before boss attacks actually begin if you want start delay")]
    [SerializeField] private float _timeAfterStartToStartAttacks = 10f;
    //attack 1 ref
    [Header("Attack 1")]
    [Space]
    [SerializeField] private GameObject _bossAttack1Indicator;
    [SerializeField] private Material _lowOpacity;
    [SerializeField] private Material _hitOpacity;
    [SerializeField] private float _startBlinkInterval = 1f;
    [SerializeField] private float _endBlinkInterval = .1f;
    [SerializeField] private float _blinkDuration = 5f;
    [SerializeField] private float _hitBoxAppearDuration = 1f;
    //attack 2 ref
    [Header("Attack 2")]
    [Space]
    [SerializeField] private Transform[] _roomWaypoints;
    [Tooltip("where attack two spawns")]
    [SerializeField] private Transform _roomStartPoint;
    [SerializeField] private GameObject _patrolEnemyPrefab;
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _seekPlayerSpeed = 5f;
    [Tooltip("Put this set to player transform so enemy will target player when in its room")]
    [SerializeField] private Transform _playerTranform;
    [Tooltip("Attack two will despawn and a new attack will be chosen if nothing happens in this amount of time")]
    [SerializeField] private float _timeAttackTwoLasts = 15f;
    [SerializeField] private Transform _attackRoomTwoBorderOne;
    [SerializeField] private Transform _attackRoomTwoBorderTwo;
    private Transform _targetPoint;
    private bool _playerInAttackRange = false;
    private int _currentTargetIndex;
    private GameObject _instantiatedPrefab;
    
    /// <summary>
    /// Starts random attacks at the set interval
    /// </summary>
    void Start()
    {
        StartCoroutine(ChooseAttacksRepeatedly());
    }

    /// <summary>
    /// checks for player position and used for testing
    /// </summary>
    void Update()
    {
        CheckPlayerInAttackRoom();
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
    /// creates an indicator that the room is about to be attacked, and then attacks everything in room
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackOne()
    {
        _attackInProgress = true;
        _bossAttack1Indicator.GetComponent<MeshRenderer>().material = _lowOpacity;
        //start blinking indicating attack will happen soon
        float elapsedTime = 0f;
        while(elapsedTime < _blinkDuration)
        {
            float currentBlinkInterval = Mathf.Lerp(_startBlinkInterval, _endBlinkInterval, elapsedTime / _blinkDuration);
            _bossAttack1Indicator.GetComponent<MeshRenderer>().enabled = true;
            yield return new WaitForSeconds(currentBlinkInterval);
            _bossAttack1Indicator.GetComponent<MeshRenderer>().enabled = false;
            yield return new WaitForSeconds(currentBlinkInterval);
            elapsedTime+= currentBlinkInterval * 2;
        }

        // after done blinking make the block solid and enable collider to hit player for a second
        _bossAttack1Indicator.GetComponent<MeshRenderer>().material = _hitOpacity;
        _bossAttack1Indicator.GetComponent<MeshRenderer>().enabled = true;
        _bossAttack1Indicator.GetComponent<BoxCollider>().enabled = true;

        //wait for hit time
        yield return new WaitForSeconds(_hitBoxAppearDuration);

        //disable the enabled
        _bossAttack1Indicator.GetComponent<MeshRenderer>().enabled = false;
        _bossAttack1Indicator.GetComponent<BoxCollider>().enabled = false;
        
        //end attack and cycle to another
        _attackInProgress = false;
    }

    /// <summary>
    /// Enemy patrols a set room when the attack is called. Player needs to shoot enemy to get rid of it.
    /// if player enters room enemy will target player.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackTwo()
    {
        _attackInProgress = true;
        //choose random start point
        _instantiatedPrefab = Instantiate(_patrolEnemyPrefab, _roomStartPoint.position, Quaternion.identity);
        ChooseNextRandomPoint();
        float elapsedTime = 0f;
        while(_instantiatedPrefab != null && elapsedTime < _timeAttackTwoLasts)
        {
            if(!_playerInAttackRange)
            {
                MoveToTarget();
            }
            else
            {
                MoveToPlayer();
            }
            
            // Check if the GameObject has reached the target point
            if (Vector3.Distance(_instantiatedPrefab.transform.position, _targetPoint.position) < 0.1f)
            {
                // Choose the next random target point when the current one is reached
                ChooseNextRandomPoint();
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //if timer for attack runs out destory attack and start a new one
        if(_instantiatedPrefab != null)
        {
            Destroy(_instantiatedPrefab);
            _instantiatedPrefab = null;
        }

        //if enemy is killed or destroyed then move on to next attack
        _attackInProgress = false;
    }

    /// <summary>
    /// moves patrol enemy to specified target
    /// </summary>
    private void MoveToTarget()
    {
        // Move the GameObject towards the target point at a constant speed
        _instantiatedPrefab.transform.position = Vector3.MoveTowards(_instantiatedPrefab.transform.position, _targetPoint.position, _patrolSpeed * Time.deltaTime);
    }

    /// <summary>
    /// moves patrol enemy to specified target
    /// </summary>
    private void MoveToPlayer()
    {
        // Move the GameObject towards the target point at a constant speed
        _instantiatedPrefab.transform.position = Vector3.MoveTowards(_instantiatedPrefab.transform.position, _playerTranform.position, _seekPlayerSpeed * Time.deltaTime);
    }

    /// <summary>
    /// choosees next random point for patroling enemy
    /// </summary>
    private void ChooseNextRandomPoint()
    {
        // Pick a random point from the array
        _currentTargetIndex = Random.Range(0, _roomWaypoints.Length);
        _targetPoint = _roomWaypoints[_currentTargetIndex];
    }

    /// <summary>
    /// Chooses attakcs randomly when there is not a current attack in progress
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChooseAttacksRepeatedly()
    {
        yield return new WaitForSeconds(_timeAfterStartToStartAttacks);
        while(true)
        {
            if(!_attackInProgress)
            {
                _attackInProgress = true;
                yield return new WaitForSeconds(_timeBetweenAttacks);
                if(Random.Range(0, 1f) > .5f)
                {
                    StartCoroutine(AttackOne());
                }
                else
                {
                    StartCoroutine(AttackTwo());
                }
            }
            yield return null;
        }
    }
}
