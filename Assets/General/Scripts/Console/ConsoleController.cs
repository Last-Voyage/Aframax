/*****************************************************************************
// Name: ConsoleController.CS
// Author: Nabil Tagba
// Overview: Handles the console being turned on and off
// along with its hosting the methods the quick action buttons
// in the console will use
*****************************************************************************/

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//only works in engine or development builds
#if DEVELOPMENT_BUILD || UNITY_EDITOR

/// <summary>
/// Controls the console onn a global scope,
/// for example the console being turned on and
/// off. May contain functions for quick action with 
/// buttons
/// </summary>
public class ConsoleController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _isInDeveloperMode;

    [Space]
    [Tooltip("Can be found inside of the console prefab")]
    [Header("References")]
    [SerializeField] private GameObject _content;
    [Tooltip("Can be found inside of the console prefab")]
    [SerializeField] private Button _playerTakeDamageButton;
    [SerializeField] private TMP_InputField _dmgAmountInputField;

    [SerializeField] private GameObject _toggleGodMode;

    private PlayerInputMap _playerInput;
    
    /// <summary>
    /// happens before the start of the game
    /// </summary>
    private void Awake()
    {
        _playerInput = new PlayerInputMap();
        _playerInput.Enable();
    }

    /// <summary>
    /// happens when the game starts
    /// </summary>
    private void Start()
    {
        //linking player take damage button to corresponding methode
        _playerTakeDamageButton.onClick.AddListener(HurtPlayer);
        _toggleGodMode.GetComponent<Button>().onClick.AddListener(ToggleGodMode);
        _playerInput.DebugConsole.OpenCloseConsole.performed += ctx => ToggleConsole();
    }

  /// <summary>
  /// toggles the console on and off
  /// </summary>
    private void ToggleConsole()
    {
        //toggle console on and off
        if (!_isInDeveloperMode) return;
        
        if (_content.activeSelf)
        {
            _content.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
        else
        {
            _content.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
    }

    #region Damage Player
    
    /// <summary>
    /// deal damage to the player
    /// </summary>
    private void HurtPlayer()
    {
        float amount;
        //making sure there is input and getting the amount
        if (_dmgAmountInputField.text.Length > 0 && float.TryParse(_dmgAmountInputField.text, out amount))
        {
            // safety check for having the health on the player
            try
            {
                FindObjectOfType<PlayerHealth>().TakeDamage(amount, null);
            }
            catch (NullReferenceException e)
            {
                print(e.Message);
            }
        }
    }
    #endregion

    #region God Mode

    /// <summary>
    /// toggles between going in and out of god mode
    /// </summary>
    private void ToggleGodMode()
    {
        //safety check to see for player health component not being on the player
        try
        {
            //toggling between god mode
            if (FindObjectOfType<PlayerHealth>()._shouldTakeDamage)
            {
                EnterGodMode();
            }
            else if (!FindObjectOfType<PlayerHealth>()._shouldTakeDamage)
            {
                ExitGodMode();
            }
        }
        catch (NullReferenceException e)
        {
            print(e.Message);
        }
    }

    /// <summary>
    /// puts the player in god mode
    /// </summary>
    private void EnterGodMode()
    {
        FindObjectOfType<PlayerHealth>()._shouldTakeDamage = false;
        _toggleGodMode.GetComponentInChildren<TMP_Text>().text = "Exit God Mode";
    }

    /// <summary>
    /// Takes the player out of god mode
    /// </summary>
    private void ExitGodMode() 
    {
        FindObjectOfType<PlayerHealth>()._shouldTakeDamage = true;
        _toggleGodMode.GetComponentInChildren<TMP_Text>().text = "Enter God Mode";
    }
    
    #endregion

    /// <summary>
    /// called when the object is destroyed.
    /// removes all listener to lower chances
    /// of memory leaks
    /// </summary>
    private void OnDestroy()
    {
        Time.timeScale = 1;
        _playerTakeDamageButton.onClick.RemoveAllListeners();
        _toggleGodMode.GetComponent<Button>().onClick.RemoveAllListeners();
        _playerInput.Disable();
    }
}

#endif