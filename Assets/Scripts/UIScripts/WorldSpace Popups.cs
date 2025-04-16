/*****************************************************************************
// File Name :         WorldSpacePopups.cs
// Author :            Jeremiah Peters
// Contributers :      Charlie Polonus
// Creation Date :     9/28/2024
//
// Brief Description : Manages the world space pop ups for interactable objects
*****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// runs the world space pop ups for interactable objects
/// makes them look at the player and change depending on proximity
/// </summary>
public class WorldSpacePopups : MonoBehaviour, IUiSwap
{
    private Camera _playerCamera;

    private GameObject _playerReference;

    private SpriteRenderer _objectSpriteReference;

    private PlayerInteraction _playerInteractor;

    private GameObject _interactableObject;

    [SerializeField]
    private Sprite _farDistanceSprite;

    [SerializeField]
    private Sprite _closeDistanceSprite;
    
    [SerializeField] 
    private Sprite _closeDistanceControllerSpriteAsset, _closeDistanceKeyboardSpriteAsset;

    [SerializeField]
    private float _playerDetectionProximity;

    [SerializeField]
    private float _visibilityProximity;
    //should be larger than _playerDetectionProximity

    private float _playerProximity;
    
    // Cached variables
    private WaitForSeconds _findPlayerWait = new WaitForSeconds(.1f);
    private Transform _playerTransform;


    private void Awake()
    {
        _objectSpriteReference = GetComponent<SpriteRenderer>();
        StartCoroutine(FindPlayer());

        // Get the interactable object this popup is tied to
        if (!transform.GetComponentInParent<IPlayerInteractable>().IsUnityNull())
        {
            _interactableObject = transform.parent.gameObject;
        }
    }

    private void LateUpdate()
    {
        if (!_playerCamera.IsUnityNull())
        {
            //rotate to face the player camera
            transform.LookAt(_playerCamera.transform);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);
            transform.Rotate(0, 180, 0);
        }

        if (_playerReference.IsUnityNull())
        {
            return;
        }

        //check proximity to player
        _playerProximity = Vector3.Distance(_playerTransform.position, transform.position);

        // If the player is in range and is currently looking at the interactable
        if (_playerProximity < _playerDetectionProximity
            && _playerInteractor.CurrentInteractable() == _interactableObject)
        {
            _objectSpriteReference.sprite = _closeDistanceSprite;
        }
        // The player is in range to see it
        else if (_playerProximity < _visibilityProximity)
        {
            _objectSpriteReference.sprite = _farDistanceSprite;
        }
        // The player is nowhere near the interactable
        else if (!_objectSpriteReference.sprite.IsUnityNull())
        {
            _objectSpriteReference.sprite = null;
        }
    }

    /// <summary>
    /// This changes the currently used close UI based on whether the player is using controller
    /// </summary>
    public void OnUiSwap()
    {
        _closeDistanceSprite = UiManager.UsingController
            ? _closeDistanceControllerSpriteAsset
            : _closeDistanceKeyboardSpriteAsset;
    }

    /// <summary>
    /// just getting references for the player
    /// </summary>
    private IEnumerator FindPlayer()
    {
        yield return _findPlayerWait;

        //added check for PlayerFunctionality to get rid of null error
        PlayerFunctionalityCore pfc = PlayerFunctionalityCore.Instance;
        if(!pfc.IsUnityNull())
        {
            _playerCamera = Camera.main;
            _playerReference = pfc.transform.GetChild(1).gameObject;
            _playerInteractor = pfc.GetComponentInChildren<PlayerInteraction>();
            _playerTransform = _playerReference.transform;
        }

        _objectSpriteReference = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// changes whether the object's popup is enabled or not according to the taken bool
    /// </summary>
    /// <param name="doesHavePopup"></param>
    public void TogglePopUp(bool doesHavePopup)
    {
        _objectSpriteReference.enabled = doesHavePopup;
    }

    /// <summary>
    /// This adds a listener to ui swapping, and makes sure that the ui is properly swapped
    /// </summary>
    private void OnEnable()
    {
        UiManager.Instance.GetOnSwapInput.AddListener(OnUiSwap);
        OnUiSwap();
    }

    /// <summary>
    /// This removes the ui swapping listener
    /// </summary>
    private void OnDisable()
    {
        UiManager.Instance.GetOnSwapInput.RemoveListener(OnUiSwap);
    }
}
