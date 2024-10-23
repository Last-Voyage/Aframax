/*
// Name: ConsoleController.CS
// Author: Nabil Tagba
// Overview: Handles the console being turned on and off
// along with its hosting the methodes the quick action buttons
// in the console will use
 */
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



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

    private void Awake()
    {
       _playerInput = new PlayerInputMap();
        _playerInput.Enable();
    }

    private void Start()
    {
        //linking player take damage button to corresponding methode
        _playerTakeDamageButton.onClick.AddListener(HurtPlayer);
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

    /// <summary>
    /// deal damage to the player
    /// </summary>
    private void HurtPlayer()
    {
        float _amount;
        if (_dmgAmountInputField.text.Length > 0 && float.TryParse(_dmgAmountInputField.text, out _amount))
        {
            GameObject.FindObjectOfType<PlayerHealth>().TempEnemyDamage(_amount);
        }
        
    }

    /// <summary>
    /// called when the object is destroyed.
    /// removes all listener to lower chances
    /// of memmory leaks
    /// </summary>
    private void OnDestroy()
    {
        Time.timeScale = 1;
        _playerTakeDamageButton.onClick.RemoveAllListeners();
        _playerInput.Disable();
    }

    

}
