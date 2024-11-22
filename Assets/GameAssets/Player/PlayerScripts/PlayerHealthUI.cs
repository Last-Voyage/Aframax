/*****************************************************************************
// File Name :         PlayerHealthUI.cs
// Author :            Jeremiah Peters
// Contributors:       Ryan Swanson, Andrea Swihart-DeCoster
// Creation Date :     9/16/24
//
// Brief Description : operates the health ui for the player
*****************************************************************************/

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
    /// <param name="healthPercent"> current health percentage </param>
    /// <param name="currentHealth"> current health value </param>
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

    /// <summary>
    /// Disables damaged UI
    /// </summary>
    private void TurnOffDamagedUI()
    {
        foreach (Image currentImage in _damagedUIImages)
        {
            currentImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Subscribes to events
    /// </summary>
    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().AddListener(UpdateHealthUI);
    }

    /// <summary>
    /// Removes event subscriptions
    /// </summary>
    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().RemoveListener(UpdateHealthUI);
    }
}
