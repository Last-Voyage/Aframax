/*****************************************************************************
// File Name :         PlayerHealthUI.cs
// Author :            Jeremiah Peters
// Contributors:       Ryan Swanson, Andrea Swihart-DeCoster, Nick Rice, Adam Garwacki
// Creation Date :     9/16/24
//
// Brief Description : operates the health ui for the player
*****************************************************************************/

using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
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

    [Header("Player Healed")]
    [SerializeField] private Animator _healingEffectAnimator;
    private const string _HEALING_EFFECT_TRIGGER = "PlayHealing";
    private const string _ANIM_HEALTH_STAGE = "Health_Stage";

    [Header("Heart UI fading variables")] 
    
    [SerializeField]
    private float _heartTimeToAppear;
    
    [SerializeField]
    private float _heartTimeOnScreen;
    
    [SerializeField] 
    private float _heartTimeToDisappear;

    private int _damageStatePointer;

    private IEnumerator _heartAppearanceCoroutine;

    private CanvasRenderer _heartAlphaParent;

    private Animator _heartAnimator;

    [Header("Saturation While Damaged")]
    [SerializeField] [Range(0, 1)] private float _undamagedSaturation;
    [SerializeField] [Range(0, 1)] private float _lightlyDamagedSaturation;
    [SerializeField] [Range(0, 1)] private float _kindaDamagedSaturation;
    [SerializeField] [Range(0, 1)] private float _badlyDamagedSaturation;
    [SerializeField] [Range(0, 1)] private float _onDeathSaturation;

    [Header("Health Colors")] 
    private Color _undamangedColor = Color.green;
    private Color _lightlyDamagedColor = Color.yellow;
    private Color _kindaDamagedColor = new Color(.8f,.4f,0f,1f);
    private Color _badlyDamagedColor = new Color(.8f,.1f,0f,1f);
    private Color _deathColor = Color.gray;
    
    private Color[] _damageColors;
    private float[] _damageSaturations;

    // Cached variables
    private WaitForSeconds _heartOnScreenWait;
    
    private void Awake()
    { 
        SubscribeToEvents();
    }

    /// <summary>
    /// Initializes arrays, the animator, and other variables
    /// </summary>
    private void Start()
    {
        InitializeAnimator();

        _heartAlphaParent = _playerHeart.GetComponent<CanvasRenderer>();
        _heartAlphaParent.SetAlpha(0);
        _heartOnScreenWait = new WaitForSeconds(_heartTimeOnScreen);

        _damageColors = new[] { _deathColor,_badlyDamagedColor,
            _kindaDamagedColor,_lightlyDamagedColor,_undamangedColor};
        
        _damageSaturations = new[] { _onDeathSaturation,_badlyDamagedSaturation,
            _kindaDamagedSaturation,_lightlyDamagedSaturation,_undamagedSaturation};
    }

    /// <summary>
    /// Initializes the player animator
    /// </summary>
    private void InitializeAnimator()
    {
        _heartAnimator = _playerHeart.GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    /// <summary>
    /// Updates the heart ui to match the current health
    /// Alongside controller bar color and screen saturation
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
        TurnOffDamagedUI();
        
        //this part does the blood around the edges of the screen
        switch (healthPercent)
        {
            case >=1f:
                _damageStatePointer = 4;
                break;
            case >=.75f:
                _damageStatePointer = 3;
                break;
            case >=.5f:
                _damageStatePointer = 2;
                break;
            case >=.25f:
                _damageStatePointer = 1;
                break;
            default:
                _damageStatePointer = 0;
                break;
        }
        _damagedUIImages[Mathf.Clamp(_damageStatePointer-1,0,4)].gameObject.SetActive(true);
        _heartAnimator.SetInteger(_ANIM_HEALTH_STAGE,_damageStatePointer);
        GlobalColorFilterManager.Instance.Saturation = _damageSaturations[_damageStatePointer];
        
        if (DualShockGamepad.current != null)
        {
            DualShockGamepad.current.SetLightBarColor(_damageColors[_damageStatePointer]);
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
        yield return _heartOnScreenWait;

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
    /// Plays the healing visual effect on the ui
    /// </summary>
    /// <param name="healing">The amount healed. Used because of event subscription</param>
    private void PlayHealingEffect(float healing)
    {
        _healingEffectAnimator.SetTrigger(_HEALING_EFFECT_TRIGGER);
    }

    /// <summary>
    /// Subscribes to events
    /// </summary>
    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().AddListener(UpdateHealthUI);
        PlayerManager.Instance.GetOnPlayerHealEvent().AddListener(PlayHealingEffect);
    }

    /// <summary>
    /// Removes event subscriptions
    /// </summary>
    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().RemoveListener(UpdateHealthUI);
        PlayerManager.Instance.GetOnPlayerHealEvent().RemoveListener(PlayHealingEffect);

    }
}
