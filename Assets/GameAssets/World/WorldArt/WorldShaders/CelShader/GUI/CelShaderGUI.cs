/**************************************************************************
// File Name :          CelShaderGui.cs
// Author :             Miles Rogers
// Creation Date :      9/18/2024
//
// Brief Description :  Custom editor GUI for our Cel Shader.
**************************************************************************/

// Exclude script from build
#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;

public class CelShaderGui : ShaderGUI
{
    // Foldout states
    private bool _showTilingFoldout = true;
    private bool _showColorsFoldout = true;
    private bool _showNormalFoldout = true;
    private bool _showMetallicFoldout = false;
    private bool _showEmissionFoldout = false;
    private bool _showPomFoldout = false;
    private bool _showVertexPaintFoldout = false;
    private bool _showVertexSwayFoldout = false;
    private bool _showPostFoldout = false;
    
    // When the ShaderGUI is drawn (immediate-mode)
    override public void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // Header
        EditorGUILayout.LabelField("LV Cel Shader (Standard)", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        CoreEditorUtils.DrawSplitter();
        
        // Render the Tiling Foldout
        _showTilingFoldout = CoreEditorUtils.DrawHeaderFoldout(
            "Tiling",
            _showTilingFoldout,
            false,
            (Func<bool>)null,
            null
        );
        DrawProperties(
            _showTilingFoldout,
            new Dictionary<string, string> {
                { "_TilingValue", "Tiling Values"},
                { "_TextureOffset", "Texture Offset" }
            },
            properties,
            materialEditor
        );
        
        // Render the Base Color foldout
        _showColorsFoldout = CoreEditorUtils.DrawHeaderFoldout(
            "Base Color",
            _showColorsFoldout,
            false,
            (Func<bool>)null,
            null
        );
        DrawProperties(
            _showColorsFoldout,
            new Dictionary<string, string> {
                { "_BaseColorTint", "Base Color Tint"},
                { "_BaseColor", "Base Color (Texture)" }
            },
            properties,
            materialEditor
        );
        
        // Render the Normal Map foldout
        _showNormalFoldout = CoreEditorUtils.DrawHeaderFoldout(
            "Normal",
            _showNormalFoldout,
            false,
            (Func<bool>)null,
            null
        );
        DrawProperties(
            _showNormalFoldout,
            new Dictionary<string, string> {
                { "_Normal", "Normal Map (Texture)"},
                { "_NormalMapIntensity", "Normal Map Intensity" }
            },
            properties,
            materialEditor
        );
        
        // Render the Metal foldout
        _showMetallicFoldout = CoreEditorUtils.DrawHeaderFoldout(
            "Metal",
            _showMetallicFoldout,
            false,
            (Func<bool>)null,
            null
        );
        DrawProperties(
            _showMetallicFoldout,
            new Dictionary<string, string> {
                { "_Metallic", "Metallic Map (Texture)"},
                { "_MetalBaseColor", "Metal Base Color" },
                { "_MetalHighlightColor", "Metal Highlight Color" },
                { "_MetalBackground", "Metal Background Color" }
            },
            properties,
            materialEditor
        );
        
        // Render the Emission foldout
        _showEmissionFoldout = CoreEditorUtils.DrawHeaderFoldout(
            "Emission",
            _showEmissionFoldout,
            false,
            (Func<bool>)null,
            null
        );
        DrawProperties(
            _showEmissionFoldout,
            new Dictionary<string, string> {
                { "_Emissive", "Emissive Map (Texture)"},
                { "_EmissiveIntensity", "Emissive Intensity" }
            },
            properties,
            materialEditor
        );
        
        // Render the POM (Parallax Occlusion Mapping) Foldout
        _showPomFoldout = CoreEditorUtils.DrawHeaderFoldout(
            "Parallax Occlusion Mapping",
            _showPomFoldout,
            false,
            (Func<bool>)null,
            null
        );
        DrawProperties(
            _showPomFoldout,
            new Dictionary<string, string> {
                { "_Heightmap", "Height Map (Texture)"},
                { "_HeightAmplitude", "Amplitude" }
            },
            properties,
            materialEditor
        );
        
        // Render the Vertex Paint Foldout
        _showVertexPaintFoldout = CoreEditorUtils.DrawHeaderFoldout(
            "Vertex Paint",
            _showVertexPaintFoldout,
            false,
            (Func<bool>)null,
            null
        );
        DrawProperties(
            _showVertexPaintFoldout,
            new Dictionary<string, string> {
                { "_MaskMapColor", "Color"},
                { "_MaskMapTexture", "Mask Map (Texture)" },
                { "_MaskMapIntensity", "Intensity" },
                { "_MaskMapContrast", "Contrast" }
            },
            properties,
            materialEditor
        );
        
        // Render the Wind Sway Foldout
        _showVertexSwayFoldout = CoreEditorUtils.DrawHeaderFoldout(
            "Wind Sway",
            _showVertexSwayFoldout,
            false,
            (Func<bool>)null,
            null
        );
        DrawProperties(
            _showVertexSwayFoldout,
            new Dictionary<string, string> {
                { "_WindSway", "Enable Wind Sway"},
                { "_WindSwayIntensity", "Wind Sway Intensity" },
                { "_WindSwayFrequency", "Wind Sway Frequency" },
                { "_WindSwaySpeed", "Wind Sway Speed" }
            },
            properties,
            materialEditor
        );
        
        // Render the Post Processing foldout
        _showPostFoldout = CoreEditorUtils.DrawHeaderFoldout(
            "Post-Processing",
            _showPostFoldout,
            false,
            (Func<bool>)null,
            null
        );
        DrawProperties(
            _showPostFoldout,
            new Dictionary<string, string> {
                { "_PostTint", "Tint (Post)"},
                { "_PostBrightness", "Brightness (Post)" },
                { "_PostContrast", "Contrast (Post)" },
                { "_PostSaturation", "Saturation (Post)" },
            },
            properties,
            materialEditor
        );
    }

    /// <summary>
    ///     Draws the parameters specified in properties in the shader GUI.
    /// </summary>
    /// <param name="active">
    ///     Whether or not the foldout is currently enabled.
    /// </param>
    /// <param name="properties">
    ///     Key/Value pair of the internal shader parameter name
    ///     and the one that should be displayed in the editor.
    /// </param>
    /// <param name="matProperties">
    ///     Current GUI's MaterialProperty data structures.
    /// </param>
    /// <param name="editor">
    ///     Current GUI's MaterialEditor data structure.
    /// </param>
    private void DrawProperties(
        bool active, 
        Dictionary<string, string> properties, 
        MaterialProperty[] matProperties,
        MaterialEditor editor)
    {
        if (active)
        {
            EditorGUI.indentLevel++;
            
            foreach (string property in properties.Keys)
            {
                MaterialProperty reference = FindProperty(
                    property,
                    matProperties
                );
                editor.ShaderProperty(reference, properties[property]);
            }

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }

        CoreEditorUtils.DrawSplitter();
    }
}

#endif