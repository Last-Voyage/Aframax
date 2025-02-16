/*****************************************************************************
// File Name :         PlayerHealth.cs
// Author :            Ryan Swanson
// Contributors:       Andrea Swihart-DeCoster, Nabil Tagba, David Henvick, Andrew Stapay
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
    //Set up for iframe coroutine. _iFrame delay will be inputable in the prefab,
    //so you can easily test and change what feels the best in each scenario
    [SerializeField] private float _iFrameDelayInSeconds;
    [SerializeField] private float _damageToTakeFromEnemy =25f;

    [HideInInspector]
    //Variable is used by the dev console to determine whether the player should take damage or not
    public bool CanPlayerTakeDamage { get; set; }

    [Tooltip ("Health point at which the heart beat sfx starts")]
    [SerializeField] private float _healthToStartHeartSfx;
    [Tooltip ("Health point at which the heart beat sfx ends")]
    [SerializeField] private float _healthToEndHeartSfx;
    [SerializeField] private float _heartBeatRateSfx;
    private Coroutine _heartBeatCoroutine;
    
    protected override void Awake()
    {
        base.Awake();
        SubscribeToEvents();
        CanPlayerTakeDamage = true;
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    /// <summary>
    /// simple collision to take damage when tag is enemy without using 2 or 3 other scripts
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Enemy"))
        {
            TakeDamage(_damageToTakeFromEnemy, null);
        }
    }

    /// <summary>
    /// Collision for enemy contact
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(_damageToTakeFromEnemy, null);
        }
    }

    /// <summary>
    /// Performs the base functionality then calls player related event
    /// </summary>
    /// <param name="heal"> The amount of healing received </param>
    public override void IncreaseHealth(float heal)
    {
        base.IncreaseHealth(heal);

        PlayerManager.Instance.OnInvokePlayerHealthChangeEvent(GetHealthPercent(), _currentHealth);
        PlayHeartBeatSfx();
    }
    
    /// <summary>
    /// The initiator of heart beat coroutines (should only start up and stop once per calll at max)
    /// </summary>
    public void PlayHeartBeatSfx()
    {
        if (_currentHealth <= _healthToStartHeartSfx && _currentHealth != _healthToEndHeartSfx && _heartBeatCoroutine == null)
        {
            _heartBeatCoroutine = StartCoroutine(HeartbeatLoop());
        }
        if (_currentHealth >= _healthToEndHeartSfx && _heartBeatCoroutine != null)
        {
          StopCoroutine(_heartBeatCoroutine);
        }
    }

    /// <summary>
    /// A coroutine that is mean't to loop the heartbeat sfx
    /// </summary>
    /// <returns>returns null to loop similar to update function</returns>
    private IEnumerator HeartbeatLoop()
    {
        while (true)
        {
                RuntimeSfxManager.APlayOneShotSfx?.Invoke(FmodSfxEvents.Instance.PlayerHeartBeat,
                    gameObject.transform.position);
                yield return new WaitForSeconds(_heartBeatRateSfx);
        }
    }

    /// <summary>
    /// Reduces health and calls any player damaged events
    /// </summary>
    /// <param name="damage"> amount to reduce health by </param>
    public override void TakeDamage(float damage, IBaseDamage damageSource)
    {
        if (CanPlayerTakeDamage)
        {
            base.TakeDamage(damage, null);

            PlayerManager.Instance.OnInvokePlayerDamagedEvent(damage);
            PlayerManager.Instance.OnInvokePlayerHealthChangeEvent(GetHealthPercent(), _currentHealth);

            RuntimeSfxManager.APlayOneShotSfx?
                .Invoke(FmodSfxEvents.Instance.PlayerTookDamage, gameObject.transform.position);
            PlayHeartBeatSfx();
            StartIFrames();
        }
    }

    /// <summary>
    /// used to start the player's invincibility after taking damage
    /// </summary>
    /// <returns></returns>
    private void StartIFrames()
    {
        CanPlayerTakeDamage = false;
        PrimeTween.Tween.Delay(this, _iFrameDelayInSeconds, EndIFrames);
    }

    /// <summary>
    /// used to end the player's invincibility after taking damage
    /// </summary>
    private void EndIFrames()
    {
        CanPlayerTakeDamage = true;
    }

    /// <summary>
    /// When the player dies, performs the base functionality then calls player related event
    /// </summary>
    public override void OnDeath()
    {
        base.OnDeath();
        PlayerManager.Instance.OnInvokePlayerDeath();
    }

    /// <summary>
    /// Subscribes the chosen functions to the unity events
    /// </summary>
    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealEvent().AddListener(IncreaseHealth);
    }

    /// <summary>
    /// Unsubscribes the chosen functions to the unity events
    /// </summary>
    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealEvent().RemoveListener(IncreaseHealth);
    }
}
