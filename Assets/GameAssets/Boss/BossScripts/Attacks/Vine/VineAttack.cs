/*****************************************************************************
// File Name :         VineAttack.cs
// Author :            Ryan Swanson
// Creation Date :     11/7/2024
//
// Brief Description : Controls the functionality for the bosses vine attack
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AttackWarningZone))]
public class VineAttack : MonoBehaviour
{
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackDuration;

    private GameObject _attackGameObject;
    private AttackWarningZone _warningZone;

    private Transform _attackTargetLocation;

    private Coroutine _attackRangeChecksCoroutine;
    private Coroutine _attackCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        SetStartingValues();
        StartAttackRangeChecks();
    }

    private void SetStartingValues()
    {
        _attackGameObject = transform.GetChild(0).gameObject;
        _attackGameObject.SetActive(false);

        _warningZone.GetComponent<AttackWarningZone>();

        _attackTargetLocation = PlayerMovementController.Instance.transform;
    }

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
        while (Vector3.Distance(transform.position, _attackTargetLocation.position) < _attackRange)
        {
            yield return null;
        }
        AttackWarningStart();
    }


    private void AttackWarningStart()
    {
        _attackRangeChecksCoroutine = null;
        _warningZone.GetOnWarningEndEvent().AddListener(BeginAttack);
        _warningZone.StartWarningZone();
    }

    private void BeginAttack()
    {
        _warningZone.GetOnWarningEndEvent().RemoveListener(BeginAttack);
        _attackGameObject.SetActive(true);
        _attackCoroutine = StartCoroutine(AttackProcess());
    }

    private IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(_attackDuration);
        AttackEnd();
    }

    private void AttackEnd()
    {
        _attackCoroutine = null;
        _attackGameObject.SetActive(false);
    }

}
