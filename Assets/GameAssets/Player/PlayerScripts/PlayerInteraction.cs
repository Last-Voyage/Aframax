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
    Ray ray;

    //Input
    private PlayerInput _playerInput;
    private InputAction _interactInput;


    /// <summary>
    /// Every frame, Ray is cast.  If the ray hits something and the button is
    /// </summary>
    private void Update()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, _maxReach))
        {
            GiveInteractableFeedback(hit.collider.gameObject);
            if (_interactInput.WasPerformedThisFrame()) 

            {
                Interact(hit.collider.gameObject);
            }
        }
    }

    /// <summary>
    /// checks if gameObject can be interacted with
    ///     If so, calls GiveInteractableFeedback, which should give player feedback that the object is interactable
    /// </summary>
    /// <param name="gameObject">GameObject that can potentially be interacted with</param>
    private void GiveInteractableFeedback(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out IPlayerInteractable interactable))
        {
            //REPLACE THIS WHEN WE MAKE UI FOR INTERACTION
            Debug.Log("Press E to Interact");
        }
    }

    /// <summary>
    /// Calls the gameObject's OnINteractedByPlayer function if it is interactable
    /// </summary>
    /// <param name="gameObject">GameObject that can potentially be interacted with</param>
    private void Interact(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out IPlayerInteractable interactable))
        {
            interactable.OnInteractedByPlayer();
        }
    }

    #region INPUT
    /*private void OnEnable()
    {
        SubscribeInput();
    }

    private void OnDisable()
    {
        UnsubscribeInput();
    }*/

    public void SubscribeInput()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.currentActionMap.Enable();

        _interactInput = _playerInput.currentActionMap.FindAction("Interact");
        if (_interactInput != null)
        {
            Debug.Log("InteractInput established");
        }
        else
        {
            Debug.Log("Houston...");
        }
    }

    /// <summary>
    /// Unsubscribes from all input
    /// </summary>
    public void UnsubscribeInput()
    {
        _playerInput = null;
    }
    #endregion
}
