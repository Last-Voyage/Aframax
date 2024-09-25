using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBuoyancyTest : MonoBehaviour
{
    [SerializeField] private Vector4 waveA;
    [SerializeField] private Vector4 waveB;
    [SerializeField] private Vector4 waveC;

    [SerializeField] private float overallHeightAdjust = 1.0F;

    [SerializeField] private float normalExaggeration = 2.0F;

    private Vector3 startPos;
    private Vector3 _tangent;
    private Vector3 _binormal;
    
    void Start()
    {
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        _tangent = new Vector3(1.0F, 0.0F, 0.0F);
        _binormal = new Vector3(0.0F, 0.0F, 1.0F);
        
        Vector3 pos = startPos;
        
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
        transform.eulerAngles = 
            Vector3.Normalize(
                Vector3.Cross(_binormal, _tangent)
        ) * normalExaggeration;
    }

    Vector3 CalculateGerstnerWave(
        Vector4 wave,
        Vector3 pos)
    {
        float steepness = wave.z;
        float wavelength = wave.w;
        float k = 2 * Mathf.PI / wavelength;
        float c = Mathf.Sqrt(9.8F / k);
        Vector2 d = Vector3.Normalize(new Vector3(wave.x, wave.y, 0.0F));
        float f = k * (Vector3.Dot(d, new Vector3(pos.x, pos.y, 0.0F)) - c * Time.time);
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
