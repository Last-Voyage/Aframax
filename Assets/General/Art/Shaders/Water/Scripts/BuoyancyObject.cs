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

public class BuoyancyObject : MonoBehaviour
{
    // Our wave spectra. For accurate results, these
    // should be set to the same values as the shader
    //
    // Expressed as a Vector4: (Direction, Steepness, 
    // Wavelength X, Wavelength Y)
    [SerializeField] private Vector4 waveA;
    [SerializeField] private Vector4 waveB;
    [SerializeField] private Vector4 waveC;

    // Should also match shader
    [SerializeField] private float overallHeightAdjust = 1.0F;

    // Amount to multiply the calculated normal vector
    // by for more a more pronounced effect
    [SerializeField] private float normalExaggeration = 2.0F;

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
        _tangent = new Vector3(1.0F, 0.0F, 0.0F);
        _binormal = new Vector3(0.0F, 0.0F, 1.0F);
        
        Vector3 pos = _startPos;
        
        pos += CalculateGerstnerWave(
            waveA,
            pos
        );
        pos += CalculateGerstnerWave(
            waveB,
            pos
        );
        pos += CalculateGerstnerWave(
            waveC,
            pos
        );

        transform.position = pos;
        
        // Normal vector calculation
        transform.eulerAngles = 
            Vector3.Normalize(
                Vector3.Cross(
                    _binormal, 
                    _tangent
        )) * normalExaggeration;
    }

    // Same wave equation as the shader
    // Based on: https://en.wikipedia.org/wiki/Trochoidal_wave
    Vector3 CalculateGerstnerWave(
        Vector4 wave,
        Vector3 pos)
    {
        float steepness = wave.z;
        float wavelength = wave.w;
        
        // k = Wave number + angular frequency
        float k = 2 * Mathf.PI / wavelength;
        
        // c = Phase speed (based on gravitational constant)
        float c = Mathf.Sqrt(9.8F / k);
        
        Vector2 d = Vector3.Normalize(new Vector3(
            wave.x, 
            wave.y, 
            0.0F
        ));
        
        float f = k * (Vector3.Dot(d, new Vector3(
            pos.x, 
            pos.y, 
            0.0F
        )) - c * Time.time);
        
        float a = steepness / k;
        
        _tangent += new Vector3(
            -d.x * d.x * (steepness * Mathf.Sin(f)),
            d.x * (steepness * Mathf.Cos(f)),
            -d.x * d.y * (steepness * Mathf.Sin(f))
        );
                
        _binormal += new Vector3(
            -d.x * d.y * (steepness * Mathf.Sin(f)),
            d.y * (steepness * Mathf.Cos(f)),
            -d.y * d.y * (steepness * Mathf.Sin(f))
        );
                
        return new Vector3(
            d.x * (a * Mathf.Cos(f)),
            a * Mathf.Sin(f) * overallHeightAdjust,
            d.y * (a * Mathf.Cos(f))
        );
    }
}
