/*****************************************************************************
// File Name :         GeneralPlayerCollisionTrigger.cs
// Author :            Ryan Swanson
// Contributor:        
// Creation Date :     4/6/25
//
// Brief Description : Provides a general collision trigger for design
*****************************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides a general collision trigger for design to use how they want
/// </summary>
public class GeneralPlayerCollisionTrigger : MonoBehaviour
{
    [SerializeField] private bool _doesDestroyOnContact;
    [SerializeField] private UnityEvent _onPlayerCollision;
    
    [SerializeField] private bool _doesDestroyOnDelayedContact;
    [SerializeField] private float _delayTime;
    [SerializeField] private UnityEvent _onDelayedPlayerCollision;
    private WaitForSeconds _waitDelay;
    private bool _waitingOnDelay;

    /// <summary>
    /// Performs any needed set up
    /// </summary>
    private void Start()
    {
        _waitDelay = new WaitForSeconds(_delayTime);
    }

    /// <summary>
    /// Called when contacting the player
    /// </summary>
    public void PlayerContact()
    {
        if (_waitingOnDelay)
        {
            return;
        }
        
        PlayerContactEffect();
        
        if (_delayTime > 0)
        {
            StartCoroutine(DelayContactEffect());
        }
    }

    /// <summary>
    /// The effect to occur on player contact
    /// </summary>
    private void PlayerContactEffect()
    {
        _onPlayerCollision?.Invoke();
        if(_doesDestroyOnContact )
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// The delayed effect to occur on player contact
    /// </summary>
    private void DelayedContactEffect()
    {
        _onDelayedPlayerCollision?.Invoke();
        if (_doesDestroyOnDelayedContact)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Delays the player contact effect
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayContactEffect()
    {
        _waitingOnDelay = true;
        yield return _waitDelay;
        _waitingOnDelay = true;
        
        DelayedContactEffect();
    }
    
    /// <summary>
    /// Called to remove any listeners
    /// </summary>
    private void OnDestroy()
    {
        _onPlayerCollision?.RemoveAllListeners();
    }
}
