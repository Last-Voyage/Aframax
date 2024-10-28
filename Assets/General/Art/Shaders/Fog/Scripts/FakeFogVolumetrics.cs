/*****************************************************************************
// File Name :         FakeFogVolumetrics.cs
// Author :            Miles Rogers
// Creation Date :     10/23/2024
//
// Brief Description : Sets the properties of the fake volumetric material
//                     component of the fog system. It will automatically
//                     retrieve properties of the point light when it is
//                     updates both in-game and in-editor.
*****************************************************************************/

using UnityEngine;

/// <summary>
/// Looks for a PointLight component in the parent
/// object and sets the fake volumetric fog contribution
/// accordingly.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class FakeFogVolumetrics : MonoBehaviour
{
    /// <summary>
    /// How much intensity to add to the volumetric effect
    /// in addition to the point light intensity
    /// </summary>
    public float intensityMultiplier = 1.0F;
    
    private Material _lightMaterial;
    private Light _pointLight;

    private void Start()
    {
        _lightMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        _pointLight = transform.parent.GetComponent<Light>();
    }

    private float _lastIntensity = 0.0F;
    private Color _lastColor = Color.black;
    private float _lastRange = 0.0F;

    private void Update()
    {
        // If light intensity changes, update material
        if (!Mathf.Approximately(
            _pointLight.intensity, 
            _lastIntensity))
        {
            _lastIntensity = _pointLight.intensity;
            _lightMaterial.SetFloat(
                "_Intensity", 
                _lastIntensity * intensityMultiplier
            );
        }

        // If light color changes, update material
        if (_pointLight.color != _lastColor)
        {
            _lastColor = _pointLight.color;
            _lightMaterial.SetColor(
                "_LightColor",
                _lastColor
            );
        }

        // If light range changes, update scale
        if (!Mathf.Approximately(
            _pointLight.range, 
            transform.localScale.x))
        {
            _lastRange = _pointLight.range;
            transform.localScale = new Vector3(
                _lastRange,
                _lastRange,
                _lastRange
            );
        }
    }
}
