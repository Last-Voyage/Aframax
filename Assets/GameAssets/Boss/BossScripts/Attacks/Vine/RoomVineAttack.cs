/*****************************************************************************
// File Name :         FullRoomAttack.cs
// Author :            Tommy Roberts
// Contributor :       Andrew Stapay
//                     Ryan Swanson
// Creation Date :     10/9/2024
//
// Brief Description : controls the full room attack for the boss
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// contains functionality for full room attack
/// </summary>
public class RoomVineAttack : BaseBossAttack
{
    [Tooltip("Link to indicator object that flashes red currently")]
    [SerializeField] private GameObject _bossAttack1Indicator;
    [SerializeField] private Material _lowOpacity;
    [SerializeField] private Material _hitOpacity;
    [Space]

    [Tooltip("Tentacle Prefab goes here")]
    [SerializeField] private GameObject _tentaclePrefab;
    [Tooltip("Set locations for tentacle spawns here. If this list is empty, random locations will be added")]
    [SerializeField] private Transform[] _spawnPoints;
    [Tooltip("The maximum number of tentacles that can spawn in one room")]
    [SerializeField] private int _maxEnemies = 3;
    [Tooltip("The minimum number of tentacles that can spawn in one room")]
    [SerializeField] private int _minEnemies = 1;
    [Space]

    [Tooltip("How fast the indicator blinks when it begins blinking")]
    [SerializeField] private float _startBlinkInterval = 1f;
    [Tooltip("How fast the indicator blinks right before it actually attacks")]
    [SerializeField] private float _endBlinkInterval = .1f;
    [Tooltip("How long the indicator will blink for")]
    [SerializeField] private float _blinkDuration = 3f;
    [Tooltip("How long the actual attack will stay covering the room")]
    [SerializeField] private float _hitBoxAppearDuration = 1f;
    [Tooltip("Amount of time between attacks (in seconds)")]
    [SerializeField] private float _timeBetweenAttacks = 3f;
    [Space]

    private Transform _playerTransform;

    private GameObject[][] _spawnedEnemies = new GameObject[0][];
    private List<Coroutine> _activeCoroutines = new List<Coroutine>();

    private const float ATTACK_DETECTION_RANGE = 8;

    private int _numTentaclesDestroyed;
    private int _numTentaclesSpawned;

    private void Start()
    {
        _isAttackActive = false;
    }

    /// <summary>
    /// links attack to boss attack manager
    /// </summary>
    private void OnEnable() 
    {
        SubscribeToEvents();
    }

    protected override void SubscribeToEvents()
    {
        _onBeginAttack.AddListener(BeginAttack);
    }

    protected override void UnsubscribeToEvents()
    {
        _onBeginAttack.RemoveListener(BeginAttack);
    }

    /// <summary>
    /// unlinks attack script from boss manager
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    private void InitializePlayerTransform()
    {
        _playerTransform = PlayerMovementController.Instance.transform;
    }

    /// <summary>
    /// Begins the attack functionality
    /// </summary>
    protected override void BeginAttack()
    {
        base.BeginAttack();
        _numTentaclesDestroyed = 0;

        InitializePlayerTransform();

        if (_spawnedEnemies.Length == 0)
        {
            // Spawn the tentacle enemies
            SpawnTentacles();
        }

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _activeCoroutines.Add(StartCoroutine(PerformAttack(i)));
        }
    }

    /// <summary>
    /// Chooses a random room inside the boat to attack
    /// Currently unused. It may be used in later iterations if we want random placement
    /// </summary>
    private void AttackRandomRoom()
    {
        int randomRoom = UnityEngine.Random.Range(0, _spawnPoints.Length);

        transform.position = _spawnPoints[randomRoom].position;
    }

    /// <summary>
    /// Spawns a random number of tentacles at each defined spawn point
    /// </summary>
    private void SpawnTentacles()
    {
        _spawnedEnemies = new GameObject[_spawnPoints.Length][];

        for (int i = 0; i < _spawnedEnemies.Length; i++)
        {
            _spawnedEnemies[i] = new GameObject[UnityEngine.Random.Range(_minEnemies, _maxEnemies)];

            for (int j = 0; j < _spawnedEnemies[i].Length; j++)
            {
                float xDiff = (float)(UnityEngine.Random.Range(-100, 100)) / 100f;
                float yDiff = 1.15f;
                float zDiff = (float)(UnityEngine.Random.Range(-100, 100)) / 100f;

                float yRotation = UnityEngine.Random.Range(-180, 180);

                GameObject spawnedTectacle;
                spawnedTectacle = Instantiate(_tentaclePrefab, transform.parent);
                // Need to be childed to move with the boat
                spawnedTectacle.transform.parent = transform;

                _numTentaclesSpawned++;

                _spawnedEnemies[i][j] = spawnedTectacle;
                _spawnedEnemies[i][j].transform.position = _spawnPoints[i].position + new Vector3(xDiff, yDiff, zDiff);
                _spawnedEnemies[i][j].transform.eulerAngles = transform.eulerAngles + new Vector3(0, yRotation, 0);

                AddTentacleDestroyedListener(spawnedTectacle.GetComponentInChildren<WeakPointHandler>());
            }
        }
    }

    /// <summary>
    /// Adds a listener to the tentacle destroyed event
    /// </summary>
    /// <param name="weakPointHandler"></param>
    private void AddTentacleDestroyedListener(WeakPointHandler weakPointHandler)
    {
        weakPointHandler.GetOnAllWeakPointsDestroyedEvent().AddListener(OnTentacleDestroyed);
    }

    /// <summary>
    /// creates an indicator that the room is about to be attacked, and then attacks everything in room
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformAttack(int spawnIndex)
    {
        while (true)
        {
            if (TentaclesLeft(spawnIndex))
            {
                if (Vector3.Distance(_spawnPoints[spawnIndex].position, _playerTransform.position) < ATTACK_DETECTION_RANGE)
                {
                    GameObject newHitbox = Instantiate(_bossAttack1Indicator, _spawnPoints[spawnIndex].position, 
                        Quaternion.identity);

                    // Need to be childed to move with the boat
                    newHitbox.transform.parent = transform;
                    
                    //TODO in VS: Damage and timer should be handled by a new script on the indicator prefab, not here
                    //That link doesn't exist
                    // https://bradleycapstone.atlassian.net/browse/LV-322?atlOrigin=eyJpIjoiZjM1ZGM1MTg5MTA3NDY0ZjlkMmRiYTRhMDViNDYwYjUiLCJwIjoiaiJ9
                    var attackCollider = newHitbox.GetComponent<Collider>();
                    attackCollider.enabled = false;

                    var attackMeshRenderer = newHitbox.GetComponent<MeshRenderer>();
                   
                    attackMeshRenderer.material = _lowOpacity;

                    //start blinking indicating attack will happen soon
                    float elapsedTime = 0f;
                    while (TentaclesLeft(spawnIndex) && elapsedTime < _blinkDuration)
                    {
                        float currentBlinkInterval = Mathf.Lerp(_startBlinkInterval, _endBlinkInterval, 
                            elapsedTime / _blinkDuration);
                        attackMeshRenderer.enabled = true;
                        yield return new WaitForSeconds(currentBlinkInterval);
                        attackMeshRenderer.enabled = false;
                        yield return new WaitForSeconds(currentBlinkInterval);
                        elapsedTime += currentBlinkInterval * 2;
                    }

                    if (TentaclesLeft(spawnIndex))
                    {
                        // after done blinking make the block solid and enable collider to hit player for a second
                        attackMeshRenderer.material = _hitOpacity;

                        // TODO in VS: Damage and timer should be handled by a new script on the indicator prefab, not here
                        // https://bradleycapstone.atlassian.net/browse/LV-322?atlOrigin=eyJpIjoiZjM1ZGM1MTg5MTA3NDY0ZjlkMmRiYTRhMDViNDYwYjUiLCJwIjoiaiJ9
                        attackCollider.enabled = true;

                        attackMeshRenderer.enabled = true;

                        //wait for hit time
                        yield return new WaitForSeconds(_hitBoxAppearDuration);

                        //disable the enabled
                        attackMeshRenderer.enabled = false;
                        attackCollider.enabled = false;

                        // wait before attacking again
                        yield return new WaitForSeconds(_timeBetweenAttacks);
                    }

                    Destroy(newHitbox);
                }
            }

            if (!TentaclesLeft(spawnIndex))
            {
                StopCoroutine(_activeCoroutines[spawnIndex]);
            }

            yield return null;
        }
    }

    /// <summary>
    /// Determines whether or not there are tentacles at a certain spawn point
    /// </summary>
    /// <param name="i"> The index of the spawn point in the _spawnedEnemies array </param>
    /// <returns> true if there is at least one tentacle at the spawn point. false otherwise </returns>
    private bool TentaclesLeft(int i)
    {
        for (int j = 0; j < _spawnedEnemies[i].Length; j++)
        {
            if (_spawnedEnemies[i][j] != null)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Called when a tentacle is destroyed to track this attacks lifetime
    /// </summary>
    /// <param name="weakPointHandler"></param>
    private void OnTentacleDestroyed(WeakPointHandler weakPointHandler)
    {
        _numTentaclesDestroyed++;

        if (_numTentaclesDestroyed == _numTentaclesSpawned)
        {
            EndAttack();
        }
    }
}
