/*****************************************************************************
// File Name :         GoreSettingAssetSwapper.cs
// Author :            Jeremiah Peters
// Creation Date :     4/4/25
//
// Brief Description : reads the gore setting and swaps assets accordingly
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoreSettingAssetSwapper : MonoBehaviour
{
    [SerializeField] private Sprite GoreAsset;

    [SerializeField] private Sprite SafeAsset;

    /// <summary>
    /// /sets the object's sprite to the correct one for the setting
    /// </summary>
    private void Awake()
    {
        if (SaveManager.Instance.GetGameSaveData().IsGoreOn)
        {
            //normal

            //check if the attached object has an image component or a sprite renderer
            if (gameObject.GetComponent<Image>())
            {
                gameObject.GetComponent<Image>().sprite = GoreAsset;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = GoreAsset;
            }
        }
        else
        {
            //no gore

            //check if the attached object has an image component or a sprite renderer
            if (gameObject.GetComponent<Image>())
            {
                gameObject.GetComponent<Image>().sprite = SafeAsset;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = SafeAsset;
            }
        }
    }
}
