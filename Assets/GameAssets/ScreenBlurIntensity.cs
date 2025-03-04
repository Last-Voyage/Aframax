/**************************************************************************
// File Name :          ScreenBlurIntensity.cs
// Author :             Caelie Joyner
// Creation Date :      3/3/2025
//
// Brief Description :  Adjusts the blur intensity of the Gaussian blur
                        shadergraph material
**************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBlurIntensity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        [SerializeField] private Material _gaussianMaterial;
        _gaussianMaterial.SetFloat("BlurIntensity", BlurIntensity);
    }
}
