/*****************************************************************************
// File Name :         BaseBossAttackSystem.cs
// Author :            Mark Hanson
// Creation Date :     10/21/2024
//
// Brief Description : The base of each attack the boss does to make each attack easier to set up
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The abstract class of holding all the functions of the attack
/// literally from beginning to end
/// </summary>
public abstract class BaseBossAttackSystem : MonoBehaviour
{
    [Tooltip("Throw tangible Game Object of Attack in here")]
    [SerializeField] protected GameObject[] _attackObjects;

    //For spawning in at one or more locations
    [Tooltip("Add all locations of where attack should be")]
    [SerializeField] protected Transform[] _spawnLocation;

    //For adjusting variable from modular damage interface outside of code
    [Tooltip("Enter Damage of this attack here")]
    [SerializeField] protected float _damageAmount;

    //Variables that are from modular damage interface
    public float DamageAmount { get; protected set; }

    private UnityEvent<BaseBossAttackSystem> _attackBegin = new();
    private UnityEvent<BaseBossAttackSystem> _attackEnd = new();

    /// <summary>
    /// Sets initial values
    /// </summary>
    protected virtual void Awake()
    {
        DamageAmount = _damageAmount;
        DetermineRandomAttackLocation();
    }
    
    /// <summary>
    /// Randomizer for the starting position
    /// </summary>
    protected virtual void DetermineRandomAttackLocation()
    {
        foreach (GameObject Attack in _attackObjects)
        {
            Attack.transform.position = _spawnLocation[Random.Range(0, _spawnLocation.Length)].position;
        }
    }
    
    /// <summary>
    /// Attack beginning event for boss phase
    /// </summary>
    protected virtual void AttackBegin()
    {
        InvokeAttackBegin();
    }
    
    /// <summary>
    /// Attack ending event for boss phase
    /// </summary>
    protected virtual void AttackEnd()
    {
        InvokeAttackEnd();
    }
    
    #region Events
    
    protected void InvokeAttackBegin()
    {
        _attackBegin?.Invoke(this);
    }

    protected void InvokeAttackEnd()
    {
        _attackEnd?.Invoke(this);
    }

    #endregion
    
    #region Getters
    
    public UnityEvent<BaseBossAttackSystem> GetAttackBegin() => _attackBegin;
    public UnityEvent<BaseBossAttackSystem> GetAttackEnd() => _attackEnd;
    
    #endregion
}