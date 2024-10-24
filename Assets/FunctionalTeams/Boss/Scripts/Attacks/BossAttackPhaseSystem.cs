/*****************************************************************************
// File Name :         BaseHealth.cs
// Author :            Mark Hanson
// Creation Date :     10/22/2024
//
// Brief Description : The system to manage what phase the boss is on and also switch between them along with which attack comes out
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossAttackPhaseSystem : MonoBehaviour
{
    //Should hold a list of numbers which are being swapped into the current max attacks
    [Tooltip("How many attacks do you want per phase")]
    protected private int[] _attacksPerPhase;

    protected private int _currentMaxAttacks;

    protected private int _attackCounter = 0;

    //The current phase it is on
    protected private int _phaseCounter = 0;
    //A switch bool that works in tandem with the phase counter
    protected private bool _isPhaseOver;
    //Ideally have a empty hold 2 or more attacks in the one empty and match it up with the phase number
    [Tooltip("Through in attack empty holder that aligns with phase")]
    [SerializeField] private GameObject[] _attackCollection;
    private UnityEvent<BossAttackPhaseSystem> _phaseBegin;
    private UnityEvent<BossAttackPhaseSystem> _phaseEnd;

    private void Awake()
    {
        _isPhaseOver = false;
        _attackCollection[_phaseCounter].SetActive(true);
        InvokePhaseBegin();
        _currentMaxAttacks = _attacksPerPhase[_phaseCounter];
    }

    private IEnumerator AttackManagement()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if(_isPhaseOver )
            {
                for (int i = 0; i <= _currentMaxAttacks; ++i)
                {
                    _attackCollection[_attackCounter].SetActive(true);
                    _attackCounter++;
                }
                _isPhaseOver = false;
            }
        }
    }

    /// <summary>
    /// Phase Management works as a way to listen to attack events and elaborate on what phase it should be on
    /// And when it is over.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PhaseManagement()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            if(_isPhaseOver)
            {
                InvokePhaseEnd();
                _phaseCounter++;
                _currentMaxAttacks = _attacksPerPhase[_phaseCounter];
                InvokePhaseBegin();
            }
            if(_phaseCounter > _attackCollection.Length)
            {
                //Does game end here??
            }
        }
    }

    #region Events
    private void InvokePhaseBegin()
    {
        _phaseBegin?.Invoke(this);
    }
    private void InvokePhaseEnd()
    {
        _phaseEnd?.Invoke(this);
    }
    #endregion
    #region Getters
    public UnityEvent<BossAttackPhaseSystem> GetPhaseBegin() => _phaseBegin;
    public UnityEvent<BossAttackPhaseSystem> GetPhaseEnd() => _phaseEnd;
    #endregion
}
