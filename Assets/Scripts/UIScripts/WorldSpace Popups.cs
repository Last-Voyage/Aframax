/*****************************************************************************
// File Name :         WorldSpacePopups.cs
// Author :            Jeremiah Peters
// Contributers :      Charlie Polonus
// Creation Date :     9/28/2024
//
// Brief Description : Manages the world space pop ups for interactable objects
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// runs the world space pop ups for interactable objects
/// makes them look at the player and change depending on proximity
/// </summary>
public class WorldSpacePopups : MonoBehaviour
{
    private Camera _playerCamera;

    private GameObject _playerReference;

    private SpriteRenderer _objectSpriteReference;

    private PlayerInteraction _playerInteractor;

    private GameObject _interactableObject;

    [SerializeField]
    private TextMeshProUGUI _popUpTextContainer;

    [Tooltip("Whether or not the player is using a controller")]
    private bool _usingController;

    [SerializeField]
    private Sprite _farDistanceSprite;

    [SerializeField]
    private Sprite _closeDistanceSprite; // Need to change the name for this
    
    [SerializeField] 
    private Sprite _closeDistanceControllerSprite;

    [SerializeField]
    private float _playerDetectionProximity;

    [SerializeField]
    private float _visibilityProximity;
    //should be larger than _playerDetectionProximity

    [SerializeField]
    private string _farText;

    [SerializeField]
    private string _closeText;

    private float _playerProximity;
    
    // Cached variables
    private WaitForSeconds _findPlayerWait = new WaitForSeconds(.1f);
    private Transform _playerTransform;


    private void Awake()
    {
        _objectSpriteReference = GetComponent<SpriteRenderer>();
        StartCoroutine(FindPlayer());

        // Get the interactable object this popup is tied to
        if (transform.GetComponentInParent<IPlayerInteractable>() != null)
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
            // This will change the sprite
            if (_usingController)
            {
                _objectSpriteReference.sprite = _closeDistanceControllerSprite;
            }
            else
            {
                _objectSpriteReference.sprite = _closeDistanceSprite;
            }
            
            _popUpTextContainer.text = _closeText;
        }
        // The player is in range to see it
        else if (_playerProximity < _visibilityProximity)
        {
            _objectSpriteReference.sprite = _farDistanceSprite;
            _popUpTextContainer.text = _farText;
        }
        // The player is nowhere near the interactable
        else
        {
            _objectSpriteReference.sprite = null;
            _popUpTextContainer.text = null;
        }
    }

    /// <summary>
    /// just getting references for the player
    /// </summary>
    /// <returns></returns>
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
}
