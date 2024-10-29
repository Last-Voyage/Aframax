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
using UnityEngine.SceneManagement;

/// <summary>
/// A class that contains multiple functions for the act system that are updated within a coroutine
/// </summary>
public class BossAttackActSystem : MonoBehaviour
{
    //Should hold a list of Integers which each play the role of current max attacks for each act
    [Tooltip("How many attacks do you want per act")]
    [SerializeField] private int[] _attacksPerAct;
    //The max attacks for the current act we are on
    protected private int _currentMaxAttacks;
    //Keeps the attacks going through the attack collection and not back tracking through previosly used attacks
    protected private int _attackCounter = 0;
    //Keeps tracked of attack that are over then matches with attack counter to determine if an act is over
    protected int _attackOverCounter = 0;
    //The base boss attack system to listen out for attacks
    public BaseBossAttack AttackComponent { get; private set; }

    //The current act it is on
    protected private int _actCounter = 0;
    //A switch bool that works in tandem with the act counter
    protected private bool _isActOver;
    //Ideally have a empty hold 2 or more attacks in the one empty and match it up with the act number
    [Tooltip("Throw in attack empty holders that aligns with act")]
    [SerializeField] private GameObject[] _attackCollection;
    //Events for the atacks to listen for
    private UnityEvent _actBegin = new();
    private UnityEvent _actEnd = new();
    private UnityEvent _endOfGameScene = new();
    /// <summary>
    /// Beginning method to start phase at the beginning
    /// </summary>
    private void Awake()
    {
        _currentMaxAttacks = _attacksPerAct[_actCounter];
        InvokeActBegin();
        AttackListentoActs();
        ActManagement();
        AttackManagement();
    }
    /// <summary>
    /// A way for attacks that are dropped in to listen for when an act ends
    /// </summary>
    private void AttackListentoActs()
    {
        if (TryGetComponent<BaseBossAttack>(out BaseBossAttack baseBossAttackSystem))
        {
            AttackComponent = baseBossAttackSystem;
            AttackComponent.GetAttackEnd().AddListener(AttackEnd);
        }
    }
    /// <summary>
    /// Method for GetAttackEnd so the act system 
    /// </summary>
    protected void AttackEnd()
    {
        ++_attackOverCounter;
        AttackManagement();
    }
    /// <summary>
    /// A way of being able to cycle through as many attack as the acts need
    /// </summary>
    /// <returns></returns>
    protected void AttackManagement()
    {
         if(_isActOver)
         {
             for (int i = 0; i <= _currentMaxAttacks; ++i)
             {
                 _attackCollection[_attackCounter].SetActive(true);
                 _attackCounter++;
             }
            _isActOver = false;
         }
    }
    /// <summary>
    /// Act beginning for attack
    /// </summary>
    protected virtual void ActBegin()
    {
        InvokeActBegin();
    }
    /// <summary>
    /// Act end for attack
    /// </summary>
    protected virtual void ActEnd()
    {
        InvokeActEnd();
    }
    protected virtual void EndOfGameScene()
    {
        InvokeEndOfGameScene();
    }
    /// <summary>
    /// A way of moving to a new act while showing through events whats its doing to other scripts
    /// </summary>
    protected virtual void NextAct()
    {
        InvokeActEnd();
        _actCounter++;
        _currentMaxAttacks = _attacksPerAct[_actCounter];
        InvokeActBegin();
    }
    /// <summary>
    /// Act Management works as a way to listen to attack events and elaborate on what act it should be on
    /// And when it is over.
    /// </summary>
    /// <returns></returns>
    protected void ActManagement()
    {
            //When the act is over move to the next one while notifying event system
            if(_isActOver)
            {
                NextAct();
            }
            if(_actCounter > _attacksPerAct.Length)
            {
                InvokeEndOfGameScene();
                SceneManager.LoadScene("TitleSceen", LoadSceneMode.Single);

            }
            if (_attackCounter == _attackOverCounter)
            {
                _isActOver = true;
            }
    }

    #region Events
    /// <summary>
    /// A way for other scripts to see the act beginning
    /// If needed something can listen to those from another script to do something
    /// </summary>
    private void InvokeActBegin()
    {
        _actBegin?.Invoke();
    }
    /// <summary>
    /// A way for other scripts to see the act ending
    /// If needed something can listen to those from another script to do something
    /// </summary>
    private void InvokeActEnd()
    {
        _actEnd?.Invoke();
    }
    /// <summary>
    /// A way for other scripts to see the end of game to shut down stuff if need be
    /// </summary>
    private void InvokeEndOfGameScene()
    {
        _endOfGameScene?.Invoke();
    }
    #endregion
    #region Getters
    public UnityEvent GetActBegin() => _actBegin;
    public UnityEvent GetActEnd() => _actEnd;
    #endregion
}
