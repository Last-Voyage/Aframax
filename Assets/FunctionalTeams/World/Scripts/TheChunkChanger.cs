/******************************************************************************
// File Name:       TheChunkChanger.cs
// Author:          Nick Rice
// Creation Date:   September 30th, 2024
//
// Description:     This script fires off the event that changes the chunks
******************************************************************************/
using UnityEngine;

public class TheChunkChanger : MonoBehaviour
{
    /// <summary>
    /// When the object attached to this script touches the end of the boat object; it 
    /// Fires off the event to change chunks
    /// </summary>
    /// <param name="collision">The object that is interacting with the trigger</param>
    private void OnTriggerEnter(Collider collision)
    {
        // The conditional should be changed
        if (collision.gameObject.name == "TempObjForChunkLoading")
        {
            EnvironmentManager.Instance.SendChangeTheChunk()?.Invoke();
        }
    }
}
