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

    [SerializeField] private Image[] _damagedUIImages;

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
        InitializeAnimator();
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
            case 4:
                TurnOffDamagedUI();
                break;
            case >3:
                TurnOffDamagedUI();
                _damagedUIImages[3].gameObject.SetActive(true);
                break;
            case >2:
                TurnOffDamagedUI();
                _damagedUIImages[2].gameObject.SetActive(true);
                break;
            case >1:
                TurnOffDamagedUI();
                _damagedUIImages[1].gameObject.SetActive(true);
                break;
            case <1:
                TurnOffDamagedUI();
                _damagedUIImages[0].gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("this shouldn't happen");
                break;
        }
    }

    private void TurnOffDamagedUI()
    {
        foreach (Image I in _damagedUIImages)
        {
            I.gameObject.SetActive(false);
        }
    }

    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().AddListener(UpdateHealthUI);
    }

    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().RemoveListener(UpdateHealthUI);
    }
}
