/**************************************************************************
// File Name :          BuoyancyObject.cs
// Author :             Miles Rogers
// Creation Date :      9/30/2024
//
// Brief Description :  There's definitely a better way to do this,
//                      but for now we're just doing the same wave
//                      calculation as we are in the shader, but
//                      only for one point (object world position).
**************************************************************************/

using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Performs the same wave calculation that happens in the shader
/// on the CPU, should not be used outside of the WaterTestGym
/// scene.
/// </summary>
public class BuoyancyObject : MonoBehaviour
{
    // Our wave spectra. For accurate results, these
    // should be set to the same values as the shader
    //
    // Expressed as a Vector4: (Direction, Steepness, 
    // Wavelength X, Wavelength Y)
    [SerializeField] private Vector4 _waveA;
    [SerializeField] private Vector4 _waveB;
    [SerializeField] private Vector4 _waveC;

    // Should also match shader
    [SerializeField] private float _overallHeightAdjust = 1.0F;

    // Amount to multiply the calculated normal vector
    // by for more a more pronounced effect
    [SerializeField] private float _normalExaggeration = 2.0F;

    // Stored starting position, so we don't apply our
    // effect additively
    private Vector3 _startPos;
    
    // Stored data needed for our normal vector
    // calculation
    private Vector3 _tangent;
    private Vector3 _binormal;
    
    void Start()
    {
        _startPos = transform.position;
    }

    private void FixedUpdate()
    {
        // Reset our tangent and binormal vectors to identity
        _tangent = new Vector3(1.0F, 0.0F, 0.0F);
        _binormal = new Vector3(0.0F, 0.0F, 1.0F);
        
        // Reset to start position so the effect is not
        // additive
        Vector3 pos = _startPos;
        
        // Sum together our waves
        pos += CalculateGerstnerWave(
            _waveA,
            pos
        );
        pos += CalculateGerstnerWave(
            _waveB,
            pos
        );
        pos += CalculateGerstnerWave(
            _waveC,
            pos
        );

        transform.position = pos;
        
        // Normal vector calculation
        transform.eulerAngles = 
            Vector3.Normalize(
                Vector3.Cross(
                    _binormal, 
                    _tangent
        )) * _normalExaggeration;
    }
    
    /// <summary>
    /// Evaluates the current Gerstner Wave displacement at a
    /// given position.
    /// 
    /// Based on: https://en.wikipedia.org/wiki/Trochoidal_wave
    /// </summary>
    /// <param name="wave">The current wave spectrum to be evaluated</param>
    /// <param name="pos">The position of the evaluation in world space.</param>
    /// <returns>Evaluated position</returns>
    Vector3 CalculateGerstnerWave(
        Vector4 wave,
        Vector3 pos)
    {
        // Gather terms from wave spectrum
        float steepness = wave.z;
        float wavelength = wave.w;
        
        // k = Wave number + angular frequency
        float k = 2 * Mathf.PI / wavelength;
        
        // c = Phase speed (based on gravitational constant)
        float c = Mathf.Sqrt(9.8F / k);
        
        // Calculate dispersion relation
        Vector2 d = Vector3.Normalize(new Vector3(
            wave.x, 
            wave.y, 
            0.0F
        ));
        
        // The evaluation of our function f
        float f = k * (Vector3.Dot(d, new Vector3(
            pos.x, 
            pos.y, 
            0.0F
        )) - c * Time.time);
        
        // a = Amplitude
        float a = steepness / k;
        
        // Calculate the tangent vector
        _tangent += new Vector3(
            -d.x * d.x * (steepness * Mathf.Sin(f)),
            d.x * (steepness * Mathf.Cos(f)),
            -d.x * d.y * (steepness * Mathf.Sin(f))
        );
        
        // Calculate the binormal vector
        _binormal += new Vector3(
            -d.x * d.y * (steepness * Mathf.Sin(f)),
            d.y * (steepness * Mathf.Cos(f)),
            -d.y * d.y * (steepness * Mathf.Sin(f))
        );
        
        // Returns the position
        return new Vector3(
            d.x * (a * Mathf.Cos(f)),
            a * Mathf.Sin(f) * _overallHeightAdjust,
            d.y * (a * Mathf.Cos(f))
        );
    }
}
