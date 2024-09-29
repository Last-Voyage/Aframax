/*
// Name: ConsoleController.CS
// Author: Nabil Tagba
// Overview: Handles the console being turned on and off
// along with its hosting the methodes the quick action buttons
// in the console will use
 */
using UnityEngine;
using UnityEngine.UI;



public class ConsoleController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _isInDeveloperMode = false;

    
    [Space]
    [Header("References")]
    [SerializeField] private GameObject _content;
    [SerializeField] private Button _playerTakeDamageButton;

    private void Start()
    {
        //linking player take damage button to corresponding methode
        _playerTakeDamageButton.onClick.AddListener(HurtPlayer);
    }
    private void Update()
    {
        //toggle console on and off
        if (_isInDeveloperMode && Input.GetKeyUp(KeyCode.C))
        {
            
            if (_content.active)
            {
                _content.SetActive(false);
                Time.timeScale = 1;
            }
            else 
            {
                _content.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    /// <summary>
    /// deal damage to the player
    /// </summary>
    private void HurtPlayer()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>().TakeDamage(1);
    }

    /// <summary>
    /// called when the object is destroyed.
    /// removes all listener to lower chances
    /// of memmory leaks
    /// </summary>
    private void OnDestroy()
    {
        _playerTakeDamageButton.onClick.RemoveAllListeners();
    }


}
