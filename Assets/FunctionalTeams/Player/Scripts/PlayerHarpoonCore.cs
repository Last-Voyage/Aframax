/*****************************************************************************
// File Name :         PlayerHarpoonCore.cs
// Author :            Ryan Swanson
// Creation Date :     9/28/2024
//
// Brief Description : Holds higher level functionality to setup the player and harpoon
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds functionality for managing the relationship of player scripts 
/// and centralizing certain functionality
/// </summary>
public class PlayerHarpoonCore : MonoBehaviour
{
    [SerializeField] private PlayerMovementController _playerMovementController;
    [SerializeField] private HarpoonGun _harpoonGun;

    private PlayerInputMap _playerInputMap;

    private void OnEnable()
    {
        SubscribePlayerInput();
    }

    private void OnDisable()
    {
        UnsubscribePlayerInput();
    }

    #region Input
    /// <summary>
    /// Subscribes to all needed input
    /// </summary>
    private void SubscribePlayerInput()
    {
        _playerInputMap = new();
        _playerInputMap.Enable();

        SubscribeToMovementInput();
        SubscribeToCameraInput();
        SubscribeToHarpoonInput();
    }

    /// <summary>
    /// Subscribes to movement input
    /// </summary>
    private void SubscribeToMovementInput()
    {

    }

    /// <summary>
    /// Subscribes to camera input
    /// </summary>
    private void SubscribeToCameraInput()
    {

    }

    /// <summary>
    /// Subscribes to harpoon input
    /// </summary>
    private void SubscribeToHarpoonInput()
    {
        _harpoonGun.SubscribeInput();
    }

    /// <summary>
    /// Unsubscribes to all input
    /// </summary>
    private void UnsubscribePlayerInput()
    {
        UnsubscribeToMovementInput();
        UnsubscribeToCameraInput();
        UnsubscribeToHarpoonInput();

        _playerInputMap.Disable();
    }

    /// <summary>
    /// Unsubscribes to movement input
    /// </summary>
    private void UnsubscribeToMovementInput()
    {

    }

    /// <summary>
    /// Unsubscribes to camera input
    /// </summary>
    private void UnsubscribeToCameraInput()
    {

    }

    /// <summary>
    /// Unsubscribes to harpoon input
    /// </summary>
    private void UnsubscribeToHarpoonInput()
    {
         
    }
    #endregion
}