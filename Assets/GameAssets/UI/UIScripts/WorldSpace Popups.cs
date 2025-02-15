/*****************************************************************************
// File Name :         WorldSpacePopups.cs
// Author :            Jeremiah Peters
// Creation Date :     9/28/2024
//
// Brief Description : Manages the world space pop ups for interactable objects
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// runs the world space pop ups for interactable objects
/// makes them look at the player and change depending on proximity
/// </summary>
public class WorldSpacePopups : MonoBehaviour
{
    private Camera _playerCamera;

    private GameObject _playerReference;

    private SpriteRenderer _objectSpriteReference;

    [SerializeField]
    private TextMeshProUGUI _popUpTextContainer;

    [SerializeField]
    private Sprite _farDistanceSprite;

    [SerializeField]
    private Sprite _closeDistanceSprite;

    [SerializeField]
    private float _playerDetectionProximity;

    [SerializeField]
    private float _visibilityProximity;
    //should be larger than _playerDetectionProximity

    [SerializeField]
    private string _farText;

    [SerializeField]
    private string _closeText;


    private void Awake()
    {
        StartCoroutine(FindPlayer());
    }

    private void LateUpdate()
    {
        if (_playerCamera != null)
        {
            //rotate to face the player camera
            transform.LookAt(_playerCamera.transform);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            transform.Rotate(0, 180, 0);
        }

        if (_playerReference != null)
        {
            //check proximity to player
            if (Vector3.Distance(_playerReference.transform.position, transform.position) >= _visibilityProximity)
            {
                _objectSpriteReference.sprite = null;
                _popUpTextContainer.text = null;
            }
            else if (Vector3.Distance(_playerReference.transform.position, transform.position) >= _playerDetectionProximity)
            {
                _objectSpriteReference.sprite = _farDistanceSprite;
                _popUpTextContainer.text = _farText;
            }
            else
            {
                _objectSpriteReference.sprite = _closeDistanceSprite;
                _popUpTextContainer.text = _closeText;
            }
        }
    }

    /// <summary>
    /// just getting references for the player
    /// </summary>
    /// <returns></returns>
    private IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(0.1f);

        _playerCamera = PlayerFunctionalityCore.Instance.PlayerCamera.transform.Find("Main Camera").GetComponent<Camera>();
        _playerReference = PlayerFunctionalityCore.Instance.transform.GetChild(1).gameObject;

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
