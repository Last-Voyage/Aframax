using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialPopUps : MonoBehaviour
{
    /*PlayerInputMap _playerInputMap;
    InputAction _inputAction;*/

    TextAndTimerData _timerData;

    [Tooltip("The UI element that will actually display the text")]
    [SerializeField]
    TextMeshProUGUI _textContainer;

    [SerializeField]
    ScriptableUI[] _uIData;

    private int _dataPointer;

    private float _charactersPerSecond = 1.75f;

    private const string _YOUDIDIT = "Great Job!";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(_textContainer);
        //_moreUIData[0];
        // I think I want to put this in?
        StartCoroutine(TimeBeforeText());
    }


    private IEnumerator TimeBeforeText()
    {
        yield return new WaitForSeconds(_uIData[_dataPointer].GetTextAndTimer().Time);
        StartCoroutine(DisplayingTheText());
        // Add in an event listener here!
        // That would go through each of the different events!
    }

    private IEnumerator DisplayingTheText()
    {
        _textContainer.text = "";

        int counter = 0;
        while(counter < _uIData[_dataPointer].GetTextAndTimer().Text.Length)
        {
            _textContainer.text += _uIData[_dataPointer].GetTextAndTimer().Text[counter++];
            yield return new WaitForSeconds(1/_charactersPerSecond);
        }
        StartTutorialCheck();
    }


    private void StartTutorialCheck()
    {
        Debug.Log("Yeah it started, tutorial checking");
        switch(_dataPointer)
        {
            case 0:
                PlayerManager.Instance.GetMovementToggleEvent().AddListener(NextTutorial); // Dog why won't this work!
                Debug.Log("Listener checked off");
                break;
            case 1:
                PlayerManager.Instance.GetHarpoonFocusStartEvent().AddListener(NextTutorial);
                break;
            case 2:
                PlayerManager.Instance.GetHarpoonFiredStartEvent().AddListener(NextTutorial);
                break;
            case 3:
                PlayerManager.Instance.GetHarpoonFiredStartEvent().AddListener(NextTutorial);
                break;
            default:
                break;
        }
    }

    private void EndTutorialCheck()
    {
        switch (_dataPointer)
        {
            case 0:
                PlayerManager.Instance.GetMovementToggleEvent().RemoveListener(NextTutorial);
                break;
            case 1:
                PlayerManager.Instance.GetHarpoonFocusStartEvent().RemoveListener(NextTutorial);
                break;
            case 2:
                PlayerManager.Instance.GetHarpoonFiredStartEvent().RemoveListener(NextTutorial);
                break;
            case 3:
                PlayerManager.Instance.GetHarpoonFiredStartEvent().RemoveListener(NextTutorial);
                break;
            default:
                break;
        }
    }

    private void NextTutorial()
    {
        Debug.Log("tried to leave it");
        EndTutorialCheck();
        StopCoroutine(DisplayingTheText());
        _textContainer.text = _YOUDIDIT;

        _dataPointer++;
        StartCoroutine(TimeBeforeText());
    }

    /// <summary>
    /// The overloaded NextTutorial that is in place because of a bool event
    /// </summary>
    /// <param name="isIt">Is the player moving?</param>
    private void NextTutorial(bool isIt)
    {
        Debug.Log("Yeha, we tried to check it");
        if(isIt)
        {
            // I had to do this because the event fires off with a bool, and I don't want to make a new event
            // and possibly break things
            NextTutorial();
        }
    }

    /// <summary>
    /// Removes any listeners if the game stops before the tutorial stops
    /// </summary>
    private void OnDisable()
    {
        PlayerManager.Instance.GetMovementToggleEvent().RemoveListener(NextTutorial);
        PlayerManager.Instance.GetHarpoonFocusStartEvent().RemoveListener(NextTutorial);
        PlayerManager.Instance.GetHarpoonFiredStartEvent().RemoveListener(NextTutorial);
    }
}
