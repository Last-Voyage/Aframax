/*****************************************************************************
// File Name :         WhackAMole.cs
// Author :            Tommy Roberts
// Creation Date :     4/3/2025
//
// Brief Description : This script controls the functionality for the whack a mole
                        vine stuff
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all the Whack a Mole functions
/// </summary>
public class WhackAMole : MonoBehaviour
{
    [SerializeField] private Transform[] _whackAMolePoints;
    private bool _canAttack = true;
    private bool _attackTriggered = false;
    [SerializeField] private float _attackInterval = 6f;
    private float _attackTimer = 0;
    [SerializeField] private GameObject _whackAMoleVine;
    private GameObject _currentActiveVine;
    private ProceduralVine _currentActiveVineScript;
    private Transform _playerTransform;

    /// <summary>
    /// spawns a new attack if conditions are right and reduces attack cd.
    /// </summary>
    private void Update()
    {
        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }

        if (_canAttack && _attackTimer <= 0)
        {
            SpawnVineAtRandomHole(_playerTransform);
        }
    }

    /// <summary>
    /// Calls the attack when player walks into trigger (PlayerCollision.cs)
    /// </summary>
    /// <param name="player"></param>
    public void CallAttack(Transform player)
    {
        //if attack in progress or on cd don't call
        if (_attackTimer > 0 || _attackTriggered)
            return;

        _attackTriggered = true;
        
        SpawnVineAtRandomHole(player);
    }

    /// <summary>
    /// Spawns a vine at a random hole in wall
    /// </summary>
    /// <param name="player"></param>
    private void SpawnVineAtRandomHole(Transform player)
    {
        if (!_canAttack || !_attackTriggered) return;

        _attackTimer = _attackInterval;
        _playerTransform = player;

        //spawn in vine and set variables
        var randTransform = _whackAMolePoints[Random.Range(0, _whackAMolePoints.Length)];
        _currentActiveVine = Instantiate(_whackAMoleVine, transform);
        _currentActiveVineScript = _currentActiveVine.transform.GetChild(2).GetComponent<ProceduralVine>();

        //set up vine and start the appearance
        Vector3 vinePos = new Vector3(randTransform.position.x, randTransform.position.y - 5.5f, randTransform.position.z);
        _currentActiveVine.transform.position = vinePos;
        _currentActiveVine.transform.forward = randTransform.up;

        CreateSpawnVfx(randTransform);

        StartCoroutine(StartVineAppear());
    }

    /// <summary>
    /// Starts the vine appearing and triggers the actual rear back for attacking when done appearing
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartVineAppear()
    {
        yield return new WaitForSeconds(.1f);
        _currentActiveVineScript.StartAppear(_playerTransform);

        //wait for fully appeared 
        yield return new WaitUntil(() => _currentActiveVineScript.GetIsAppeared());

        //trigger attack
        _currentActiveVineScript.StartAttack(_playerTransform.position);
    }

    /// <summary>
    /// Creates the enemy spawn vfx
    /// </summary>
    /// <param name="spawnTransform"> The transform to spawn at </param>
    private void CreateSpawnVfx(Transform spawnTransform)
    {
        VfxManager.Instance.GetMonsterSpawnVfx().PlayNextVfxInPool
            (spawnTransform.position, spawnTransform.rotation);
    }

    public bool CanAttack { get => _canAttack; set => _canAttack = value; }
}
