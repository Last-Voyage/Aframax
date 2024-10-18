/*****************************************************************************
// File Name :         PlayerHealth.cs
// Author :            Ryan Swanson
// Creation Date :     10/15/24
//
// Brief Description : Controls the player's health functionality
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : BaseHealth
{
    private void Update()
    {
        //This is for testing only.
        //If I forget to remove this after code reviews then you have my permission to be mildly annoyed
        if (Input.GetKeyDown(KeyCode.U)) HealthDecrease(2);
        if (Input.GetKeyDown(KeyCode.I)) HealthIncrease(1);
    }

    #region Base Class
    
    protected override void HealthDecrease(float damage)
    {
        base.HealthDecrease(damage);
        PlayerManager.Instance.InvokePlayerDamagedEvent(damage);
        PlayerManager.Instance.InvokePlayerHealthChangeEvent(GetHealthPercent(), _currentHealth);
    }

    protected override void HealthIncrease(float heal)
    {
        base.HealthIncrease(heal);
        PlayerManager.Instance.InvokePlayerHealEvent(heal);
        PlayerManager.Instance.InvokePlayerHealthChangeEvent(GetHealthPercent(), _currentHealth);
    }

    protected override void Death()
    {
        base.Death();
        PlayerManager.Instance.InvokeOnPlayerDeath();
    }
    
    #endregion

    /// <summary>
    /// This is TEMPORARY
    /// Waiting for Marks damage system
    /// Base function for taking damage is protected. Could use an event, but that falls out of my task scope
    /// </summary>
    public void TempEnemyDamage(float damage)
    {
        HealthDecrease(damage);
    }

    protected override void Awake()
    {
        base.Awake();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {

    }

    private void UnsubscribeToEvents()
    {

    }
}
