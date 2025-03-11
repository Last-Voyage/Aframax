/*****************************************************************************
// File Name :         PlayerHealthUI.cs
// Author :            Jeremiah Peters
// Contributors:       Ryan Swanson, Andrea Swihart-DeCoster, Nick Rice
// Creation Date :     9/16/24
//
// Brief Description : operates the health ui for the player
*****************************************************************************/

using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains the functionality for all health UI
/// </summary>
public class PlayerHealthUi : MonoBehaviour
{
    [Header("Player Damaged")]

    [SerializeField] private Image[] _damagedUIImages;

    [Tooltip("Heart UI Object")]

    [SerializeField] private GameObject _playerHeart;

    [Header("Heart UI fading variables")] 
    
    [SerializeField]
    private float _heartTimeToAppear;
    
    [SerializeField]
    private float _heartTimeOnScreen;
    
    [SerializeField] 
    private float _heartTimeToDisappear;

    private IEnumerator _heartAppearanceCoroutine;

    private CanvasRenderer _heartAlphaParent;

    private Animator _animator;
    
    private void Awake()
    { 
        SubscribeToEvents();
    }

    private void Start()
    {
        InitializeAnimator();

        _heartAlphaParent = _playerHeart.GetComponent<CanvasRenderer>();
        _heartAlphaParent.SetAlpha(0);
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
        if (_heartAppearanceCoroutine != null)
        {
            StopCoroutine(_heartAppearanceCoroutine);
            _heartAppearanceCoroutine = null;
        }
        
        _heartAppearanceCoroutine = HeartAppearance();
        StartCoroutine(_heartAppearanceCoroutine);

        //this part does the blood around the edges of the screen
        switch (healthPercent)
        {
            case >=1f:
                TurnOffDamagedUI();
                _animator.SetFloat("Health_Stage_Num",4);
                break;
            case >.75f:
                TurnOffDamagedUI();
                _damagedUIImages[3].gameObject.SetActive(true);
                _animator.SetFloat("Health_Stage_Num", 3);
                break;
            case >.5f:
                TurnOffDamagedUI();
                _damagedUIImages[2].gameObject.SetActive(true);
                _animator.SetFloat("Health_Stage_Num", 2);
                break;
            case >.25f:
                TurnOffDamagedUI();
                _damagedUIImages[1].gameObject.SetActive(true);
                _animator.SetFloat("Health_Stage_Num", 1);
                break;
            default:
                TurnOffDamagedUI();
                _damagedUIImages[0].gameObject.SetActive(true);
                _animator.SetFloat("Health_Stage_Num", 0);
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
    /// Causes the heart to appear and disappear over designer specified durations
    /// </summary>
    private IEnumerator HeartAppearance()
    {
        // Make sure the heart doesn't blink
        float startAlpha = _heartAlphaParent.GetAlpha();

        // Lerp to completely visible
        float appearTime = 0;
        while (appearTime < _heartTimeToAppear)
        {
            appearTime += Time.deltaTime;

            // Get the ratio of time, lerp the alpha
            float alphaValueRatio = appearTime / _heartTimeToAppear;
            _heartAlphaParent.SetAlpha(Mathf.Lerp(startAlpha, 1, alphaValueRatio));

            yield return null;
        }
        // Safeguard: Make sure that it's visible at the end
        _heartAlphaParent.SetAlpha(1);

        // Keep the heart on screen for some time
        yield return new WaitForSeconds(_heartTimeOnScreen);

        // Lerp to completely hidden
        appearTime = 0;
        while (appearTime < _heartTimeToDisappear)
        {
            appearTime += Time.deltaTime;

            // Get the ratio of time, lerp the alpha
            float alphaValueRatio = appearTime / _heartTimeToDisappear;
            _heartAlphaParent.SetAlpha(Mathf.Lerp(1, 0, alphaValueRatio));

            yield return null;
        }
        // Safeguard: Make sure that it's hidden at the end
        _heartAlphaParent.SetAlpha(0);

        _heartAppearanceCoroutine = null;
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
