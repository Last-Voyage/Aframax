/*****************************************************************************
// File Name :         ModularDamage.cs
// Author :            Mark Hanson
// Creation Date :     10/1/2024
//
// Brief Description : The functational application of the damage system that is used to interact with the health system
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularDamage : MonoBehaviour
{

    [Tooltip("Put the health script here")]
    [SerializeField] private GameObject _health;
    private UniversalHealth _healthScript;

    [Tooltip("Base damage value")]
    [SerializeField] private int _damageAmount;

    private int _criticalDamage;

    private bool _attackMode = false;
    private bool _critMode = false;

    // Start is called before the first frame update
    void Start()
    {
        _healthScript = _health.GetComponent<UniversalHealth>();
    }

    void OnTriggerEnter(Collider col)
    {
        
    }

}
