/*****************************************************************************
// File Name :         VineAttackGroup.cs
// Author :            Tommy Roberts
// Contributor :       Andrew Stapay, Ryan Swanson, Mark Hanson
// Creation Date :     10/9/2024
//
// Brief Description : Controls the functionality of handling all vine attacks
*****************************************************************************/

using UnityEngine;
/// <summary>
/// Contains an array of vine attacks
/// Can contain additional functionality as needed
/// </summary>
[System.Serializable]
public class VineAttackGroup
{
    [field:SerializeField] public VineAttack[] VineAttacksThisGroup { get; private set; }
}
