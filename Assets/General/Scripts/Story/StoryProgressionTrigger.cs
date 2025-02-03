/******************************************************************************
// File Name:       StoryProgressionTrigger.cs
// Author:          Ryan Swanson
// Creation Date:   February 2nd, 2025
//
// Description:     Manages sound effects during runtime.
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
            StoryManager.Instance.ProgressNextStoryBeat();

            Destroy(gameObject);
        }
    }
}
