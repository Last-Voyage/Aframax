/******************************************************************************
// File Name:       StoryProgressionTrigger.cs
// Author:          Ryan Swanson
// Creation Date:   February 2nd, 2025
//
// Description:     A trigger that can be reused by design to progress story events
******************************************************************************/

using UnityEngine;

/// <summary>
/// Trigger to progress the story activated by the player
/// </summary>
public class StoryProgressionTrigger : MonoBehaviour
{
    /// <summary>
    /// Detects if it comes into contact with a player and progresses the story beat, then destroys itself
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //null check added for testing
            StoryManager storyManager = StoryManager.Instance;
            if(storyManager !=null)
            {
                storyManager.ProgressNextStoryBeat();
            }

            Destroy(gameObject);
        }
    }
}
