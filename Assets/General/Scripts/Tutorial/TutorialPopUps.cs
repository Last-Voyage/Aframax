using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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

    [Tooltip("Characters per second being shown on screen")]
    private const float _CHARACTERS_PER_SECOND = 3.5f;

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

    private IEnumerator DisplayingTheText()
    {
        _textContainer.text = "";

        int counter = 0;
        while(counter < _uIData[_dataPointer].GetTextAndTimer().Text.Length)
        {
            _textContainer.text += _uIData[_dataPointer].GetTextAndTimer().Text[counter++];
            yield return new WaitForSeconds(1f/_CHARACTERS_PER_SECOND);
        }
        StartTutorialCheck();
    }


    private void StartTutorialCheck()
    {
        Debug.Log("Yeah it started, tutorial checking");
        switch(_dataPointer)
        {
            case 0:
                // This needs to have stuff in script instead of being based off of the actual tutorial stuff
                // Starts the movement check; It would be better for the player to act in these scenarios
                // So, just have the player hit or walk into a trigger.
                Debug.Log(PlayerManager.Instance);
                Debug.Log("Listener checked off");
                _walkTutorialObject.SetActive(true);
                break;
            case 1:
                PlayerManager.Instance.GetOnHarpoonFocusStartEvent().AddListener(NextTutorial);
                break;
            case 2:
                PlayerManager.Instance.GetOnHarpoonFiredEvent().AddListener(NextTutorial);
                break;
            case 3:
                _shootTutorialObject.SetActive(true);
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

    private void NextTutorial()
    {
        Debug.Log("tried to leave it");
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
