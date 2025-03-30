/******************************************************************************
// File Name:       HarpoonVisualAudioEffectsBank.cs
// Author:          Nick Rice and Ryan Swanson
// Creation Date:   February 23rd, 2025
//
// Description:     Holds the data for what vfx goes with each material
******************************************************************************/

using UnityEngine;

/// <summary>
/// Contains the data used in determining what materials are associated with what effects
/// </summary>
[System.Serializable]
public class HarpoonVisualAudioEffectsBank
{
    public Material AssociatedMaterial;
    public HarpoonCollisionType CollisionType;
}
