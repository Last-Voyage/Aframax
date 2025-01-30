/*****************************************************************************
// File Name :         TutorialPopUps.cs
// Author :            Nick Rice
// Contributer :       Charlie Polonus
//                     
// Creation Date :     10/22/24
//
// Brief Description : This script handles the tutorial process, it's words, and objects
*****************************************************************************/
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This class handles the tutorial process, it's words, and objects
/// </summary>
public class TutorialPopUps : MonoBehaviour
{
    [Tooltip("The UI element that will actually display the text")]
    [SerializeField]
    private TextMeshProUGUI _textContainer;

    [Tooltip("The data used in the UI element")]
    [SerializeField]
    private ScriptableDialogueUI[] _uIData;

    [Tooltip("The collider the player has to walk into")]
    private GameObject _walkTutorialObject;

    [Tooltip("The collider the player has to shoot")]
    private GameObject _shootTutorialObject;

    [Tooltip("The pointer for which ui data is currently being used")]
    private int _dataPointer;

    [Tooltip("The message shown after completing a tutorial")]
    private const string _CONGRATULATION_MESSAGE = "Great Job!";

    [Tooltip("The time the final message is displayed")]
    private const float _FINAL_MESSAGE_DISPLAY_TIME = 2f;
    
    [Tooltip("The time the final message is displayed")]
    private WaitForSeconds _finalMessageWait;

    // Start's the tutorial process
    void Start()
    {
        _finalMessageWait = new WaitForSeconds(_FINAL_MESSAGE_DISPLAY_TIME);
        
        if (_uIData[0].IsContainingData())
        {
            StartCoroutine(TimeBeforeText());
        }
    }
    
    /// <summary>
    /// This coroutine will wait for x seconds, then start the displaying text coroutine
    /// </summary>
    /// <returns>The number of seconds in the UI data</returns>
    private IEnumerator TimeBeforeText()
    {
        yield return new WaitForSeconds(_uIData[_dataPointer].GetTextAndTimer()[0].GetTimeBeforeText);
        StartCoroutine(DisplayingTheText());
    }

    /// <summary>
    /// This takes the text, makes it invisible, then slowly makes it visible by x characters a second
    /// Finally starting tutorial check when done
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayingTheText()
    {
        // Takes the display text and makes it invisible
        _textContainer.text = _uIData[_dataPointer].GetTextAndTimer()[0].GetText;
        _textContainer.maxVisibleCharacters = 0;

        // Gets total length of text in characters, and gets the speed of the text display
        int totalLength = _uIData[_dataPointer].GetTextAndTimer()[0].GetText.Length;
        float typeSpeed = totalLength / (float)_uIData[_dataPointer].GetTextAndTimer()[0].GetTimeToDisplay;

        // As long as all the text hasn't been fully displayed, this will continually
        // display more characters for the total display time
        while (_textContainer.maxVisibleCharacters < totalLength)
        {
            _textContainer.maxVisibleCharacters++;
            yield return new WaitForSeconds(1f / typeSpeed);
        }
        StartTutorialCompletionCheck();
    }


    /// <summary>
    /// This will choose which tutorial check is happening based on how many tutorials have been completed
    /// </summary>
    private void StartTutorialCompletionCheck()
    {
        switch(_dataPointer)
        {
            case 0: // Sets the walk tutorial object active
                _walkTutorialObject.SetActive(true);
                break;
            case 1: // Adds the listener for harpoon focusing
                PlayerManager.Instance.GetOnHarpoonFocusStartEvent().AddListener(NextTutorial);
                break;
            case 2: // Sets the shoot tutorial object active
                _shootTutorialObject.SetActive(true);
                break;
            default: // Prevents the end message from instantly disappearing
                StartCoroutine(EndMessageBuffer());
                break;
        }
    }

    /// <summary>
    /// This ends the tutorial completion conditional check
    /// </summary>
    private void EndTutorialCheck()
    {
        switch (_dataPointer)
        {
            case 1: // Stops listening for whether or not the player has zoomed in
                PlayerManager.Instance.GetOnHarpoonFocusStartEvent().RemoveListener(NextTutorial);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Adds time before finishing the tutorial to allow the final message to be read
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndMessageBuffer()
    {
        yield return _finalMessageWait;
        NextTutorial();
    }

    /// <summary>
    /// This stops the tutorial condition checking, stops displaying text, 
    /// displays the congrats message, increases the pointer
    /// And starts the next line of text if there are enough UI data types
    /// </summary>
    private void NextTutorial()
    {
        EndTutorialCheck();
        StopCoroutine(DisplayingTheText());
        _textContainer.text = _CONGRATULATION_MESSAGE;

        _dataPointer++;
        if (_dataPointer < _uIData.Length)
        {
            StartCoroutine(TimeBeforeText());
        }
        else
        {
            GameStateManager.Instance.GetOnCompletedEntireTutorial()?.Invoke();
            _textContainer.text = "";
        }
    }

    

    /// <summary>
    /// Removes any listeners if the game stops before the tutorial stops
    /// </summary>
    private void OnDisable()
    {
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().RemoveListener(NextTutorial);
        GameStateManager.Instance.GetOnCompletedTutorialSection().RemoveListener(NextTutorial);
    }
}
