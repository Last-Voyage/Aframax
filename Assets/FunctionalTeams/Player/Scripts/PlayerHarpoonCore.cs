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
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerCameraController _playerCamera;
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
        _playerMovementController.SubscribeInput();
    }

    /// <summary>
    /// Subscribes to camera input
    /// </summary>
    private void SubscribeToCameraInput()
    {
        _playerCamera.SubscribeInput();
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
        _playerMovementController.UnsubscribeInput();
    }

    /// <summary>
    /// Unsubscribes to camera input
    /// </summary>
    private void UnsubscribeToCameraInput()
    {
        _playerCamera.UnsubscribeInput();
    }

    /// <summary>
    /// Unsubscribes to harpoon input
    /// </summary>
    private void UnsubscribeToHarpoonInput()
    {
         
    }
    #endregion

    #region Getters
    public PlayerMovementController GetPlayerMovementController() => _playerMovementController;
    public PlayerHealth GetPlayerHealth() => _playerHealth;
    public PlayerCameraController GetPlayerCameraController() => _playerCamera;
    public HarpoonGun GetHarpoonGun() => _harpoonGun;
    #endregion
}
