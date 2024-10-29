/*****************************************************************************
// File Name :         BossAttackActSystem.cs
// Author :            Mark Hanson
// Creation Date :     10/22/2024
//
// Brief Description : The system to manage what act the boss is on and also switch between them along with which attack comes out
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable]
public struct Act
{
    [field: Tooltip("Attacks within the Act")]
    [field: SerializeField] public BaseBossAttack[] ActAttacks { get; private set; }
}

/// <summary>
/// A class that contains multiple functions for the act system that are updated within a coroutine
/// </summary>
public class BossAttackActSystem : MonoBehaviour
{
    [Tooltip("Acts within the boss fight")]
    [SerializeField] private Act[] _bossFightActs;

    private int _currentAct;

    [Tooltip("# of attacks completed")]
    protected private int _attackCounter = 0;

    //The base boss attack system to listen out for attacks
    public BaseBossAttack AttackComponent { get; private set; }

    //The current act it is on
    protected private int _actCounter = 0;

    [Tooltip("Invoked when an act begins")]
    private UnityEvent _actBegin = new();

    [Tooltip("Invoked when an act ends")]
    private UnityEvent _actEnd = new();

    private void Start()
    {
        InitializeActVariables();
    }

    /// <summary>
    /// just for testing
    /// </summary>
    private void Update()
    {
        // TODO - Connect this to the end of the tutorial
        // Test the begin interior attack until act system is properly connected to the start of the game / end
        // of tutorial
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            BeginAct();
        }
    }


    /// <summary>
    /// Initializes act variables
    /// </summary>
    private void InitializeActVariables()
    {
        _currentAct = 0;
    }

    /// <summary>
    /// A way for attacks that are dropped in to listen for when an act ends
    /// </summary>
    private void InitializeActAttackListeners()
    {
        Act act = _bossFightActs[_currentAct];

        foreach(BaseBossAttack baseBossAttack in act.ActAttacks)
        {
            baseBossAttack.GetAttackEndEvent().AddListener(AttackEnd);
        }
    }

    private void RemoveActAttackListeners()
    {
        Act act = _bossFightActs[_currentAct];

        foreach (BaseBossAttack baseBossAttack in act.ActAttacks)
        {
            baseBossAttack.GetAttackEndEvent().RemoveListener(AttackEnd);
        }
    }

    private void BeginAct()
    {
        // Ensures that an act never attempts to begin if past the available acts
        if (_currentAct == _bossFightActs.Length)
        {
            return;
        }

        InitializeActAttackListeners();

        Act act = _bossFightActs[_currentAct];
        foreach (BaseBossAttack baseBossAttack in act.ActAttacks)
        {
            baseBossAttack.InvokeAttackBegin();
        }
    }


    /// <summary>
    /// Method for GetAttackEnd so the act system 
    /// </summary>
    private void AttackEnd()
    {
        _attackCounter++;

        CheckIfActCompleted();
    }

    /// <summary>
    /// A way of being able to cycle through as many attack as the acts need
    /// </summary>
    /// <returns></returns>
    private void CheckIfActCompleted()
    {
        if (_actCounter == _bossFightActs[_currentAct].ActAttacks.Length)
        {
            EndAct();
        }
    }

    /// <summary>
    /// Act end for attack
    /// </summary>
    private void EndAct()
    { 
        InvokeActEndEvent();

        RemoveActAttackListeners();

        if(_currentAct == _bossFightActs.Length)
        {
            // TODO: Game End Code Here
        }
    }

    #region Events

    /// <summary>
    /// Act beginning for attack
    /// </summary>
    private void InvokeBeginActEvent()
    {
        _actBegin?.Invoke();
    }

    /// <summary>
    /// A way for other scripts to see the act ending
    /// If needed something can listen to those from another script to do something
    /// </summary>
    private void InvokeActEndEvent()
    {
        _actEnd?.Invoke();
    }

    #endregion

    #region Getters

    public UnityEvent GetActBegin() => _actBegin;
    public UnityEvent GetActEnd() => _actEnd;

    #endregion
}
