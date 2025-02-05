/**********************************************************************************************************************
// File Name :         IPlayerInteractable.cs
// Author :            Alex Kalscheur
// Creation Date :     11/7/2024
//
// Brief Description : Interface for implementing interactable system with player
**********************************************************************************************************************/

/// <summary>
/// Interface for implementing interactable system with player
/// </summary>
public interface IPlayerInteractable
{
    /// <summary>
    /// Will be called when interacted with by the player
    /// </summary>
    abstract void OnInteractedByPlayer();
}
