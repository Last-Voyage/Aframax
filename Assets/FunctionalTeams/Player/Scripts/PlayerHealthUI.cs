/*****************************************************************************
// File Name :         PlayerHealthUI.cs
// Author :            Jeremiah Peters
//                     Ryan Swanson
// Creation Date :     9/16/24
//
// Brief Description : operates the health bar for the player
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains the functionality for all health UI
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Image _healthBar;

    [Space]
    [Header("Player Damaged")]
    [SerializeField] private float _damageUIDisplayTime;
    [SerializeField] private Image _damagedUI;
    private Coroutine _damagedUICoroutine;


    private void Awake()
    {
        //this way it doesn't waste time doing find if it's already connected
        if (_healthBar == null)
        {
            //this used to only find object of type image but i reworked it so it should be less terrible
            //now it won't break if there's other images in the scene
            _healthBar = GameObject.Find("Health").GetComponent<Image>();
            if (_healthBar == null)
            {
                Debug.Log("Couldn't find health bar. Make sure there's one in the scene!");
            }
        }
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    /// <summary>
    /// Updates the visual health bar
    /// </summary>
    /// <param name="healthPercent"></param>
    private void UpdateHealthBar(float healthPercent,float currentHealth)
    {
        if (_healthBar != null)
        {
            _healthBar.fillAmount = healthPercent;
        }
    }

    /// <summary>
    /// Starts the process of showing the player damaged ui
    /// </summary>
    private void PlayerDamagedUI(float damage)
    {
        if(_damagedUICoroutine != null)
        {
            StopCoroutine(_damagedUICoroutine);
        }
        _damagedUICoroutine = StartCoroutine(PlayerDamagedUIProcess());
    }

    /// <summary>
    /// Displays the ui for when the player is damaged
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerDamagedUIProcess()
    {
        //I've been told to simply show it and hide it for now, might use an animation later
        _damagedUI.enabled = true;

        yield return new WaitForSeconds(_damageUIDisplayTime);

        _damagedUI.enabled = false;
    }

    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().AddListener(UpdateHealthBar);
        PlayerManager.Instance.GetOnPlayerDamageEvent().AddListener(PlayerDamagedUI);
    }

    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().RemoveListener(UpdateHealthBar);
        PlayerManager.Instance.GetOnPlayerDamageEvent().RemoveListener(PlayerDamagedUI);
    }
}
