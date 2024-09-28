using System.Collections;
using System.Collections.Generic;
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
                Time.timeScale = 0;

                
            }
            else 
            {
                Time.timeScale = 1;
                _content.SetActive(true);
                
            }
        }
    }


    private void HurtPlayer()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>().TakeDamage(1);
    }

    private void OnDestroy()
    {
        _playerTakeDamageButton.onClick.RemoveAllListeners();
    }


}
