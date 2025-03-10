using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeInteractableHelper : MonoBehaviour
{
    public void SetHudInventoryImage(int imageId)
    {
        GameObject.FindFirstObjectByType<HudInventoryManager>().SetInventoryImage(imageId);
    }

    public void StoryManagerProgressNextStoryBeat()
    {
        StoryManager.Instance.ProgressNextStoryBeat();
    }

    public void StoryManagerTriggerStoryBeat(int storyBeatId)
    {
        StoryManager.Instance.TriggerStoryBeat(storyBeatId);
    }

    public void HudInventoryDisableImages()
    {
        GameObject.FindFirstObjectByType<HudInventoryManager>().DisableInventoryImages();
    }
}
