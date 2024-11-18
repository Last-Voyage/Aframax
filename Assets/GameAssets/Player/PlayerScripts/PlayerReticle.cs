/*****************************************************************************
// File Name :         PlayerReticle.cs
// Author :            Adam Garwacki
// Contributors:       David Henvick
// Creation Date :     10/27/2024
//
// Brief Description : Allows for dynamic reticle control based on inputs.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows the on-screen reticle to change according to the player's focus.
/// </summary>
public class PlayerReticle : MonoBehaviour
{
    [Tooltip("Reticle color while unfocused. Should be less prominent than the focused color.")]
    [SerializeField] private Color _unfocusedColor;
    [Tooltip("Reticle color while focused. Should be more prominent than the unfocused color.")]
    [SerializeField] private Color _focusedColor;

    [Space]

    [Tooltip("The minimum size of the reticle scope. Higher numbers increase visibility.")]
    [SerializeField] private float _minScopeSize;

    [Tooltip("How much bigger the outer ring of the reticle is compared to the inner, dynamic one.")]
    [SerializeField] private float _scopePadding;

    [Tooltip("The circle showing the potential deviation of harpoon shots")]
    private GameObject _reticleScope;

    private HarpoonGun _harpoonGunScript;

    private RectTransform _outerRingRectTransform;
    private RectTransform _scopeRectTransform;

    private Image _frameImage;
    private Image _scopeImage;

    private float _newReticleSize = 0;
    [Tooltip("Scales reticle assets in relation to reticle inaccuracy. Adjusting this leads to inaccurate shown aim.")]
    private readonly float _maxScopeSize = 1000;
    private readonly float _scopeScalar = 1000;

    private bool _isFocusing;
    private bool _isUnfocusing;
    private bool _isFocusChanging;

    /// <summary>
    /// Initially sets the reticle to be visually unfocused.
    /// </summary>
    private void Start()
    {
        ObtainReferences();

        InitializeReticle();

        _isFocusing = false;
        _isUnfocusing = false;
    }

    /// <summary>
    /// Called by the harpoon when the state of the reticle needs to change (focusing and unfocusing)
    /// </summary>
    public void ChangeFocus()
    {
        _isFocusChanging = true;
        StartCoroutine(ChangingReticleFocus());
    }

    /// <summary>
    /// Used to update the reticle appearance and size
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangingReticleFocus()
    {
        while (_isFocusChanging)
        { 
            if (_newReticleSize < _maxScopeSize || _newReticleSize > _minScopeSize)
            {
                yield return new WaitForFixedUpdate();
                AdjustReticleSize();
                AdjustReticleAppearance();
            }
            else
            {
                _isFocusChanging = false;
            }
        }
    }

        public void ReticleFire()
    {
        //_newReticleSize = _maxScopeSize;
        //AdjustReticleAppearance();
    }

    /// <summary>
    /// Sets references to other GameObjects and components.
    /// </summary>
    private void ObtainReferences()
    {
        _harpoonGunScript = HarpoonGun.Instance;

        _reticleScope = gameObject.transform.GetChild(0).gameObject;

        _outerRingRectTransform = gameObject.GetComponent<RectTransform>();
        _scopeRectTransform = _reticleScope.GetComponent<RectTransform>();

        _frameImage = gameObject.GetComponent<Image>();
        _scopeImage = _reticleScope.GetComponent<Image>();

        gameObject.GetComponent<Image>().color = _unfocusedColor;
    }

    /// <summary>
    /// Initializes the size of the reticle assets.
    /// </summary>
    private void InitializeReticle()
    {
        _newReticleSize = _harpoonGunScript.GetFocusStartingInaccuracy() * _scopeScalar;

        // Sets the size of the frame which dynamically represents deviation range of harpoon shots
        _scopeRectTransform.sizeDelta =
            new Vector2(Mathf.Clamp(_newReticleSize, _minScopeSize, _maxScopeSize),
            Mathf.Clamp(_newReticleSize, _minScopeSize, _maxScopeSize));
        // Gives the active reticle range a static outer frame
        _outerRingRectTransform.sizeDelta =
            new Vector2(Mathf.Clamp(_newReticleSize + _scopePadding, _minScopeSize, _maxScopeSize),
            Mathf.Clamp(_newReticleSize + _scopePadding, _minScopeSize, _maxScopeSize));
    }

    /// <summary>
    /// Adjusts the size of the inner reticle in proportion to current focus.
    /// </summary>
    private void AdjustReticleSize()
    {
        _newReticleSize = _harpoonGunScript.GetCurrentFocusAccuracy() * _scopeScalar;

        _scopeRectTransform.sizeDelta =
            new Vector2(Mathf.Clamp(_newReticleSize, _minScopeSize, _maxScopeSize),
            Mathf.Clamp(_newReticleSize, _minScopeSize, _maxScopeSize));
    }

    /// <summary>
    /// Adjusts the visual prominence of the reticle depending on the status of the harpoon gun and player inputs.
    /// </summary>
    private void AdjustReticleAppearance()
    {
        // If the player is focusing the harpoon gun and not reloading, the reticle gains prominence
        if (_harpoonGunScript.GetCurrentFocusState() == HarpoonGun.EFocusState.Focusing &&
            _harpoonGunScript.GetHarpoonFiringState() != HarpoonGun.EHarpoonFiringState.Reloading)
        {
            _frameImage.color = _focusedColor;
            _scopeImage.color = _focusedColor;
        }
        // When the player is not reloading nor focusing, the reticle is less prominent
        else if (_harpoonGunScript.GetHarpoonFiringState() != HarpoonGun.EHarpoonFiringState.Reloading)
        {
            _frameImage.color = _unfocusedColor;
            _scopeImage.color = _unfocusedColor;
        }
        // When the player is reloading, the reticle disappears
        else
        {
            _frameImage.color = Color.clear;
            _scopeImage.color = Color.clear;
        }

    }
}
