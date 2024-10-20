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
/// A list of variables and functions that can be attached to other scripts
/// </summary>
public interface ModularDamage
{
    //The ending damage value used to be passed on to health
    void ApplyDamage();
    //These variables even though currently private will be public hence why they are PascalCase
    [Tooltip("Base damage value")]
    float DamageAmount { get;}
    //When in attack mode it will give actual damage
    //attack mode would be when a move like the tongue is visually swinging or agressing but for the player
    //it is when your harpon would just be shot out ideally not used when reeling back harpoon
    bool CanApplyDamage { get;}
    //event will mostly be used to listen to the universal health variables
    UnityEvent<float> DamageEvent { get; }
}
