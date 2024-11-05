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

    protected UnityEvent _onBeginAttack = new();
    protected UnityEvent  _onAttackEnd = new();

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
    }

    /// <summary>
    /// Stops the attack from playing
    /// </summary>
    protected virtual void EndAttack()
    {
        _isAttackActive = false;
        InvokeAttackEnd();
    }

    #region Events

    /// <summary>
    /// Invokes this attack's _attackBegin event
    /// </summary>
    public void InvokeAttackBegin()
    {
        _onBeginAttack?.Invoke();
    }

    /// <summary>
    /// Invokes this attack's _attackEnd event
    /// </summary>
    public void InvokeAttackEnd()
    {
        _onAttackEnd?.Invoke();
    }

    #endregion
    
    #region Getters
    
    public UnityEvent GetAttackBeginEvent() => _onBeginAttack;
    public UnityEvent GetAttackEndEvent() => _onAttackEnd;
    
    #endregion
}
