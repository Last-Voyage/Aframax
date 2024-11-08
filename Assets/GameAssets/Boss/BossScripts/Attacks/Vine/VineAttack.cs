/*****************************************************************************
// File Name :         VineAttack.cs
// Author :            Ryan Swanson
// Creation Date :     11/7/2024
//
// Brief Description : Controls the functionality for the bosses vine attack
*****************************************************************************/

using System.Collections;
using UnityEngine;

/// <summary>
/// Functionality for the spawned tentacles for the vine attack
/// </summary>
[RequireComponent(typeof(AttackWarningZone))]
public class VineAttack : MonoBehaviour
{
    [Tooltip("The range the player must be within before it attacks")]
    [SerializeField] private float _attackRange;
    [Tooltip("How long the attack hitbox remains active")]
    [SerializeField] private float _attackDuration;

    private GameObject _attackGameObject;
    private AttackWarningZone _warningZone;

    private Transform _attackTargetLocation;

    private Coroutine _attackRangeChecksCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        SetStartingValues();
        SubscribeToEvents();
        StartAttackRangeChecks();
    }

    /// <summary>
    /// Sets any values that are needed before functionality begins
    /// </summary>
    private void SetStartingValues()
    {
        _attackGameObject = transform.GetChild(0).GetChild(0).gameObject;
        _attackGameObject.SetActive(false);

        _warningZone = GetComponent<AttackWarningZone>();

        _attackTargetLocation = PlayerMovementController.Instance.transform;
    }

    /// <summary>
    /// Starts checking if the player is in the attack range
    /// </summary>
    private void StartAttackRangeChecks()
    {
        if(_attackRangeChecksCoroutine == null)
        {
            _attackRangeChecksCoroutine = StartCoroutine(PlayerInAttackRangeChecks());
        }
    }

    /// <summary>
    /// creates an indicator that the room is about to be attacked, and then attacks everything in room
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerInAttackRangeChecks()
    {
        while (Vector3.Distance(transform.position, _attackTargetLocation.position) > _attackRange)
        {
            yield return null;
        }
        AttackWarningStart();
    }

    /// <summary>
    /// Starts the attack warning
    /// </summary>
    private void AttackWarningStart()
    {
        _attackRangeChecksCoroutine = null;
        
        _warningZone.StartWarningZone();
    }

    /// <summary>
    /// Activates the hitbox after the warning has concluded
    /// </summary>
    private void BeginAttack()
    {
        _attackGameObject.SetActive(true);
        StartCoroutine(AttackProcess());
    }

    /// <summary>
    /// The duration of the attack
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(_attackDuration);
        AttackEnd();
    }

    /// <summary>
    /// Concludes the attack
    /// </summary>
    private void AttackEnd()
    {
        _attackGameObject.SetActive(false);

        //Loops back around to continue attacking
        StartAttackRangeChecks();
    }

    private void SubscribeToEvents()
    {
        _warningZone.GetOnWarningEndEvent().AddListener(BeginAttack);
    }

    private void UnsubscribeToEvents()
    {
        _warningZone.GetOnWarningEndEvent().RemoveListener(BeginAttack);
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

}
