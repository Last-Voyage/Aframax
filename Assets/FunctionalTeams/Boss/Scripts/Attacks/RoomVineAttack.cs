/*****************************************************************************
// File Name :         FullRoomAttack.cs
// Author :            Tommy Roberts
// Contributor :       Andrew Stapay
// Creation Date :     10/9/2024
//
// Brief Description : controls the full room attack for the boss
*****************************************************************************/
using System.Collections;
using UnityEngine;
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
    [SerializeField] private Vector3[] _setSpawnPoints;
    [Tooltip("Set prefab scales for the set tentacle spawns here. " +
        "Index i in this list will correlate to index i in the spawn point list")]
    [SerializeField] private Vector3[] _setAttackScales;
    [Tooltip("The number of rooms where tentacles will spawn")]
    [SerializeField] private int _roomsAttacked = 1;
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

    [Tooltip("Player Transform goes here to detect when to attack")]
    [SerializeField] private Transform _playerTransform;

    private Vector3[] _spawnPoints = new Vector3[0];
    private Vector3[] _attackScales = new Vector3[0];
    private int _randomRoom = 0;
    private GameObject[] _spawnedEnemies;

    private void Start()
    {
        _isAttackActive = false;
        CreateRandomSpawnPoints();
    }

    /// <summary>
    /// links attack to boss attack manager
    /// </summary>
    private void OnEnable() 
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// unlinks attack script from boss manager
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    protected override void SubscribeToEvents()
    {
        BossAttackManager.BeginRoomVineAttack += ActivateThisAttack;
    }

    protected override void UnsubscribeToEvents()
    {
        BossAttackManager.BeginRoomVineAttack -= ActivateThisAttack;
    }

    protected override void BeginAttack()
    {
        base.BeginAttack();
    }

    protected override void EndAttack()
    {
        base.EndAttack();
    }

    private void CreateRandomSpawnPoints()
    {
        if (_setSpawnPoints.Length == 0)
        {
            _spawnPoints = new Vector3[] {new Vector3(11.92f, 8.5f, -9.11f),
                                      new Vector3(14.86f, 8.5f, -2.52f),
                                      new Vector3(14.81f, 8.5f, 4.92f),
                                      new Vector3(10.91f, 8.5f, 17.63f),
                                      new Vector3(10.95f, 8.5f, 11.5f),
                                      new Vector3(8.91f, 8.5f, 0.59f)};

            _attackScales = new Vector3[] {new Vector3(1.25f, 1, 0.7f),
                                       new Vector3(0.5f, 1, 1.5f),
                                       new Vector3(0.5f, 1, 1.2f),
                                       Vector3.one,
                                       new Vector3(1, 1, 1.2f),
                                       new Vector3(0.52f, 1, 2.75f)};
        }
    }

    /// <summary>
    /// pretty self explanatory
    /// </summary>
    private void ActivateThisAttack()
    {
        // tell the attack manager that we are attacking
        _isAttackActive = true;

        // Determine which room to attack
        AttackRandomRoom();

        SpawnTentacles();
        StartCoroutine(PerformAttack());
    }

    /// <summary>
    /// Chooses a random room inside the boat to attack
    /// </summary>
    private void AttackRandomRoom()
    {
        if (_setSpawnPoints.Length > 0)
        {
            _randomRoom = UnityEngine.Random.Range(0, _setSpawnPoints.Length);

            transform.position = _setSpawnPoints[_randomRoom];
            transform.localScale = _setAttackScales[_randomRoom];
        }
        else
        {
            _randomRoom = UnityEngine.Random.Range(0, _spawnPoints.Length);

            transform.position = _spawnPoints[_randomRoom];
            transform.localScale = _attackScales[_randomRoom];
        }
    }

    private void SpawnTentacles()
    {
        _spawnedEnemies = new GameObject[UnityEngine.Random.Range(_minEnemies, _maxEnemies)];

        for (int i = 0; i < _spawnedEnemies.Length; i++)
        {
            float xDiff = (float)(UnityEngine.Random.Range(-25, 25)) / 100f;
            float yDiff = 1.15f;
            float zDiff = (float)(UnityEngine.Random.Range(-25, 25)) / 100f;
            float yRotation = UnityEngine.Random.Range(-180, 180);

            _spawnedEnemies[i] = Instantiate(_tentaclePrefab, transform);
            _spawnedEnemies[i].transform.position = transform.position + new Vector3(xDiff, yDiff, zDiff);
            _spawnedEnemies[i].transform.eulerAngles = transform.eulerAngles + new Vector3(0, yRotation, 0);
        }
    }

    /// <summary>
    /// creates an indicator that the room is about to be attacked, and then attacks everything in room
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformAttack()
    {
        // setting attack indicator
        _bossAttack1Indicator.GetComponent<MeshRenderer>().material = _lowOpacity;
        var attackMeshRenderer = _bossAttack1Indicator.GetComponent<MeshRenderer>();
        var attackCollider = _bossAttack1Indicator.GetComponent<Collider>();

        if (Vector3.Distance(transform.position, _playerTransform.position) < 1)
        {
            //start blinking indicating attack will happen soon
            float elapsedTime = 0f;
            while (elapsedTime < _blinkDuration)
            {
                float currentBlinkInterval = Mathf.Lerp(_startBlinkInterval, _endBlinkInterval, elapsedTime / _blinkDuration);
                attackMeshRenderer.enabled = true;
                yield return new WaitForSeconds(currentBlinkInterval);
                attackMeshRenderer.enabled = false;
                yield return new WaitForSeconds(currentBlinkInterval);
                elapsedTime += currentBlinkInterval * 2;
            }

            // after done blinking make the block solid and enable collider to hit player for a second
            attackMeshRenderer.material = _hitOpacity;
            attackMeshRenderer.enabled = true;
            attackCollider.enabled = true;

            //wait for hit time
            yield return new WaitForSeconds(_hitBoxAppearDuration);

            //disable the enabled
            attackMeshRenderer.enabled = false;
            attackCollider.enabled = false;

            //end attack and cycle to another
            BossAttackManager.Instance.AttackInProgress = false;

            // wait before attacking again
            yield return new WaitForSeconds(_timeBetweenAttacks);
        }
    }
}
