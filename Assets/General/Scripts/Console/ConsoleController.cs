/*
// Name: ConsoleController.CS
// Author: Nabil Tagba
// Overview: Handles the console being turned on and off
// along with its hosting the methodes the quick action buttons
// in the console will use
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//only works in engine or development builds
#if DEVELOPMENT_BUILD || UNITY_EDITOR

/// <summary>
/// Controlls the console onn a global scope,
/// for example the console being turned on and
/// off. May contain funtions for quick action with 
/// buttons
/// </summary>
public class ConsoleController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _isInDeveloperMode = false;


    [Space]
    [Tooltip("Can be found inside of the console prefab")]
    [Header("References")]
    [SerializeField] private GameObject _content;
    [Tooltip("Can be found inside of the console prefab")]
    [SerializeField] private Button _playerTakeDamageButton;
    [SerializeField] private TMP_InputField _dmgAmountInputField;
    private PlayerInputMap _playerInput;

    [SerializeField] private GameObject _toggleGodMode;

    private void Awake()
    {
        _playerInput = new PlayerInputMap();
        _playerInput.Enable();
    }

    private void Start()
    {
        //linking player take damage button to corresponding methode
        _playerTakeDamageButton.onClick.AddListener(HurtPlayer);
        _toggleGodMode.GetComponent<Button>().onClick.AddListener(ToggleGodMode);
        _playerInput.DebugConsole.OpenCloseConsole.performed += ctx => ToggleConsole();
    }

  
    private void ToggleConsole()
    {
        //toggle console on and off
        if (_isInDeveloperMode)
        {

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
    }

    #region Damage Player
    /// <summary>
    /// deal damage to the player
    /// </summary>
    private void HurtPlayer()
    {
        float _amount;
        //making sure there is input and geting the amount
        if (_dmgAmountInputField.text.Length > 0 && float.TryParse(_dmgAmountInputField.text, out _amount))
        {
            // safty check for having the the health on the player
            try
            {
                GameObject.FindObjectOfType<PlayerHealth>().TakeDamage(_amount);
            }
            catch (NullReferenceException e)
            {
                print(e.Message);
            }
            
        }


    }
    #endregion

    #region God Mode
    private void ToggleGodMode()
    {
        //safty check to see for player health component not being on the player
        try
        {
            //toggling between god mode
            if (GameObject.FindObjectOfType<PlayerHealth>()._shouldTakeDamage == true)
            {
                EnterGodMode();
            }
            else if (GameObject.FindObjectOfType<PlayerHealth>()._shouldTakeDamage == false)
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
    /// 
    /// </summary>
    private void EnterGodMode()
    {
        GameObject.FindObjectOfType<PlayerHealth>()._shouldTakeDamage = false;
        _toggleGodMode.GetComponentInChildren<TMP_Text>().text = "Exit God Mode";
    }

    /// <summary>
    /// 
    /// </summary>
    private void ExitGodMode() 
    {
        GameObject.FindObjectOfType<PlayerHealth>()._shouldTakeDamage = true;
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





