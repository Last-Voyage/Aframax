/**********************************************************************************************************************
// File Name :         IPlayerInteractable.cs
// Author :            Alex Kalscheur
// Creation Date :     11/7/2024
//
// Brief Description : Interface for implementing interactable system with harpoon
**********************************************************************************************************************/

/// <summary>
/// Interface for implementing interactable system with player
/// </summary>
public interface IHarpoonInteractable
{
    /// <summary>
    /// Will be called when the object is hit by a harpoon
    /// </summary>
    abstract void OnHitByHarpoon();
}
