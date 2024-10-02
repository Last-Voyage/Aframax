/*****************************************************************************
// File Name :         UniversalHealth.cs
// Author :            Mark Hanson
// Creation Date :     9/27/2024
//
// Brief Description : The functational application of the health using editable serialize fields 
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UniversalHealth : UniversalHealthSystem
{
    [Header("Health Description")]
    [Tooltip("Health at the beginning of game")]
    [SerializeField] private float _maxHealth;
    [Tooltip("Determines if player or not")]
    [SerializeField] private bool _isPlayer;

    /// Events to be called outside Universal Health
    private UnityEvent<float> _healthIncrease = new();
    private UnityEvent<float> _healthDecrease = new();

    //The health assets and variables that will be used derive from here
    private Health health;
    void Start()
    {
        health = new Health(_maxHealth, _maxHealth, _isPlayer);
    }
    // Update is called once per frame
    void Update()
    {
        //When a wave clears health goes back to max
        if (health.WaveClear && health.IsPlayer)
        {
            health.CurrentHealth = health.MaxHealth;
            health.WaveClear = false;
        }
        // If the health goes all the way down to 0 the game object dies
        if (health.CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
        //If while healing the amount of health exceeds the max amount of health return it back to the max instead of leaving higher
        if (health.CurrentHealth > health.MaxHealth)
        {
            health.CurrentHealth = health.MaxHealth;
        }
    }


    /// <summary>
    /// A way for health to be damaged
    /// </summary>
    /// <param name="_damage"></param>
    private void HealthDecrease(float _damage)
    {
        health.CurrentHealth -= _damage;
    }
    /// <summary>
    /// A way for the health to be healed
    /// </summary>
    /// <param name="_heal"></param>
    private void HealthIncrease(float _heal)
    {
        _healthIncrease?.Invoke(_heal);
        health.CurrentHealth += _heal;
    }

    //#Event Getters
   // public UnityEvent<float> GetHealthIncrease() => _healthIncrease;
    //#endregion

}
