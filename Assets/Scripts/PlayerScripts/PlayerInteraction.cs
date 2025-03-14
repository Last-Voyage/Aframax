/**********************************************************************************************************************
// File Name :         PlayerInteraction.cs
// Author :            Alex Kalscheur
// Contributers :      Charlie Polonus
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
    private InteractableUi _interactableUi;

    //Input
    private PlayerInput _playerInput;
    private InputAction _interactInput;

    /// <summary>
    /// Right away finds the InteractableUI script
    ///     Uses FindObjectOfType as both this object and the InteractableUI object are on prefabs
    /// </summary>
    private void Awake()
    {
        _interactableUi = FindObjectOfType<InteractableUi>();
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
            _interactableUi.SetInteractUIStatus(true);
            if (_interactInput.WasPerformedThisFrame())
            {
                interactableComponent.OnInteractedByPlayer();
            }
        }
        else
        {
            _interactableUi.SetInteractUIStatus(false);
        }
    }

    /// <summary>
    /// Checks to see if the player is currently able to interact with something
    /// </summary>
    /// <returns>The object the player is currently able to interact with</returns>
    public GameObject CurrentInteractable()
    {
        SetRaycast();
        if (Physics.Raycast(_ray, out RaycastHit hit, _maxReach) &&
            hit.collider.gameObject.TryGetComponent(out IPlayerInteractable interactableComponent))
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
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
