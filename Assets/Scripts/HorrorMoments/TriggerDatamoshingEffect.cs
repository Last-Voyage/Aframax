using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TriggerDatamoshingEffect : MonoBehaviour
{
    private DatamoshEffectManager _datamoshEffectManager;

    private void Start()
    {
        _datamoshEffectManager = DatamoshEffectManager.Instance;
    }
    
    public void ShowDatamoshEffectForSeconds(float duration)
    {
        _datamoshEffectManager.ShowDatamoshEffectForSeconds(duration);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DatamoshEffectManager.Instance.ShowDatamoshEffectForSeconds(3.0F);
        }
    }
}
