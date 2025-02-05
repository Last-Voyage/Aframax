/**********************************************************************************************************************
// File Name :         PlayerInteraction.cs
// Author :            Alex Kalscheur
// Creation Date :     11/10/24
// 
// Brief Description : Controls the interaction system on the player side
**********************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

/// <summary>
/// Controls the interaction system on the player side
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Tooltip("How far can the player reach interactable objects")]
    [SerializeField] private float _maxReach;
    [Tooltip("Ray that is cast from camera to find interactable objects")]
    private Ray _ray;
    [Tooltip("UI object that will be toggled when you can or cannot interact with an object")]
    private InteractableUI _interactableUI;

    //Input
    private PlayerInput _playerInput;
    private InputAction _interactInput;

    /// <summary>
    /// Right away finds the InteractableUI script
    ///     Uses FindObjectOfType as both this object and the InteractableUI object are on prefabs
    /// </summary>
    private void Awake()
    {
        _interactableUI = FindObjectOfType<InteractableUI>();
    }

    /// <summary>
    /// Every frame, checks for interactable objects
    /// </summary>
    private void Update()
    {
        CheckForInteractable();
    }

    /// <summary>
    /// Sets the ray, then checks to see if ray intersects interactable object
    ///     If it intersects, it sets then checks if interact button was pressed
    ///         If button is pressed, it interacts with object
    ///     In either case, it makes sure UI reflects whether or not the player can interact with an object
    /// </summary>
    private void CheckForInteractable()
    {
        SetRaycast();
        if (Physics.Raycast(_ray, out RaycastHit hit, _maxReach) && 
            hit.collider.gameObject.TryGetComponent(out IPlayerInteractable interactableComponent))
        {
            _interactableUI.SetInteractUIStatus(true);
            if (_interactInput.WasPerformedThisFrame())
            {
                interactableComponent.OnSoundChange();
            }
        }
        else
        {
            _interactableUI.SetInteractUIStatus(false);
        }
    }

    /// <summary>
    /// Casts ray from center of camera.  Used to check for interactable objects
    /// </summary>
    private void SetRaycast()
    {
        _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    }

    #region INPUT
    /// <summary>
    /// Subscribes to input
    /// </summary>
    public void SubscribeInput()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.currentActionMap.Enable();

        _interactInput = _playerInput.currentActionMap.FindAction("Interact");
    }

    /// <summary>
    /// Unsubscribes from input
    /// </summary>
    public void UnsubscribeInput()
    {
        _playerInput = null;
    }
    #endregion
}
