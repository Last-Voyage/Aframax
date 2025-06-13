/*****************************************************************************
// File Name :         PlayerReticle.cs
// Author :            Adam Garwacki
// Contributors:       David Henvick, Ryan Swanson
// Creation Date :     10/27/2024
//
// Brief Description : Allows a reticle scope to be displayed and controlled
//                     by the player focusing the harpoon gun. Also displays
//                     ammo icons representing the player's stock and max.
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
    [Tooltip("Reticle color while unfocused. Also ammo icons' colors while the player lacks them.")]
    [SerializeField] private Color _unfocusedColor;
    [Tooltip("Reticle color while focused. Also ammo icons' colors while they are stocked.")]
    [SerializeField] private Color _focusedColor;
    [Tooltip("Ammo icon color while the player is fully out of ammo.")]
    [SerializeField] private Color _outOfAmmoIconColor;

    [Space]

    [Tooltip("The minimum size of the reticle scope. Higher numbers increase visibility.")]
    [SerializeField] private float _minScopeSize;
    [Tooltip("How much bigger the outer ring of the reticle is compared to the inner, dynamic one.")]
    [SerializeField] private float _scopePadding;

    [Space]

    [Tooltip("Parent GameObject under which ammo icons are held.")]
    [SerializeField] private GameObject _ammoIconHost;
    [Tooltip("Prefab hosting a sprite for ammo icons. Will change colors to match the scope's sprite.")]
    [SerializeField] private GameObject _harpoonIcon;

    [Space]
    [SerializeField] private GameObject _horizonDot;


    [Tooltip("The circle showing the potential deviation of harpoon shots")]
    private GameObject _reticleScope;

    private HarpoonGun _harpoonGunScript;

    private RectTransform _outerRingRectTransform;
    private RectTransform _scopeRectTransform;

    private Image _frameImage;
    private Image _scopeImage;

    private List<Image> _ammoIconList = new();

    private float _newReticleSize = 0;
    private float _gunMaxAmmo;

    [Tooltip("Scales reticle assets in relation to reticle inaccuracy. Adjusting this leads to inaccurate shown aim.")]
    private readonly float _maxScopeSize = 1000;
    private readonly float _scopeScalar = 1000;

    private bool _isFocusChanging;

    private WaitForFixedUpdate _reticlePhysicsWait = new WaitForFixedUpdate();
    
    /// <summary>
    /// Initially sets the reticle to be visually unfocused.
    /// </summary>
    private void Start()
    {
        ObtainReferences();

        InitializeReticle();

        InitializeAmmoDisplay();

        _isFocusChanging = false;
    }

    /// <summary>
    /// Called by the harpoon when the state of the reticle needs to change (focusing and unfocusing)
    /// </summary>
    public void ChangeFocus()
    {
        _isFocusChanging = true;

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ChangingReticleFocus());
        }
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
                yield return _reticlePhysicsWait;
                AdjustReticleSize();
                AdjustReticleAppearance();
            }
            else
            {
                _isFocusChanging = false;
            }
        }
    }

    /// <summary>
    /// Resets the reticle to its original size once a harpoon is fired.
    /// </summary>
    public void ReticleFire()
    {
        _newReticleSize = _maxScopeSize;
        AdjustReticleSize();
        AdjustReticleAppearance();

        // Maybe refresh the ammo icons here?
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

        _gunMaxAmmo = _harpoonGunScript.GetMaxAmmo();

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
    /// Initialize ammo icons for each individual harpoon the player can possibly hold.
    /// </summary>
    private void InitializeAmmoDisplay()
    {
        GameObject newIcon; 
        for (int i = 0; i < _gunMaxAmmo; i++)
        {
            newIcon = Instantiate(_harpoonIcon, _ammoIconHost.transform);
            newIcon.GetComponent<RectTransform>().position = newIcon.transform.position;
            newIcon.GetComponent<Image>().color = _focusedColor;
            _ammoIconList.Add(newIcon.GetComponent<Image>());

        }

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

        PlayerCameraController.Instance.IsReticleFullyZoomed = _newReticleSize <= _minScopeSize;
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

    /// <summary>
    /// Unfocuses the ammo icon for the most recent harpoon fired.
    /// </summary>
    public void UpdateAmmoDisplay()
    {
        switch (_harpoonGunScript.GetReserveAmmo())
        {
            case 0:
                OutOfAmmoIconUpdate();
                break;
            default:
                _ammoIconList[_harpoonGunScript.GetReserveAmmo()].color = _unfocusedColor;
                break;
        }

    }

    /// <summary>
    /// Updates ammo icons to a new color when the player has no ammo left.
    /// </summary>
    private void OutOfAmmoIconUpdate()
    {
        foreach(Image icon in _ammoIconList)
        {
            icon.color = _outOfAmmoIconColor;
        }
        
    }

    /// <summary>
    /// Visually shows all appropriate ammo icons as active.
    /// </summary>
    public void RestockAmmoIcons()
    {
        // As this function is called after reloading we don't need it running twice at the same time
        // Also prevents an error that can occur if reloading at the same time as restocking
        if (_harpoonGunScript.GetHarpoonFiringState() == HarpoonGun.EHarpoonFiringState.Reloading)
        {
            return;
        }

        int i;
        
        for (i = 0; i < _harpoonGunScript.GetReserveAmmo() + 1; i++)
        {
            if(i < _gunMaxAmmo)
            {
                _ammoIconList[i].color = _focusedColor;
            }
        }

        while (i < _gunMaxAmmo)
        {
            _ammoIconList[i].color = _unfocusedColor;
            i++;
        }
    }

    /// <summary>
    /// Toggles all ammo icons on or off.
    /// </summary>
    public void ToggleAmmoIcons()
    {
        foreach (Image icon in _ammoIconList)
        {
            icon.enabled = !icon.enabled;
        }
    }

    public GameObject GetHorizonDot()
    {
        return _horizonDot;
    }
}
