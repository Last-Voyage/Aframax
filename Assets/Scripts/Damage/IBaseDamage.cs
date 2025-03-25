/*****************************************************************************
// File Name :         ModularDamage.cs
// Author :            Mark Hanson
// Contributor :       Andrea Swihart-DeCoster
// Creation Date :     10/1/2024
//
// Brief Description : The collective index of the damage system that is used to interact with a health system
*****************************************************************************/

using UnityEngine;

/// <summary>
/// The interface the will be referred defining all damage functionality.
/// </summary>
public interface IBaseDamage
{
    /// <summary>
    /// The ending damage value used to be passed on to health
    /// </summary>
    void ApplyDamage(GameObject damageRecipient);
}
