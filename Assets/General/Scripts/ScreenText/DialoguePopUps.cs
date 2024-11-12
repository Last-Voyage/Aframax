/*****************************************************************************
// File Name :         DialoguePopUps.cs
// Author :            Nick Rice
//                     
// Creation Date :     11/12/24
//
// Brief Description : This script handles the dialogue process, it's words and timing
*****************************************************************************/
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This class handles the dialogue process, it's words and timing
/// </summary>
public class DialoguePopUps : MonoBehaviour
{
    [Tooltip("The UI element that will actually display the text")]
    [SerializeField]
    TextMeshProUGUI _textContainer;

    // Temp variable used for testing
    [Tooltip("The data used in the UI element")]
    [SerializeField]
    ScriptableDialogueUI _uIData;

    [Tooltip("The collider the player has to walk into")]
    GameObject _walkTutorialObject;

    [Tooltip("The collider the player has to shoot")]
    GameObject _shootTutorialObject;

    [Tooltip("The pointer for which ui data is currently being used")]
    private int _dataPointer = 0;
    
    /// <summary>
    /// Temporary; used for testing
    /// </summary>
    private void Start()
    {
        GameStateManager.Instance.GetOnNewDialogueChain()?.Invoke(_uIData);
    }

    /// <summary>
    /// The pass through function for actually displaying the dialogue
    /// Because events do not like coroutines
    /// </summary>
    private void FunctionForDisplayingText(ScriptableDialogueUI dialogueUI)
    {
        StartCoroutine(DisplayingTheText(dialogueUI));
    }

    /// <summary>
    /// This takes the text, makes it invisible, then slowly makes it visible by x characters a second
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayingTheText(ScriptableDialogueUI moreDialogue)
    {
        foreach (TextAndTimerData dialogueInfo in moreDialogue.GetTextAndTimer2())
        {
            // Wait to start displaying the next text
            yield return new WaitForSeconds(moreDialogue.GetTextAndTimer2()[_dataPointer].GetTimeBeforeText);
            // Takes the display text and makes it invisible
            _textContainer.text = moreDialogue.GetTextAndTimer2()[_dataPointer].GetText;
            _textContainer.maxVisibleCharacters = 0;

            // Gets total length of text in characters, and gets the speed of the text display
            int totalLength = moreDialogue.GetTextAndTimer2()[_dataPointer].GetText.Length;
            float typeSpeed = totalLength / (float)moreDialogue.GetTextAndTimer2()[_dataPointer].GetTimeToDisplay;

            // As long as all the text hasn't been fully displayed, this will continually
            // display more characters for the total display time
            while(_textContainer.maxVisibleCharacters < totalLength)
            {
                _textContainer.maxVisibleCharacters++;
                yield return new WaitForSeconds(1f/typeSpeed);
            }
            _dataPointer++;
        }

        _dataPointer = 0;
        yield return new WaitForSeconds(2f);
        _textContainer.text = ""; 
    }

    /// <summary>
    /// Adds a listener to the event that starts the dialogue chain
    /// </summary>
    private void OnEnable()
    {
        GameStateManager.Instance.GetOnNewDialogueChain().
            AddListener(FunctionForDisplayingText);
    }

    /// <summary>
    /// Removes the listener to the event that starts the dialogue chain
    /// PREVENTS MEMORY LEAK
    /// </summary>
    private void OnDisable()
    {
        GameStateManager.Instance.GetOnNewDialogueChain().
            RemoveListener(FunctionForDisplayingText);
    }
}
