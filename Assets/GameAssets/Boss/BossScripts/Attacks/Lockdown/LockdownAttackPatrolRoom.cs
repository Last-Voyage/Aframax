/*****************************************************************************
// File Name :         PatrolEnemyRoom.cs
// Author :            Ryan Swanson
// Creation Date :     10/31/2024
//
// Brief Description : Controls functionality for detecting if the player is in the room
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls functionality for detecing if the player is in the associated room
/// </summary>
public class LockdownAttackPatrolRoom : MonoBehaviour
{
    /// <summary>
    /// Exists so that if the room has multiple trigger colliders. 
    /// If they overlap that can be considered 2 enter collisions before exiting any colliders
    /// </summary>
    private int _playerCollisionCounter = 0;

    // Event for when the player enters the room
    private UnityEvent _onPlayerRoomEnter = new();
    // Event for when the player exits the room
    private UnityEvent _onPlayerRoomExit = new();

    #region Collision
    /// <summary>
    /// Checks if the player entered the room
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        if(IsColliderPlayer(collider))
        {
            IncreaseCollisionCounter();
        }
    }

    /// <summary>
    /// Checks if the player exited the room
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerExit(Collider collider)
    {
        if (IsColliderPlayer(collider))
        {
            DecreaseCollisionCounter();
        }
    }

    /// <summary>
    /// Increases the counter for detecting if the player is in the room
    /// Needed to allow for multiple trigger colliders to be on one room
    /// </summary>
    private void IncreaseCollisionCounter()
    {
        if(_playerCollisionCounter == 0)
        {
            InvokeOnPlayerRoomEnter();
        }
        _playerCollisionCounter++;
    }

    /// <summary>
    /// Decreases the counter for detecting if the player is in the room
    /// Needed to allow for multiple trigger colliders to be on one room
    /// </summary>
    private void DecreaseCollisionCounter()
    {
        _playerCollisionCounter--;
        if(_playerCollisionCounter ==0)
        {
            InvokeOnPlayerRoomExit();
        }
    }

    /// <summary>
    /// Checks if the collision object is the player
    /// </summary>
    /// <param name="collider"> The collider that we are checking if it belongs to the play</param>
    /// <returns> If the provided collider belongs to the player </returns>
    private bool IsColliderPlayer(Collider collider)
    {
        return collider.gameObject.GetComponent<PlayerCollision>();
    }
    #endregion

    #region Events
    /// <summary>
    /// Invokes event for the player entering the room
    /// </summary>
    private void InvokeOnPlayerRoomEnter()
    {
        _onPlayerRoomEnter?.Invoke();
    }

    /// <summary>
    /// Invokes event for the player exiting the room
    /// </summary>
    private void InvokeOnPlayerRoomExit()
    {
        _onPlayerRoomExit?.Invoke();
    }
    #endregion

    #region Getters
    public bool IsPlayerInRoom() => (_playerCollisionCounter > 0);

    public UnityEvent GetOnPlayerRoomEnterEvent() => _onPlayerRoomEnter;
    public UnityEvent GetOnPlayerRoomExitEvent() => _onPlayerRoomExit;
    #endregion
}
