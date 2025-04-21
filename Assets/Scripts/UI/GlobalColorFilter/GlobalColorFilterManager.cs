/**********************************************************************************************************************
// File Name :          GlobalColorFilterManager.cs
// Author :             Miles Rogers
// Creation Date :      4/15/2025
//
// Brief description :  A global singleton to manage scene saturation and color blindness filters
**********************************************************************************************************************/

using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A global singleton to manage scene saturation and color blindness filters
/// </summary>
[ExecuteInEditMode]
public class GlobalColorFilterManager : MonoBehaviour
{
    /// <summary>
    /// List of available color blindness modes, mapped to ColorblindnessFiltersScriptableObject
    /// </summary>
    public enum ColorBlindnessMode
    {
        None = 0,
        Protanopia = 1,
        Protanomaly = 2,
        Deuteranopia = 3,
        Deuteranomaly = 4,
        Tritanopia = 5,
        Tritanomaly = 6,
        Achromatopsia = 7,
        Achromatomaly = 8
    }
    
    /// <summary>
    /// Global static instance
    /// </summary>
    public static GlobalColorFilterManager Instance { get; private set; }
    
    /// <summary>
    /// Data for the colorblindness filter
    /// </summary>
    [SerializeField] private ColorblindnessFiltersScriptableObject
        _colorblindnessFiltersScriptableObject;
    
    /// <summary>
    /// Fullscreen shader material for the color manager
    /// </summary>
    [SerializeField] private Material _colorManagerMaterial;

    /// <summary>
    /// Saturation value to apply to the scene
    /// </summary>
    [Range(0.0F, 1.5F)] public float Saturation = 1.0F;
    
    /// <summary>
    /// Last value when saturation was changed (used to detect changes in the value)
    /// </summary>
    private float _lastSaturation;

    /// <summary>
    /// Cached shader value for red channel influence
    /// </summary>
    private static readonly int _RED_MIXING = 
        Shader.PropertyToID("_RedMixing");
    
    /// <summary>
    /// Cached shader value for green channel influence
    /// </summary>
    private static readonly int _GREEN_MIXING = 
        Shader.PropertyToID("_GreenMixing");
    
    /// <summary>
    /// Cached shader value for blue channel influence
    /// </summary>
    private static readonly int _BLUE_MIXING = 
        Shader.PropertyToID("_BlueMixing");

    /// <summary>
    /// Cached shader value for global saturation
    /// </summary>
    private static readonly int _SATURATION = 
        Shader.PropertyToID("_Saturation");

    /// <summary>
    /// Register global instance, set default values
    /// </summary>
    private void Awake()
    {
        // Global static registry
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _lastSaturation = Saturation;
        
        SetSaturation();
    }

    /// <summary>
    /// Change/set the global saturation when the saturation value changes
    /// </summary>
    private void Update()
    {
        if (!Mathf.Approximately(_lastSaturation, Saturation))
        {
            _lastSaturation = Saturation;
            SetSaturation();
        }
    }

    /// <summary>
    /// Set global saturation in material
    /// </summary>
    private void SetSaturation()
    {
        _colorManagerMaterial.SetFloat(_SATURATION, Saturation);
    }

    /// <summary>
    /// Reset all parameters on disable
    /// </summary>
    private void OnDestroy()
    {
        SetColorblindnessFilterPrivate(ColorBlindnessMode.None);
        Saturation = 1.0F;
        SetSaturation();
    }

    /// <summary>
    /// Set global colorblindness filter
    /// </summary>
    /// <param name="mode">The type of colorblindness to simulate</param>
    public static void SetColorblindnessFilter(ColorBlindnessMode mode)
    {
        Instance.SetColorblindnessFilterPrivate(mode);
    }

    /// <summary>
    /// Set global colorblindness filter (private implementation)
    /// </summary>
    /// <param name="mode">The type of colorblindness to simulate</param>
    private void SetColorblindnessFilterPrivate(ColorBlindnessMode mode)
    {
        ColorblindnessFiltersScriptableObject.ColorProfile profile;

        switch (mode)
        {
            case ColorBlindnessMode.None:
                profile = _colorblindnessFiltersScriptableObject.None;
                break;
            case ColorBlindnessMode.Protanopia:
                profile = _colorblindnessFiltersScriptableObject.Protanopia;
                break;
            case ColorBlindnessMode.Protanomaly:
                profile = _colorblindnessFiltersScriptableObject.Protanomaly;
                break;
            case ColorBlindnessMode.Deuteranopia:
                profile = _colorblindnessFiltersScriptableObject.Deuteranopia;
                break;
            case ColorBlindnessMode.Deuteranomaly:
                profile = _colorblindnessFiltersScriptableObject.Deuteranomaly;
                break;
            case ColorBlindnessMode.Tritanopia:
                profile = _colorblindnessFiltersScriptableObject.Tritanopia;
                break;
            case ColorBlindnessMode.Tritanomaly:
                profile = _colorblindnessFiltersScriptableObject.Tritanomaly;
                break;
            case ColorBlindnessMode.Achromatopsia:
                profile = _colorblindnessFiltersScriptableObject.Achromatopsia;
                break;
            case ColorBlindnessMode.Achromatomaly:
                profile = _colorblindnessFiltersScriptableObject.Achromatomaly;
                break;
            default:
                profile = _colorblindnessFiltersScriptableObject.None;
                break;
        }
        
        _colorManagerMaterial.SetVector(
            _RED_MIXING,
            new Vector4(
                profile.redChannel.r, 
                profile.redChannel.g, 
                profile.redChannel.b, 
                1.0F
        ));
        _colorManagerMaterial.SetVector(
            _BLUE_MIXING,
            new Vector4(
                profile.blueChannel.r, 
                profile.blueChannel.g, 
                profile.blueChannel.b, 
                1.0F
        ));
        _colorManagerMaterial.SetVector(
            _GREEN_MIXING,
            new Vector4(
                profile.greenChannel.r, 
                profile.greenChannel.g, 
                profile.greenChannel.b, 
                1.0F
        ));
    }
}
