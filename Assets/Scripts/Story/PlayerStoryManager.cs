/*****************************************************************************
// File Name :         PlayerStoryManager.cs
// Author :            Ryan Swanson
// Contributors:       
// Creation Date :     3/27/25
//
// Brief Description : Provides the story manager with access to modify the player
*****************************************************************************/

using UnityEngine;

/// <summary>
/// Provides the story manager with access to modify the player
/// </summary>
public class PlayerStoryManager : MonoBehaviour
{
    /// <summary>
    /// Changes the value of the player speed
    /// </summary>
    /// <param name="newSpeed"></param>
    public void ModifyPlayerSpeed(float newSpeed)
    {
        PlayerMovementController.Instance.SetCurrentMovementSpeed(newSpeed);
    }

    /// <summary>
    /// Changes the value of the player acceleration
    /// </summary>
    /// <param name="newAcceleration"></param>
    public void ModifyPlayerAcceleration(float newAcceleration)
    {
        PlayerMovementController.Instance.SetCurrentAcceleration(newAcceleration);
    }
}
