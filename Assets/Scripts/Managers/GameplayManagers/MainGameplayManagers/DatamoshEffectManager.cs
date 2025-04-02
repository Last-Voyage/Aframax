using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatamoshEffectManager : MonoBehaviour
{
    public static DatamoshEffectManager Instance { get; private set; }
    
    [SerializeField] private Material _datamoshMaterial;
    
    private static readonly int EnableDatamoshEffect = 
        Shader.PropertyToID("_EnableDatamoshEffect");
    private static readonly int NaNPropEnabled = Shader.PropertyToID("_NaNPropEnabled");
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
        
        _datamoshMaterial.SetInt(EnableDatamoshEffect, 0);
    }
    
    public void ShowDatamoshEffectForSeconds(float duration)
    {
        StartCoroutine(DatamoshEffectBegin(duration));
    }

    private IEnumerator DatamoshEffectBegin(float duration)
    {
        _datamoshMaterial.SetInt(EnableDatamoshEffect, 1);
        
        yield return new WaitForSeconds(duration);

        _datamoshMaterial.SetInt(EnableDatamoshEffect, 0);
    }
}
