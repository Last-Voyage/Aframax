/*****************************************************************************
// File Name :         UniversalHealthSystem.cs
// Author :            Mark Hanson
// Creation Date :     9/27/2024
//
// Brief Description : A storage for health usable by players and enemies alike
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// holder of all definitions to do with health
/// </summary>
public class UniversalHealthSystem : MonoBehaviour
{
    /// <summary>
    /// class to contain all health definitions and general set up of max and current health along with its a player or not.
    /// </summary>
     public class Health
     {
           public float MaxHealth;
           public float CurrentHealth;
        // A determiner of if the object is the player or not. If it is a player there will be special functions for it in the UniveralHealth.cs script
          public bool IsPlayer;
           public bool WaveClear = false;
        /// <summary>
        /// health attributes that are editable
        /// </summary>
        /// <param name="_mHP"></param>
        /// <param name="_cHP"></param>
        /// <param name="_iP"></param>
         public Health (float _mHP, float _cHP, bool _iP)
         {
               MaxHealth = _mHP;
               CurrentHealth = _cHP;
               IsPlayer = _iP;
          }
        /// <summary>
        /// default health settles if nothing is edited
        /// </summary>
         public Health()
         {
                MaxHealth = 1;
                CurrentHealth = 1;
                IsPlayer = false;
         }
     }
}
