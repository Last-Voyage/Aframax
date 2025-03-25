/*****************************************************************************
// File Name :         DevUiPopUp.cs
// Author :            Nabil Tagba
// Creation Date :     2/23/2025
//
// Brief Description : Keeps track of the player proximity, and if
// IsInDevUiMode is enabled. when both conditions are met, the pop up is enabled
*****************************************************************************/
using UnityEngine;

/// <summary>
/// Keeps track of the player proximity, and if
/// IsInDevUiMode is enabled. when both conditions are met, the pop up is enabled
/// </summary>
public class DevUiPopUp : MonoBehaviour
{

    [SerializeField] private GameObject _devUiObject;
    private GameObject _player;

    /// <summary>
    /// happens when the game starts
    /// </summary>
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// happens at a fix rate over time
    /// </summary>
    private void FixedUpdate()
    {
        //is the player close enough and the player is dev ui mode
        if (IsPlayerWithinProximity() && IsPlayerInDevUiMode())
        {
            //enable dev ui
            EnableUI();
        }
        else if (IsPlayerInDevUiMode() == false)
        {
            DisableUI();
        }

        if (_devUiObject.activeSelf)
        {
            //make sure the ui faces the player
            _devUiObject.transform.LookAt(_player.transform, Vector3.up);
        }
    }

    /// <summary>
    /// checks to see if the player is close enough
    /// </summary>
    /// <returns></returns>
    private bool IsPlayerWithinProximity()
    {
        bool isPlayerCloseEnough = false;

        // is player close enough
        if (Vector3.Distance(_player.transform.position, transform.position) < 10)
        {
            isPlayerCloseEnough = true;
        }
        // not close enough
        else if (Vector3.Distance(_player.transform.position, transform.position) > 10)
        {
            isPlayerCloseEnough = false;
        }

        return isPlayerCloseEnough;
    }

    /// <summary>
    /// checks to see if the player is in dev ui mode
    /// </summary>
    /// <returns></returns>
    private bool IsPlayerInDevUiMode()
    {
        return ConsoleController.Instance.IsInDevUiMode;
    }

    /// <summary>
    /// enables the pop up ui
    /// </summary>
    private void EnableUI() 
    {
        //Enable ui
        _devUiObject.SetActive(true);

    }


    /// <summary>
    /// disable the pop up ui
    /// </summary>
    private void DisableUI()
    {
        //Enable ui
        _devUiObject.SetActive(false);

    }

}
