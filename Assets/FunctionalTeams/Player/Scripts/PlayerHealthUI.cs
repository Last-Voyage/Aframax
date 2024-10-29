/*****************************************************************************
// File Name :         PlayerHealthUI.cs
// Author :            Jeremiah Peters
//                     Ryan Swanson
// Creation Date :     9/16/24
//
// Brief Description : operates the health ui for the player
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
    [Header("Heart UI")]
    [SerializeField] private GameObject _playerHeart;

    [Space]
    [Header("Player Damaged")]
    [SerializeField] private float _damageUIDisplayTime;
    [SerializeField] private Image _damagedUI;
    private Coroutine _damagedUICoroutine;

    /// <summary>
    /// locate the heart and subscribe to events
    /// </summary>
    private void Awake()
    {
        //this way it doesn't waste time doing find if it's already connected
        if (_playerHeart == null)
        {
            _playerHeart = GameObject.Find("Heart");
            if (_playerHeart == null)
            {
                Debug.Log("Couldn't find the heart object. Make sure there's one in the scene!");
            }
        }

        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    /// <summary>
    /// Updates the heart ui to match the current health
    /// </summary>
    /// <param name="healthPercent"></param>
    private void UpdateHealthUI(float healthPercent,float currentHealth)
    {
        _playerHeart.GetComponent<Animator>().SetFloat("Health_Stage_Num", 4 * healthPercent);
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
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().AddListener(UpdateHealthUI);
        PlayerManager.Instance.GetOnPlayerDamageEvent().AddListener(PlayerDamagedUI);
    }

    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().RemoveListener(UpdateHealthUI);
        PlayerManager.Instance.GetOnPlayerDamageEvent().RemoveListener(PlayerDamagedUI);
    }
}
