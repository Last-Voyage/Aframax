/*****************************************************************************
// Name: ConsoleController.CS
// Author: Nabil Tagba
// Overview: Handles the console being turned on and off
// along with its hosting the methods the quick action buttons
// in the console will use
*****************************************************************************/

using Cinemachine;
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
    private bool _isInFreeLookCamMode = false;

    [Space]
    [Tooltip("Can be found inside of the console prefab")]
    [Header("References")]
    [SerializeField] private GameObject _content;
    [Tooltip("Can be found inside of the console prefab")]
    [SerializeField] private Button _playerTakeDamageButton;
    [SerializeField] private TMP_InputField _dmgAmountInputField;

    [SerializeField] private GameObject _toggleGodModeButton;

    [Header("Free look cam references")]
    [SerializeField] private GameObject _freeLookCam;
    [SerializeField] private GameObject _toggleFreeLookCamButton;
    
    private GameObject _spawnedFreeLookCam;//the free look cam that is spawned once the the player turns on free
    //look cam mode


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
        _toggleGodModeButton.GetComponent<Button>().onClick.AddListener(ToggleGodMode);
        _playerInput.DebugConsole.OpenCloseConsole.performed += ctx => ToggleConsole();

        if (_toggleFreeLookCamButton == null) return;
        //free look cam
        _toggleFreeLookCamButton.GetComponent<Button>().onClick.AddListener(ToggleFreeLookCam);
        
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

    #region FreelookCam
    /// <summary>
    /// The player unposseses the character
    /// and become a free look cam that can
    /// fly arround
    /// </summary>
    private void ToggleFreeLookCam()
    {
        if (!_isInDeveloperMode) return;

        if (!_isInFreeLookCamMode)
        {
            if (_freeLookCam == null) return;
            _spawnedFreeLookCam = EnterFreeLookCam();
            _toggleFreeLookCamButton.GetComponentInChildren<TMP_Text>().text = "Exit Free Look Cam";
        }
        else if (_isInFreeLookCamMode)
        {
            ExitFreeLookCam();
            _toggleFreeLookCamButton.GetComponentInChildren<TMP_Text>().text = "Enter Free Look Cam";
        }

    }
    /// <summary>
    /// Puts the player in free look cam
    /// mode
    /// </summary>
    private GameObject EnterFreeLookCam()
    {
        if (!_isInDeveloperMode) return null;

        CinemachineVirtualCamera _playerVirtualCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        //spawn free look cam
        GameObject _tempFreeLookCam = Instantiate(_freeLookCam,
            Camera.main.transform.position, Quaternion.identity,
            GameObject.FindObjectOfType<BoatMover>().transform);

        //stop actual player from moving
        
        //turn off the players old virtualmachine cam gameobject
        _playerVirtualCam.enabled = false;

        _isInFreeLookCamMode = true;
        return _tempFreeLookCam;
    }
    /// <summary>
    /// takes the player out of free look cam mode
    /// </summary>
    private void ExitFreeLookCam()
    {
        if (!_isInDeveloperMode) return;

        //turn on the player virtual machine cam
        GameObject.FindObjectOfType<PlayerCamScriptTag>().gameObject.
            GetComponent<CinemachineVirtualCamera>().enabled = true;

        //allow the real player to move

        //Destroy free look cam
        Destroy(_spawnedFreeLookCam);
        

        _isInFreeLookCamMode = false;

    }

    #endregion

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
        _toggleGodModeButton.GetComponentInChildren<TMP_Text>().text = "Exit God Mode";
    }

    /// <summary>
    /// Takes the player out of god mode
    /// </summary>
    private void ExitGodMode() 
    {
        FindObjectOfType<PlayerHealth>()._shouldTakeDamage = true;
        _toggleGodModeButton.GetComponentInChildren<TMP_Text>().text = "Enter God Mode";
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
        _toggleGodModeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _playerInput.Disable();
        if (_toggleFreeLookCamButton == null) return;
        _toggleFreeLookCamButton.GetComponent<Button>().onClick.RemoveAllListeners();
        
    }
}

#endif