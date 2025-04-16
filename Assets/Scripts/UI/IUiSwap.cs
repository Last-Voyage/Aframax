/*****************************************************************************
// File Name :         IUiSwap.cs
// Author :            Nick Rice
// Creation Date :     4/15/2025
//
// Brief Description : Interface for implementing a controller and keyboard ui swap
*****************************************************************************/

public interface IUiSwap
{
    /// <summary>
    /// Will be called when the player switches between keyboard and controller
    /// </summary>
    abstract void OnUiSwap();
}
