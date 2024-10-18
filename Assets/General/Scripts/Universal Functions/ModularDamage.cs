/*****************************************************************************
// File Name :         ModularDamage.cs
// Author :            Mark Hanson
// Creation Date :     10/1/2024
//
// Brief Description : The functational application of the damage system that is used to interact with a health system
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A manipulator of Universal health that allows specific damage numbers to go through
/// </summary>
public class ModularDamage : MonoBehaviour
{
    [Tooltip("Base damage value")]
    protected float _damageAmount;
    protected bool _canApplyDamage = false;
    //event will mostly be used to listen to the universal health variables
    private UnityEvent<float> _damageEvent = new ();

    /// <summary>
    /// The ending damage value used to be passed on to health
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void ApplyDamage()
    {
        //When in attack mode it will give actual damage
        //attack mode would be when a move like the tongue is visually swinging or agressing but for the player
        //it is when your harpon would just be shot out ideally not used when reeling back harpoon
        if (_canApplyDamage)
        {
            _damageEvent?.Invoke(_damageAmount);
        }
    }
    #region Getters
    public UnityEvent<float> GetDamageEvent() => _damageEvent;
    #endregion
}
