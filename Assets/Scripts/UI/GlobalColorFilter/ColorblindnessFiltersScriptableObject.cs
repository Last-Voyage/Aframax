/**********************************************************************************************************************
// File Name :          ColorblindnessFiltersScriptableObject.cs
// Author :             Miles Rogers
// Creation Date :      4/15/2025
//
// Brief description :  Scriptable object to manage settings for simulating different types of colorblindness.
**********************************************************************************************************************/

using System;
using UnityEngine;

/// <summary>
/// Scriptable object to manage settings for simulating different types of colorblindness.
/// => (Can be created via right click menu in inspector)
/// </summary>
[CreateAssetMenu(
    fileName = "Data", 
    menuName = "ScriptableObjects/ColorblindnessFilters", 
    order = 1
)]
public class ColorblindnessFiltersScriptableObject : ScriptableObject
{
    /// <summary>
    /// Per-channel color values for ColorProfiles
    /// </summary>
    [Serializable]
    public struct ColorChannelSettings
    {
        [Range(0.0F, 1.0F)] public float r;
        [Range(0.0F, 1.0F)] public float g;
        [Range(0.0F, 1.0F)] public float b;
    }

    /// <summary>
    /// Color channel attenuation profile
    /// </summary>
    [Serializable]
    public struct ColorProfile
    {
        public ColorChannelSettings redChannel;
        public ColorChannelSettings greenChannel;
        public ColorChannelSettings blueChannel;
    }

    /// <summary>
    /// Colorblindness profile: none
    /// </summary>
    public ColorProfile None;
    
    /// <summary>
    /// Colorblindness profile: Protanopia
    /// </summary>
    public ColorProfile Protanopia;
    
    /// <summary>
    /// Colorblindness profile: Protanomaly
    /// </summary>
    public ColorProfile Protanomaly;
    
    /// <summary>
    /// Colorblindness profile: Deuteranopia
    /// </summary>
    public ColorProfile Deuteranopia;
    
    /// <summary>
    /// Colorblindness profile: Deuteranomaly
    /// </summary>
    public ColorProfile Deuteranomaly;
    
    /// <summary>
    /// Colorblindness profile: Tritanopia
    /// </summary>
    public ColorProfile Tritanopia;
    
    /// <summary>
    /// Colorblindness profile: Tritanomaly
    /// </summary>
    public ColorProfile Tritanomaly;
    
    /// <summary>
    /// Colorblindness profile: Achromatopsia
    /// </summary>
    public ColorProfile Achromatopsia;
    
    /// <summary>
    /// Colorblindness profile: Achromatomaly
    /// </summary>
    public ColorProfile Achromatomaly;
}
