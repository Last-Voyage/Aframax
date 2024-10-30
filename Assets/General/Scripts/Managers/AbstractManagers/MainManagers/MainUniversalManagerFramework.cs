/******************************************************************************
// File Name:       MainUniversalManagerFramework.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Provides the framework to be used by all main universal managers
******************************************************************************/

/// <summary>
/// Provides the framework for the main universal managers
/// Since Universal and Gameplay managers are pretty similar at the moment there isn't much here
/// </summary>
public class MainUniversalManagerFramework : MainManagerFramework
{
    /// <summary>
    /// Establishes the instance for the manager
    /// </summary>
    public override void SetupInstance()
    {

    }

    /// <summary>
    /// Performs any needed setup for the manager
    /// </summary>
    public override void SetupMainManager()
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// Subscribes to any universal events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        SceneLoadingManager.Instance.GetGameplaySceneLoaded().AddListener(SubscribeToGameplayEvents);
    }

    /// <summary>
    /// Subscribes to events exclusive to the gameplay scenes
    /// Used for subscribing to any GameplayManager events
    /// </summary>
    protected virtual void SubscribeToGameplayEvents()
    {

    }

    /// <summary>
    /// Unsubscribes from any universal events
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        SceneLoadingManager.Instance.GetGameplaySceneLoaded().RemoveListener(SubscribeToGameplayEvents);
    }

    /// <summary>
    /// Unsubscribes to events exclusive to gameplay scenes
    /// </summary>
    protected virtual void UnsubscribeToGameplayEvents()
    {

    }
}
