using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatamoshEffectManager : MonoBehaviour
{
    public static DatamoshEffectManager Instance { get; private set; }
    
    [SerializeField] private Material _datamoshMaterial;
    public float effectLength = 1.0F;
    
    private static readonly int EnableDatamoshEffect = 
        Shader.PropertyToID("_EnableDatamoshEffect");
    
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

#if UNITY_EDITOR
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayDatamoshEffect();
        }
    }
#endif
    
    public void PlayDatamoshEffect()
    {
        StartCoroutine(DatamoshEffectBegin());
    }

    private IEnumerator DatamoshEffectBegin()
    {
        _datamoshMaterial.SetInt(EnableDatamoshEffect, 1);
        
        yield return new WaitForSeconds(effectLength);

        _datamoshMaterial.SetInt(EnableDatamoshEffect, 0);
    }
}
