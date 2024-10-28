/*****************************************************************************
// File Name :         PlayerReticle.cs
// Author :            Adam Garwacki
// Creation Date :     10/27/2024
//
// Brief Description : Allows for dynamic reticle control based on inputs.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// What needs to be done:
// 1. Try to add a simple blur overlay if you can
// 2. Make an event to call out reloading when that state is active
// 3. Make the reticle outer ring properly sit outside the inner one's biggest position (about 10 units)

public class PlayerReticle : MonoBehaviour
{
    // The circle which shows the spread of shots
    [SerializeField] private GameObject _reticleScope;

    [SerializeField] private HarpoonGun _harpoonGunScript;

    [Space]

    [SerializeField] private Color _unfocusedColor;
    [SerializeField] private Color _focusedColor;

    [Space]

    // The smallest size of the reticle scope on-screen.
    // Since the smallest deviation is 0, higher numbers will be increasingly inaccurate to pinpoint aim.
    [SerializeField] private float _minScopeSize;

    // How much bigger the outer ring of the reticle is compared to the inner, dynamic one.
    // It's just a frame, serves no purpose other than a visual guide as to the maximum deviancy.
    [SerializeField] private float _scopePadding;

    private float _newReticleSize = 0;
    private readonly float _maxScopeSize = 1000;

    /// <summary>
    /// Initially sets the reticle to be visually unfocused.
    /// </summary>
    private void Start()
    {
        gameObject.GetComponent<Image>().color = _unfocusedColor;

        InitializeReticle();
    }

    /// <summary>
    /// Checks for inputs related to the harpoon gun and appropriately updates UI.
    /// </summary>
    private void Update()
    {
        // Updates the sprite of the reticle if spread range is actively changing due to focus
        if (_harpoonGunScript.CurrentFocusState() == HarpoonGun.EFocusState.Focusing || 
            _harpoonGunScript.CurrentFocusState() == HarpoonGun.EFocusState.Unfocusing)
        {
            AdjustReticleSize();
        }

        AdjustReticleAppearance();

    }

    /// <summary>
    /// Initializes the size of the reticle assets.
    /// </summary>
    private void InitializeReticle()
    {
        _newReticleSize = _harpoonGunScript.FocusStartingInaccuracy() * 1000;

        // Sets the size of the frame which dynamically represents deviation range of harpoon shots
        _reticleScope.GetComponent<RectTransform>().sizeDelta =
            new Vector2(Mathf.Clamp(_newReticleSize, _minScopeSize, _maxScopeSize), 
            Mathf.Clamp(_newReticleSize, _minScopeSize, _maxScopeSize));
        // Gives the active reticle range a static outer frame
        gameObject.GetComponent<RectTransform>().sizeDelta =
            new Vector2(Mathf.Clamp(_newReticleSize + _scopePadding, _minScopeSize, _maxScopeSize), 
            Mathf.Clamp(_newReticleSize + _scopePadding, _minScopeSize, _maxScopeSize));
    }

    /// <summary>
    /// Adjusts the size of the inner reticle in proportion to current focus.
    /// </summary>
    private void AdjustReticleSize()
    {
        gameObject.GetComponent<Image>().color = _focusedColor;

        _newReticleSize = _harpoonGunScript.CurrentFocusAccuracy() * 1000;

        _reticleScope.GetComponent<RectTransform>().sizeDelta =
            new Vector2(Mathf.Clamp(_newReticleSize, _minScopeSize, _maxScopeSize), 
            Mathf.Clamp(_newReticleSize, _minScopeSize, _maxScopeSize));
    }

    /// <summary>
    /// Adjusts the visual prominence of the reticle depending on the status of the harpoon gun and player inputs.
    /// </summary>
    private void AdjustReticleAppearance()
    {
        // If the player is focusing the harpoon gun and not reloading, the reticle gains prominence
        if (_harpoonGunScript.CurrentFocusState() == HarpoonGun.EFocusState.Focusing &&
            _harpoonGunScript.HarpoonFiringState() != HarpoonGun.EHarpoonFiringState.Reloading)
        {
            gameObject.GetComponent<Image>().color = _focusedColor;
            _reticleScope.GetComponent<Image>().color = _focusedColor;
        }
        // When the player is not reloading nor focusing, the reticle is less prominent
        else if (_harpoonGunScript.HarpoonFiringState() != HarpoonGun.EHarpoonFiringState.Reloading)
        {
            gameObject.GetComponent<Image>().color = _unfocusedColor;
            _reticleScope.GetComponent<Image>().color = _unfocusedColor;
        }
        // When the player is reloading, the reticle disappears
        else
        {
            gameObject.GetComponent<Image>().color = Color.clear;
            _reticleScope.GetComponent<Image>().color = Color.clear;
        }
    }
}
