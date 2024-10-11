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
using UnityEngine.Events;

/// <summary>
/// A manipulator of Universal health that allows specific damage numbers to go through
/// </summary>
public class ModularDamage : MonoBehaviour
{

    [Tooltip("Put the health script here")]
    [SerializeField] private GameObject _health;
    private UniversalHealth _healthScript;

    [Tooltip("Base damage value")]
    [SerializeField] private float _damageAmount;
    private bool _attackMode = false;

    //These are here if design ever wants critical hits in the future so right now they are unused
    //private int _criticalDamage;
    //private bool _critMode = false;

    // Start is called before the first frame update
    void Start()
    {
        _healthScript = _health.GetComponent<UniversalHealth>();
        //_healthScript.GetHealthDecrease().AddListener(TakeDamage);
        TakeDamage(_damageAmount);
    }

    void TakeDamage(float damage)
    {
        damage = _damageAmount;
        _healthScript.GetHealthDecrease()?.Invoke(damage);
        Debug.Log(_healthScript.GetHealthDecrease());
    }

    void OnTriggerEnter(Collider col)
    {
        _healthScript = col.gameObject.GetComponent<UniversalHealth>();
        if (_attackMode) 
        {
            
        }
    }

}
