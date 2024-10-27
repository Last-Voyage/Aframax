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
public class RoomLockdownAttack : BaseBossAttack
{
    //attack 1 ref
    [Header("Attack 1")]
    [Space]

    [Tooltip("Link to indicator object that flashes red currently")]
    [SerializeField] private GameObject _bossAttack1Indicator;
    [SerializeField] private Material _lowOpacity;
    [SerializeField] private Material _hitOpacity;

    [Tooltip("The spawn locations of the attack")]
    [SerializeField] private Vector3[] _spawnLocations;

    [Tooltip("The scale of the attack (on a per room basis)")]
    [SerializeField] private Vector3[] _attackScales;

    [Tooltip("How fast the indicator blinks when it begins blinking")]
    [SerializeField] private float _startBlinkInterval = 1f;
    [Tooltip("How fast the indicator blinks right before it actually attacks")]
    [SerializeField] private float _endBlinkInterval = .1f;
    [Tooltip("How long the indicator will blink for")]
    [SerializeField] private float _blinkDuration = 3f;
    [Tooltip("How long the actual attack will stay covering the room")]
    [SerializeField] private float _hitBoxAppearDuration = 1f;

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

    /// <summary>
    /// unlinks attack script from boss manager
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    protected override void SubscribeToEvents()
    {
        BossAttackManager.BeginRoomLockdownAttack += ActivateThisAttack;
    }

    protected override void UnsubscribeToEvents()
    {
        BossAttackManager.BeginRoomLockdownAttack -= ActivateThisAttack;
    }

    protected override void BeginAttack()
    {
        base.BeginAttack();
    }

    protected override void EndAttack()
    {
        base.EndAttack();
    }

    /// <summary>
    /// pretty self explanatory
    /// </summary>
    private void ActivateThisAttack()
    {
        StartCoroutine(PerformAttack());
    }

    /// <summary>
    /// creates an indicator that the room is about to be attacked, and then attacks everything in room
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformAttack()
    {
        // Determine which room to attack
        AttackRandomRoom();

        // tell the attack manager that we are attacking
        BossAttackManager.Instance.AttackInProgress = true;

        // setting attack indicator
        _bossAttack1Indicator.GetComponent<MeshRenderer>().material = _lowOpacity;
        var attackMeshRenderer = _bossAttack1Indicator.GetComponent<MeshRenderer>();
        var attackCollider = _bossAttack1Indicator.GetComponent<Collider>();

        //start blinking indicating attack will happen soon
        float elapsedTime = 0f;
        while(elapsedTime < _blinkDuration)
        {
            float currentBlinkInterval = Mathf.Lerp(_startBlinkInterval, _endBlinkInterval, elapsedTime / _blinkDuration);
            attackMeshRenderer.enabled = true;
            yield return new WaitForSeconds(currentBlinkInterval);
            attackMeshRenderer.enabled = false;
            yield return new WaitForSeconds(currentBlinkInterval);
            elapsedTime+= currentBlinkInterval * 2;
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
    }

    /// <summary>
    /// Chooses a random room inside the boat to attack
    /// </summary>
    private void AttackRandomRoom()
    {
        int randomRoom = UnityEngine.Random.Range(0, _spawnLocations.Length);

        transform.position = _spawnLocations[randomRoom];
        transform.localScale = _attackScales[randomRoom];
    }
}
