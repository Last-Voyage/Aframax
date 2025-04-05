/*****************************************************************************
// File Name :         GoreSettingAssetSwapper.cs
// Author :            Jeremiah Peters
// Creation Date :     4/4/25
//
// Brief Description : reads the gore setting and swaps assets accordingly
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// swaps image or sprite renderer assets according to selected gore setting
/// </summary>
public class GoreSettingAssetSwapper : MonoBehaviour
{
    [SerializeField] private Sprite _goreAsset;

    [SerializeField] private Sprite _safeAsset;

    [Space]
    [SerializeField] private bool _doesObjectDisableOnGoreSetting;
    [SerializeField] private bool _doesDisableOnGoreSettingOn;

    private Image _imageReference;

    /// <summary>
    /// /sets the object's sprite to the correct one for the setting
    /// </summary>
    private void Awake()
    {
        if(_doesObjectDisableOnGoreSetting && 
            _doesDisableOnGoreSettingOn == SaveManager.Instance.GetGameSaveData().IsGoreOn)
        {
            gameObject.SetActive(false);
            return;
        }

        _imageReference = gameObject.GetComponent<Image>();
        if(_imageReference.IsUnityNull())
        {
            return;
        }

        if (SaveManager.Instance.GetGameSaveData().IsGoreOn)
        {
            //normal
            //check if the attached object has an image component or a sprite renderer
            if (_imageReference)
            {
                _imageReference.sprite = _goreAsset;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = _goreAsset;
            }
        }
        else
        {
            //no gore
            //check if the attached object has an image component or a sprite renderer
            if (_imageReference)
            {
                _imageReference.sprite = _safeAsset;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = _safeAsset;
            }
        }
    }
}
