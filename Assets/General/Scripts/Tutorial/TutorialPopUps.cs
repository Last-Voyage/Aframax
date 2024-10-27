/*****************************************************************************
// File Name :         TutorialPopUps.cs
// Author :            Nick Rice
//                     
// Creation Date :     10/22/24
//
// Brief Description : This script handles the tutorial process, it's words, and objects
*****************************************************************************/
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// This script handles the tutorial process, it's words, and objects
/// </summary>
public class TutorialPopUps : MonoBehaviour
{
    [Tooltip("The UI element that will actually display the text")]
    [SerializeField]
    TextMeshProUGUI _textContainer;

    [Tooltip("The data used in the UI element")]
    [SerializeField]
    ScriptableUI[] _uIData;

    [Tooltip("The collider the player has to walk into")]
    [SerializeField]
    GameObject _walkTutorialObject;

    [Tooltip("The collider the player has to shoot")]
    [SerializeField]
    GameObject _shootTutorialObject;

    [Tooltip("The pointer for which ui data is currently being used")]
    private int _dataPointer;

    [Tooltip("The message shown after completing a tutorial")]
    private const string _CONGRATULATION_MESSAGE = "Great Job!";

    // Start's the tutorial process
    void Start()
    {
        _walkTutorialObject.SetActive(false);
        _shootTutorialObject.SetActive(false);
        StartCoroutine(TimeBeforeText());
    }


    /// <summary>
    /// This coroutine will wait for x seconds, then start the displaying text coroutine
    /// </summary>
    /// <returns>The number of seconds in the UI data</returns>
    private IEnumerator TimeBeforeText()
    {
        yield return new WaitForSeconds(_uIData[_dataPointer].GetTextAndTimer().Time);
        StartCoroutine(DisplayingTheText());
    }

    /// <summary>
    /// This takes the text, makes it invisible, then slowly makes it visible by x characters a second
    /// Finally starting tutorial check when done
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayingTheText()
    {
        _textContainer.text = _uIData[_dataPointer].GetTextAndTimer().Text;

        _textContainer.maxVisibleCharacters = 0;

        int totalLength = _uIData[_dataPointer].GetTextAndTimer().Text.Length;

        float typeSpeed = totalLength / 3;

        while(_textContainer.maxVisibleCharacters < totalLength)
        {
            _textContainer.maxVisibleCharacters++;
            yield return new WaitForSeconds(1f/typeSpeed);
        }
        StartTutorialCheck();
    }


    /// <summary>
    /// This will choose which tutorial check is happening based on how many tutorials have been completed
    /// </summary>
    private void StartTutorialCheck()
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
            case 0:
                break;
            case 1:
                PlayerManager.Instance.GetOnHarpoonFocusStartEvent().RemoveListener(NextTutorial);
                break;
            case 2:
                PlayerManager.Instance.GetOnHarpoonFiredEvent().RemoveListener(NextTutorial);
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
        yield return new WaitForSeconds(2f);
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
            _textContainer.text = "";
        }
    }

    /// <summary>
    /// This enables the event listener for getting the next tutorial
    /// </summary>
    private void OnEnable()
    {
        EnvironmentManager.Instance.CompletedTutorial().AddListener(NextTutorial);
    }

    /// <summary>
    /// Removes any listeners if the game stops before the tutorial stops
    /// </summary>
    private void OnDisable()
    {
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().RemoveListener(NextTutorial);
        EnvironmentManager.Instance.CompletedTutorial().RemoveListener(NextTutorial);
    }
}
