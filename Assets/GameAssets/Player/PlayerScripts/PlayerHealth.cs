/*****************************************************************************
// File Name :         PlayerHealth.cs
// Author :            Ryan Swanson
// Contributors:       Andrea Swihart-DeCoster, Nabil Tagba, David Henvick
// Creation Date :     10/15/24
//
// Brief Description : Controls the player's health functionality
*****************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls the player health functionality
/// </summary>
public class PlayerHealth : BaseHealth
{
    //set up for iframe coruitine. _iFrame delay will be inputable in the prefab, so you can easily test and change what feels the best in each scenario
    [SerializeField] private float _iFrameDelayInSeconds;
    private Coroutine _iFrameCoroutine;



    //Variable is used by the dev console to determine whether the player should take damage or not
    public bool _shouldTakeDamage = true;//Nabil made this change

    /// <summary>
    /// Performs the base functionality then calls player related event
    /// </summary>
    /// <param name="heal"> The amount of healing received </param>

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

            RuntimeSfxManager.APlayOneShotSfx?
                .Invoke(FmodSfxEvents.Instance.PlayerTookDamage, gameObject.transform.position);

            _iFrameCoroutine = StartCoroutine(InvincibilityFrames());
        }
    }

    /// <summary>
    /// used to grant the player temporary invincibility after taking damage
    /// </summary>
    /// <returns></returns>
    private IEnumerator InvincibilityFrames()
    {
        _shouldTakeDamage = false;
        yield return new WaitForSeconds(_iFrameDelayInSeconds);
        _shouldTakeDamage = true;
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
