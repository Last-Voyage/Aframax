/******************************************************************************
// File Name:       TheChunkChanger.cs
// Author:          Nick Rice
// Creation Date:   September 30th, 2024
//
// Description:     This script fires off the event that changes the chunks
******************************************************************************/
using UnityEngine;

/// <summary>
/// This script fires off the event that changes the chunks
/// </summary>
public class TheChunkChanger : MonoBehaviour
{
    private const string _TAG_CHECK_NAME = "BackOfBoat";

    /// <summary>
    /// When the object attached to this script touches the end of the boat object; it 
    /// Fires off the event to change chunks
    /// </summary>
    /// <param name="collision">The object that is interacting with the trigger</param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(_TAG_CHECK_NAME))
        {
            EnvironmentManager.Instance.SendChangeTheChunk()?.Invoke();
        }
    }
}
