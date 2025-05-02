/**********************************************************************************************************************
// File Name :          DatamoshEffectManager.cs
// Author :             Miles Rogers
// Creation Date :      4/1/2025
//
// Brief description :  A global singleton attached to the Player camera to trigger the datamosh effect. This is
//                      done to ensure the coroutine is able to complete if the source object gets destroyed.
**********************************************************************************************************************/

using System.Collections;
using UnityEngine;

/// <summary>
/// A global singleton attached to the Player camera to trigger the datamosh effect. This is
/// done to ensure the coroutine is able to complete if the source object gets destroyed.
/// </summary>
public class DatamoshEffectManager : MonoBehaviour
{
    /// <summary>
    /// Global singleton instance
    /// </summary>
    public static DatamoshEffectManager Instance { get; private set; }
    
    /// <summary>
    /// Material for the fullscreen rendering effect (see URP settings)
    /// </summary>
    [SerializeField] private Material _datamoshMaterial;
    
    /// <summary>
    /// Cached value for the _EnableDatamoshEffect boolean
    /// </summary>
    private static readonly int EnableDatamoshEffect = 
        Shader.PropertyToID("_EnableDatamoshEffect");
    
    /// <summary>
    /// Set global singleton, disable datamosh effect
    /// in material by default.
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
        
        // Disable datamosh effect in material by default
        _datamoshMaterial.SetInt(EnableDatamoshEffect, 0); // 0 = false
    }
    
    /// <summary>
    /// Play the datamosh effect for a set amount of seconds
    /// </summary>
    /// <param name="duration">The length of the effect in seconds</param>
    public void PlayDatamoshEffect(float duration)
    {
        StartCoroutine(DatamoshEffectBegin(duration));
    }

    /// <summary>
    /// Coroutine for playing the datamosh effect for a set number
    /// of seconds.
    /// </summary>
    /// <param name="duration">The length of the effect in seconds</param>
    private IEnumerator DatamoshEffectBegin(float duration)
    {
        _datamoshMaterial.SetInt(EnableDatamoshEffect, 1); // 1 = true
        yield return new WaitForSeconds(duration);
        _datamoshMaterial.SetInt(EnableDatamoshEffect, 0); // 0 = false
    }
}
