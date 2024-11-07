/*****************************************************************************
// File Name :         PlayerHealth.cs
// Author :            Ryan Swanson
// Contributors:       Andrea Swihart-DeCoster, Nabil Tagba
// Creation Date :     10/15/24
//
// Brief Description : Controls the player's health functionality
*****************************************************************************/

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls the player health functionality
/// </summary>
public class PlayerHealth : BaseHealth
{ 

    /// <summary>
    /// Performs the base functionality then calls player related event
    /// </summary>
    /// <param name="heal"> The amount of healing received </param>


    //Variable is used by the dev console to determine whether the player should take damage or not
    public bool _shouldTakeDamage = true;//Nabil made this change

    /// <summary>
    /// increases health
    /// </summary>
    /// <param name="heal"></param>

    public override void IncreaseHealth(float heal)
    {
        base.IncreaseHealth(heal);

        PlayerManager.Instance.InvokePlayerHealEvent(heal);
        PlayerManager.Instance.InvokePlayerHealthChangeEvent(GetHealthPercent(), _currentHealth);
    }

    /// <summary>
    /// Reduces health and calls any player damaged events
    /// </summary>
    /// <param name="damage"> amount to reduce health by </param>
    public override void TakeDamage(float damage, IBaseDamage damageSource)
    {
        if (_shouldTakeDamage)
        {
            base.TakeDamage(damage, null);

            PlayerManager.Instance.InvokePlayerDamagedEvent(damage);
            PlayerManager.Instance.InvokePlayerHealthChangeEvent(GetHealthPercent(), _currentHealth);

            RuntimeSfxManager.APlayOneShotSFX?
                .Invoke(FmodSfxEvents.Instance.PlayerTookDamage, gameObject.transform.position);
        }
    }

    /// <summary>
    /// When the player dies, performs the base functionality then calls player related event
    /// </summary>
    public override void OnDeath()
    {
        base.OnDeath();
        PlayerManager.Instance.InvokeOnPlayerDeath();
    }
}
