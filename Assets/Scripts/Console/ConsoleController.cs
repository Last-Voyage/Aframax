/*****************************************************************************
// Name: ConsoleController.CS
// Author: Nabil Tagba
// Contributers: Charlie Polonus
// Creation Date : UNKNOWN
// Overview: Handles the console being turned on and off
// along with its hosting the methods the quick action buttons
// in the console will use
*****************************************************************************/

using Cinemachine;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



/// <summary>
/// Controls the console on a global scope,
/// for example the console being turned on and
/// off. May contain functions for quick action with 
/// buttons
/// </summary>
public class ConsoleController : MonoBehaviour
{
    #region variables
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

    [Header("Infinite Focus References")]
    [SerializeField] private GameObject _infiniteFocusToggleButton;

    //false means not focus max and true is in max focus
    private bool _isInFocusState = true;
    public bool IsInInfiniteFocusMode = false;
    private HarpoonGun _harpoonGun; //reference to harpoon gun

    private GameObject _spawnedFreeLookCam;//the free look cam that is spawned once the the player turns on free
    //look cam mode

    //infinite ammo mode settings
    //
    [Header("infinite ammo Mode")]
    [SerializeField] private Button _infiniteAmmoModeButton;
    [SerializeField] private TMP_Text _infiniteAmmoFeedbackText;
    public bool IsInInfiniteAmmoMode = false;

    //scene skiping
    [Header("scene skiping")]
    [SerializeField] private TMP_InputField _sceneIndex;
    [SerializeField] private Button _moveToSceneButton;
    [SerializeField] private Button _moveSceneForwardButton;
    [SerializeField] private Button _moveSceneBackwardButton;

    //Dev ui mode
    [Header("Dev ui Mode")]
    [SerializeField] private Button _devUiModeButton;
    [SerializeField] private TMP_Text _devUiModeText;
    [SerializeField] private GameObject _devUi;
    public bool IsInDevUiMode;


    //trailer mode
    [Header("Trailer Mode")]
    [SerializeField] private Button _toggleTrailerModeButton;
    [SerializeField] private TMP_Text _trailerModeButtonText;
    private GameObject _playerHud;
    private bool _isInTrailerMode = false;

    private PlayerInputMap _playerInput;

    public static ConsoleController Instance;
    #endregion
    /// <summary>
    /// happens before the start of the game
    /// </summary>
    private void Awake()
    {
        _playerInput = new PlayerInputMap();
        _playerInput.Enable();
        Instance = this;
    }

    /// <summary>
    /// A public getter for whether the console is currently active
    /// </summary>
    /// <returns>If the console is currently open in the scene</returns>
    public bool ConsoleIsOpen()
    {
        return _content.activeSelf;
    }

    //only works in engine or development builds
#if DEVELOPMENT_BUILD || UNITY_EDITOR

    /// <summary>
    /// happens when the game starts
    /// </summary>
    private void Start()
    {
        DevConsoleInit();
    }

    #region dev console init
    private void DevConsoleInit()
    {
        //linking player take damage button to corresponding method
        _playerTakeDamageButton.onClick.AddListener(HurtPlayer);
        _toggleGodModeButton.GetComponent<Button>().onClick.AddListener(ToggleGodMode);
        _playerInput.DebugConsole.OpenCloseConsole.performed += ctx => ToggleConsole();
        _infiniteFocusToggleButton.GetComponent<Button>().onClick.AddListener(ToggleInfiniteFocus);
        _harpoonGun = GameObject.FindObjectOfType<HarpoonGun>();
        _infiniteAmmoModeButton.onClick.AddListener(ToggleInfiniteAmmo);
        _moveSceneBackwardButton.onClick.AddListener(Back);
        _moveSceneForwardButton.onClick.AddListener(Forward);
        _moveToSceneButton.onClick.AddListener(MoveToScene);
        _devUiModeButton.onClick.AddListener(ToggleDevUiMode);
        _toggleTrailerModeButton.onClick.AddListener(ToggleTrailerMode);
        _playerHud = GameObject.FindGameObjectWithTag("PlayerHud");

        if (_toggleFreeLookCamButton == null) return;
        //free look cam
        _toggleFreeLookCamButton.GetComponent<Button>().onClick.AddListener(ToggleFreeLookCam);
    }
    #endregion
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

            // If there's currently a note/tutorial open, don't free the mouse yet
            if (NoteInteractable.ActiveNote != null
                || TutorialPopUp.ActiveTutorial != null)
            {
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            _harpoonGun.SubscribeInput();
        }
        else
        {
            _content.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            _harpoonGun.UnsubscribeInput();
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


        GameObject _tempFreeLookCam;

        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            //spawn free look cam 
            _tempFreeLookCam = Instantiate(_freeLookCam,
                Camera.main.transform.position, Quaternion.identity);
        }
        else 
        {
            //spawn free look cam inside of the boat
            _tempFreeLookCam = Instantiate(_freeLookCam,
                Camera.main.transform.position, Quaternion.identity,
                GameObject.FindObjectOfType<BoatMover>().transform);
        }

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
            if (FindObjectOfType<PlayerHealth>().CanPlayerTakeDamage)
            {
                EnterGodMode();
            }
            else if (!FindObjectOfType<PlayerHealth>().CanPlayerTakeDamage)
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
        FindObjectOfType<PlayerHealth>().CanPlayerTakeDamage = false;
        _toggleGodModeButton.GetComponentInChildren<TMP_Text>().text = "Exit God Mode";

        
    }

    /// <summary>
    /// Takes the player out of god mode
    /// </summary>
    private void ExitGodMode() 
    {
        FindObjectOfType<PlayerHealth>().CanPlayerTakeDamage = true;
        _toggleGodModeButton.GetComponentInChildren<TMP_Text>().text = "Enter God Mode";

        
    }

    #endregion


    #region Infinite Focus

    /// <summary>
    /// puts you in and out of infinite focus mode
    /// </summary>
    private void ToggleInfiniteFocus()
    {

        if (_isInFocusState)
        {
            ExitInfiniteFocus();
            _isInFocusState = false;

        }
        else
        {
            EnterInfiniteFocus();
            _isInFocusState = true;

        }

    }

    /// <summary>
    /// Puts you into infinite focus mode
    /// </summary>
    private void EnterInfiniteFocus()
    {
       IsInInfiniteFocusMode = true;
        _harpoonGun.DebugEnterMaxFocus();
        _infiniteFocusToggleButton.GetComponentInChildren<TMP_Text>().text = "Exit Infinite Focus";
        
        PlayerManager.Instance.GetOnHarpoonFiredEvent().AddListener(_harpoonGun.DebugEnterMaxFocus);
    }

    /// <summary>
    /// takes you out of infinite focus mode
    /// </summary>
    private void ExitInfiniteFocus()
    {
        IsInInfiniteFocusMode = false;
        _harpoonGun.DebugResetFocus();
        _infiniteFocusToggleButton.GetComponentInChildren<TMP_Text>().text = "Enter Infinite Focus";

        PlayerManager.Instance.GetOnHarpoonFiredEvent().RemoveListener(_harpoonGun.DebugEnterMaxFocus);
    }


    #endregion

    #region infinite ammo
    /// <summary>
    /// toggles in and out of infinite ammo
    /// </summary>
    private void ToggleInfiniteAmmo()
    {
        if (IsInInfiniteAmmoMode)
        {
            IsInInfiniteAmmoMode = false;
            ExitInfiniteAmmoMode();
        }
        else 
        {
            IsInInfiniteAmmoMode = true;
            EnterInfiniteAmmoMode();
        }
    }

    /// <summary>
    /// puts the player in infinite ammo mode
    /// </summary>
    private void EnterInfiniteAmmoMode()
    {
        _harpoonGun.EditReserveAmmoToEnterInfiniteAmmoMode();
        _infiniteAmmoFeedbackText.text = "Exit Infinite Ammo";
    }
    /// <summary>
    /// takes the playrer out of infinite ammo mode
    /// </summary>
    private void ExitInfiniteAmmoMode()
    {
        _infiniteAmmoFeedbackText.text = "Enter Infinite Ammo";
    }

    #endregion

    #region Scene Skiping

    /// <summary>
    /// moves the player forward a scene
    /// </summary>
    private void Forward()
    {
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings -1)
        {
            AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(0,0);
        }
        else
        {
            AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(SceneManager.GetActiveScene().buildIndex + 1, 0);
        }
        
    }
    /// <summary>
    /// moves the player backwards a scene
    /// </summary>
    private void Back()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(SceneManager.sceneCountInBuildSettings - 1, 0);
        }
        else
        {
            AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(SceneManager.GetActiveScene().buildIndex -1,0);
        }
    }

    /// <summary>
    /// moves the player to a specific scene
    /// </summary>
    private void MoveToScene()
    {
        string indexAsString = _sceneIndex.text;
        int index = 0;
        //if its a number
        if (int.TryParse(indexAsString, out index))
        {
            if (index >= 0 && index <= SceneManager.sceneCountInBuildSettings - 1)
            {
                //load the scene
                AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(index, 0);
            }
        }
    }

    #endregion

    #region Dev Ui Mode

    /// <summary>
    /// Turns dev ui on and off
    /// </summary>
    private void ToggleDevUiMode()
    {
        if (IsInDevUiMode)
        {
            IsInDevUiMode = false;
            _devUi.SetActive(IsInDevUiMode);
            _devUiModeText.text = "Enter Dev Ui Mode";
        }
        else
        {
            IsInDevUiMode = true;
            _devUi.SetActive(IsInDevUiMode);
            _devUiModeText.text = "Exit Dev Ui Mode";
        }
    }


    #endregion


    #region trailer Mode
    /// <summary>
    /// turns trailer mode on and off,
    /// trailer mode means no ui
    /// </summary>
    private void ToggleTrailerMode() 
    {
        
        _playerHud.SetActive(_isInTrailerMode);
        if (_isInTrailerMode)
        {
            _trailerModeButtonText.text = "Enter Trailer Mode";
            _isInTrailerMode = false;
        }
        else 
        {
            _trailerModeButtonText.text = "Exit Trailer Mode";
            _isInTrailerMode = true;
        }
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
        _infiniteFocusToggleButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _infiniteAmmoModeButton.onClick.RemoveAllListeners();
        _moveSceneBackwardButton.onClick.RemoveAllListeners();
        _moveSceneForwardButton.onClick.RemoveAllListeners();
        _moveToSceneButton.onClick.RemoveAllListeners();
        _devUiModeButton.onClick.RemoveAllListeners();
        _toggleTrailerModeButton.onClick.RemoveAllListeners();
        if (_toggleFreeLookCamButton == null) return;
        _toggleFreeLookCamButton.GetComponent<Button>().onClick.RemoveAllListeners();
    }
    #endif
}


