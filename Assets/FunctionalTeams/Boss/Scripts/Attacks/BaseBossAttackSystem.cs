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
public abstract class BaseBossAttackSystem : MonoBehaviour, IBaseDamage
{
    [Tooltip("Throw tangible Game Object of Attack in here")]
    [SerializeField] protected GameObject[] _attackObjects;
    private UnityEvent _attackBegin = new();
    private UnityEvent _attackEnd = new();
    //For spawning in at one or more locations
    [Tooltip("Add all locations of where attack should be")]
    [SerializeField] protected Transform[] _spawnLocation;
    //For adjusting variable from modular damage interface outside of code
    [Tooltip("Enter Damage of this attack here")]
    [SerializeField] protected float _damageAmount;

    //Variables that are from modular damage interface
    public float DamageAmount { get; protected set; }
    public bool CanApplyDamage { get; protected set; }
    public UnityEvent<float> DamageEvent { get;protected set; }

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
    /// Use this as the damage that will go into the player health
    /// </summary>
    public virtual void ApplyDamage()
    {
        if (CanApplyDamage)
        {
            DamageEvent?.Invoke(DamageAmount);
        }
    }
    
    /// <summary>
    /// Attack beginning event for boss act
    /// </summary>
    protected virtual void AttackBegin()
    {
        InvokeAttackBegin();
    }
    
    /// <summary>
    /// Attack ending event for boss act
    /// </summary>
    protected virtual void AttackEnd()
    {
        InvokeAttackEnd();
    }
    
    #region Events
    
    protected void InvokeAttackBegin()
    {
        _attackBegin?.Invoke();
    }

    protected void InvokeAttackEnd()
    {
        _attackEnd?.Invoke();
    }

    #endregion
    
    #region Getters
    
    public UnityEvent GetAttackBegin() => _attackBegin;
    public UnityEvent GetAttackEnd() => _attackEnd;
    public UnityEvent<float> GetDamageEvent() => DamageEvent;
    
    #endregion
}
