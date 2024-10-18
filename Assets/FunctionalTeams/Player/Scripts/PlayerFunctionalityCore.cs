/*****************************************************************************
// File Name :         PlayerFunctionalityCore.cs
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
public class PlayerFunctionalityCore : MonoBehaviour
{
    //Contains all the associated player functionality
    //Controls player movement functionality
    [SerializeField] private PlayerMovementController _playerMovementController;
    //Controls player health functionality
    [SerializeField] private PlayerHealth _playerHealthController;
    //Controls camera movement
    [SerializeField] private PlayerCameraController _playerCamera;
    //Controls harpoon weapon functionality
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
    /// <summary>
    /// Gets the script containing player movement functionality
    /// </summary>
    /// <returns></returns>
    public PlayerMovementController GetPlayerMovementController() => _playerMovementController;
    /// <summary>
    /// Gets the script containing player health functionality
    /// </summary>
    /// <returns></returns>
    public PlayerHealth GetPlayerHealthController() => _playerHealthController;
    /// <summary>
    /// Gets the script containing player camera control functionality
    /// </summary>
    /// <returns></returns>
    public PlayerCameraController GetPlayerCameraController() => _playerCamera;
    /// <summary>
    /// Gets the script containing the player harpoon weapon functionality
    /// </summary>
    /// <returns></returns>
    public HarpoonGun GetHarpoonGun() => _harpoonGun;
    #endregion
}
