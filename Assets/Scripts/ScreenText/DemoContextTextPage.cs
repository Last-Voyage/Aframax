/*****************************************************************************
// File Name :         DemoContextTextPage.cs
// Author :            Andrew Stapay
// Creation Date :     3/27/25
//
// Brief Description : Handles and progresses through the text pages that are
                       used with the Demo Context Scene
*****************************************************************************/
using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// A struct that we will use to consolidate our pages
/// </summary>
[Serializable]
public struct TextPage
{
    // the text that we will update to when necessary
    public string TextToDisplay;

    // the amount of time that this page will be active for before moving to the next
    public float ActiveTime;
}

/// <summary>
/// Handles the text pages associated with the Demo Context Scene
/// </summary>
public class DemoContextTextPage : MonoBehaviour
{
    [SerializeField] private TextPage[] _textPages;
    private TMP_Text _currentText;

    /// <summary>
    /// Called when the associated game object is initialized
    /// Used to set up initial text
    /// </summary>
    private void Awake()
    {
        // Get the text element from the canvas
        _currentText = GetComponent<TMP_Text>();

        // Start displaying the pages
        StartCoroutine(ProgressPages());
    }

    /// <summary>
    /// Progresses through the pages set in the _textPages array
    /// </summary>
    private IEnumerator ProgressPages()
    {
        // Let's iterate through the text pages woo!
        for (int index = 0; index < _textPages.Length; index++)
        {
            // Set the text of this page
            _currentText.text = _textPages[index].TextToDisplay;

            // Wait for the active time for this page to be up
            yield return new WaitForSeconds(_textPages[index].ActiveTime);
        }
    }
}
