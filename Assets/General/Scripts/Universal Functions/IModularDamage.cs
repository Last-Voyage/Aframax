/*****************************************************************************
// File Name :         ModularDamage.cs
// Author :            Mark Hanson
// Creation Date :     10/1/2024
//
// Brief Description : The collective index of the damage system that is used to interact with a health system
*****************************************************************************/

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A list of variables and functions that can be attached to other scripts. When implemented,all functions must be 
/// used from below which should allow the damage number for editing, the switching bool of when damage is able to be
/// done,a damage event for listening to other scripts, and a function for all the variables.
/// </summary>
public interface IModularDamage
{
    //These variables even though currently private will be public hence why they are PascalCase
    [Tooltip("Base damage value")]
    float DamageAmount { get; }

    //Ensures damage is only applied when true
    bool CanApplyDamage { get; }

    //event will mostly be used to listen to the universal health variables
    UnityEvent<float> DamageEvent { get; }
    /// <summary>
    /// The ending damage value used to be passed on to health
    /// </summary>
    void ApplyDamage();
}
