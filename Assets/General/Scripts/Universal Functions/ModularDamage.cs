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
    protected bool _attackMode;
    //event will mostly be used to listen to the universal health variables
    private UnityEvent<float> _damageEvent = new ();
    //These are here if design ever wants critical hits in the future so right now they are unused
    //private int _criticalDamage;
    //private bool _critMode = false;

    /// <summary>
    /// The ending damage value used to be passed on to health
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void GiveDamage(float damage)
    {
        //When in attack mode it will give actual damage
        //attack mode would be when a move like the tongue is visually swinging or agressing but for the player
        //it is when your harpon would just be shot out ideally not used when reeling back harpoon
        if (_attackMode)
        {
            damage = _damageAmount;
            _damageEvent?.Invoke(damage);
        }
        //If not in attack mode but still colliding with an object that has a hitbox just give 0 damage
        else
        {
            damage = 0;
            _damageEvent?.Invoke(damage);
        }
    }
    #region Getters
    public UnityEvent<float> GetDamageEvent() => _damageEvent;
    #endregion
}
