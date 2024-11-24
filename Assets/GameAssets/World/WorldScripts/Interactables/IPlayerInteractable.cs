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
    public bool InteractEnabled { get; set; }
    /// <summary>
    /// Will be called when something needs a criteria for interaction
    /// </summary>
    abstract void CanBeInteractedWith();
    /// <summary>
    /// Will be called when interacted with by the player
    /// </summary>
    abstract void OnInteractedByPlayer();
}
