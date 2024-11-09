/*****************************************************************************
// File Name :         VineAttack.cs
// Author :            Ryan Swanson
// Creation Date :     11/7/2024
//
// Brief Description : Controls the functionality for the bosses vine attack
*****************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Functionality for the spawned tentacles for the vine attack
/// </summary>
public class VineAttack : MonoBehaviour
{
    [Tooltip("How long the attack hitbox remains active")]
    [SerializeField] private float _attackDuration;

    /// <summary>
    /// READ ME --------------
    /// At some point I would like to turn the Lockdown Enemy Room into a more general script so that it's
    /// functionality can
    /// </summary>
    [Tooltip("The time between when the vine can attempt an attack")]
    [SerializeField] private float _attackCooldown;
    private bool _canAttack;

    [SerializeField] private Transform _vineAttackSpawnLocation;
    [SerializeField] private GameObject _vineAttackPrefab;

    private AttackWarningZone _warningZone;
    /// <summary>
    /// NOTE THIS HAS BEEN RENAMED IN ANOTHER COMMIT
    /// I WOULD LIKE TO TURN THIS SCRIPT FROM BEING ASSOCIATED WITH ONLY THE PATROL ENEMIES INTO BEING A GENERAL
    /// SCRIPT FOR DETECTING WHEN A PLAYER ENTERS A ROOM
    /// So yes I get it that it doesn't make sense to be using the functionality from a seperate attack here
    /// </summary>
    private PatrolEnemyRoom _roomDetection;

    private GameObject _spawnedVine;

    private UnityEvent _onVinesInRoomDestroyed = new();

    private GameObject _attackGameObject;

    private void Start()
    {
        SetStartingValues();
        SubscribeToEvents();
    }

    /// <summary>
    /// Sets any values that are needed before functionality begins
    /// </summary>
    private void SetStartingValues()
    {
        _canAttack = true;
        _warningZone = GetComponent<AttackWarningZone>();
        _roomDetection = GetComponent<PatrolEnemyRoom>();

        _attackGameObject = transform.GetChild(0).GetChild(0).gameObject;
        _attackGameObject.SetActive(false);
    }

    
    public void StartVineAttack()
    {
        _spawnedVine = Instantiate(_vineAttackPrefab);

        
    }

    private void PlayerEnteredRoom()
    {
        if (_canAttack)
        {
            _canAttack = false;
            StartAttackProcess();
        }
    }

    private void StartAttackProcess()
    {
        _warningZone.StartWarningZone();
        _warningZone.GetOnWarningEndEvent().AddListener(BeginAttackDamage);
    }

    /// <summary>
    /// Activates the hitbox after the warning has concluded
    /// </summary>
    private void BeginAttackDamage()
    {
        _attackGameObject.SetActive(true);
        StartCoroutine(AttackDamageProcess());
    }

    /// <summary>
    /// The duration of the attack
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackDamageProcess()
    {
        yield return new WaitForSeconds(_attackDuration);
        AttackDamageEnd();
        yield return new WaitForSeconds(_attackCooldown);
        CooldownOver();
    }

    /// <summary>
    /// Concludes the attack
    /// </summary>
    private void AttackDamageEnd()
    {
        _attackGameObject.SetActive(false);
    }

    private void CooldownOver()
    {
        _canAttack = true;
        CheckPlayerIsAlreadyInRoom();
    }

    private void CheckPlayerIsAlreadyInRoom()
    {
        if(_roomDetection.IsPlayerInRoom())
        {
            PlayerEnteredRoom();
        }
    }

    public void SubscribeToEvents()
    {
        _roomDetection.GetOnPlayerRoomEnterEvent().AddListener(PlayerEnteredRoom);
    }

    private void UnsubscribeToEvents()
    {
        _roomDetection.GetOnPlayerRoomEnterEvent().RemoveListener(PlayerEnteredRoom);
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }
}
