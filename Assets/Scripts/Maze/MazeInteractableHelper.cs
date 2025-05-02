/******************************************************************************
// File Name:       MazeInteractableHelper.cs
// Author:          Miles Rogers
// Contributor:     ...
// Creation Date:   March 9th, 2025
//
// Description:     Helpers for interactable objects in maze subscenes
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeInteractableHelper : MonoBehaviour
{
    /// <summary>
    /// Calls SetInventoryImage() in the active HudInventoryManager
    /// </summary>
    /// <param name="imageId">Index of inventory image to display</param>
    public void SetHudInventoryImage(int imageId)
    {
        GameObject.FindFirstObjectByType<HudInventoryManager>().SetInventoryImage(imageId);
    }

    /// <summary>
    /// Calls ProgressNextStoryBeat() in the active StoryManager
    /// </summary>
    public void StoryManagerProgressNextStoryBeat()
    {
        StoryManager.Instance.ProgressNextStoryBeat();
    }

    /// <summary>
    /// Calls TriggerStoryBeat() in the active StoryManager
    /// </summary>
    /// <param name="storyBeatId">The ID of the story beat to trigger</param>
    public void StoryManagerTriggerStoryBeat(int storyBeatId)
    {
        StoryManager.Instance.TriggerStoryBeat(storyBeatId);
    }

    /// <summary>
    /// Calls DisableInventoryImages() in the active HudInventoryManager
    /// </summary>
    public void HudInventoryDisableImages()
    {
        GameObject.FindFirstObjectByType<HudInventoryManager>().DisableInventoryImages();
    }
}
