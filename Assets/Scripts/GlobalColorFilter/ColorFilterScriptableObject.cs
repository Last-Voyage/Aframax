using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Data", 
    menuName = "ScriptableObjects/ColorFilterScriptableObject", 
    order = 1
)]
public class ColorFilterScriptableObject : ScriptableObject
{
    [Serializable]
    public struct ColorChannelSettings
    {
        [Range(0.0F, 1.0F)] public float r;
        [Range(0.0F, 1.0F)] public float g;
        [Range(0.0F, 1.0F)] public float b;
    }

    [Serializable]
    public struct ColorProfile
    {
        [Range(0.0F, 1.0F)] public float percentage;
        public ColorChannelSettings redChannel;
        public ColorChannelSettings blueChannel;
        public ColorChannelSettings greenChannel;
    }

    public ColorProfile Normal;
    public ColorProfile Protanopia;
    public ColorProfile Protanomaly;
    public ColorProfile Deuteranopia;
    public ColorProfile Deuteranomaly;
}
