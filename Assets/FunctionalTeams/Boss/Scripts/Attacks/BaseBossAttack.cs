/*****************************************************************************
// File Name :         BaseBossAttackSystem.cs
// Author :            Mark Hanson
// Contributors:       Andrew Stapay, Andrea Swihart-DeCoster
// Creation Date :     10/21/2024
//
// Brief Description : The base of each attack the boss does to make each attack easier to set up
*****************************************************************************/

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The abstract class of holding all the functions of the attack
/// literally from beginning to end
/// </summary>
public class BaseBossAttack : MonoBehaviour
{
    /// <summary>
    /// If the attack is currently active and playing
    /// </summary>
    protected bool _isAttackActive;

    private UnityEvent _attackBegin = new();
    private UnityEvent  _attackEnd = new();

    /// <summary>
    /// Subscribe to any necessary events
    /// </summary>
    protected virtual void SubscribeToEvents()
    {
        //TODO : Implement any universal events here
    }

    /// <summary>
    /// Unsubscribe from any necessary events
    /// </summary>
    protected virtual void UnsubscribeToEvents()
    {
        //TODO : Implement any universal events here
    }

    /// <summary>
    /// Attack beginning event for boss phase
    /// </summary>
    protected virtual void BeginAttack()
    {
        _isAttackActive = true;
        InvokeAttackBegin();
    }

    /// <summary>
    /// Stops the attack from playing
    /// </summary>
    protected virtual void EndAttack()
    {
        _isAttackActive = false;
        InvokeAttackEnd();
        gameObject.SetActive(false);
    }

    #region Events

    /// <summary>
    /// Invokes this attack's _attackBegin event
    /// </summary>
    protected void InvokeAttackBegin()
    {
        _attackBegin?.Invoke();
    }

    /// <summary>
    /// Invokes this attack's _attackEnd event
    /// </summary>
    protected void InvokeAttackEnd()
    {
        _attackEnd?.Invoke();
    }

    #endregion
    
    #region Getters
    
    public UnityEvent GetAttackBegin() => _attackBegin;
    public UnityEvent GetAttackEnd() => _attackEnd;
    
    #endregion
}