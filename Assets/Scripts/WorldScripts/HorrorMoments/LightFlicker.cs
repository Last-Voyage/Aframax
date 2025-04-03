/**********************************************************************************************************************
// File Name :          LightShift.cs
// Author :             Andrew Stapay
// Creation Date :      2/4/25
//
// Brief description :  Triggers light flickering for the Slytherin Horror Moment
**********************************************************************************************************************/

using System.Collections;
using UnityEngine;

/// <summary>
/// Causes the lights to flicker when passed through
/// </summary>
public class LightFlicker : MonoBehaviour
{
    /// <summary>
    /// Check to ensure the collider does not trigger twice
    /// </summary>
    private bool _triggered = false;
    
    /// <summary>
    /// Called when the player makes contact with the associated trigger
    /// </summary>
    /// <param name="other"> Information about the other collider in the collision </param>
    private void OnTriggerEnter(Collider other)
    {
        if (_triggered)
        {
            return;
        }
        
        if (other.gameObject.tag == "Player")
        {
            _triggered = true;
            VfxManager.Instance.InvokeOnLightFlicker();
            Destroy(this.gameObject);
        }
    }
}
