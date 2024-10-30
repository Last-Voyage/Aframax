/*****************************************************************************
// File Name :         PlayerHealthUI.cs
// Author :            Jeremiah Peters
// Contributors:       Ryan Swanson, Andrea Swihart-DeCoster
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
    [Header("Player Damaged")]
    [SerializeField] private float _damageUIDisplayTime;
    [SerializeField] private Image _damagedUI;

    [SerializeField] private Image[] _damagedUIImages;

    private Coroutine _damagedUICoroutine;

    [Tooltip("Heart UI Object")]

    [SerializeField] private GameObject _playerHeart;

    private Animator _animator;

    /// <summary>
    /// subscribe to events
    /// </summary>
    private void Awake()
    { 
        SubscribeToEvents();
    }

    private void Start()
    {
        InitializeHeartUI();
        InitializeAnimator();
    }

    /// <summary>
    /// Initializes the heart UI
    /// </summary>
    private void InitializeHeartUI()
    {
        _playerHeart = transform.GetChild(0).gameObject;

        //this way it doesn't waste time doing find if it's already connected
        if (_playerHeart == null)
        {
            Debug.Log("Couldn't find the heart object. Make sure there's one in the scene!");
        }
    }

    /// <summary>
    /// Initializes the player animator
    /// </summary>
    private void InitializeAnimator()
    {
        _animator = _playerHeart.GetComponent<Animator>();
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
        //this updates the heart
        _animator.SetFloat("Health_Stage_Num", 4 * healthPercent);

        //this part does the blood around the edges of the screen
        switch (4 * healthPercent)
        {
            case >3:
                _damagedUI = _damagedUIImages[3];
                break;
            case >2:
                _damagedUI = _damagedUIImages[2];
                break;
            case >1:
                _damagedUI = _damagedUIImages[1];
                break;
            case <1:
                _damagedUI = _damagedUIImages[0];
                break;
            default:
                print("this shouldn't happen");
                break;
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
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().AddListener(UpdateHealthUI);
        PlayerManager.Instance.GetOnPlayerDamageEvent().AddListener(PlayerDamagedUI);
    }

    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().RemoveListener(UpdateHealthUI);
        PlayerManager.Instance.GetOnPlayerDamageEvent().RemoveListener(PlayerDamagedUI);
    }
}
