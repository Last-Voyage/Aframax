/**********************************************************************************************************************
// File Name :          LightShift.cs
// Author :             Andrew Stapay
// Creation Date :      1/31/25
//
// Brief description :  Triggers the Light Shift Horror moment when the player passes through the associated area.
**********************************************************************************************************************/
using UnityEngine;

/// <summary>
/// Activates the Light Shift Horror Moment when passed through
/// </summary>
public class LightShift : MonoBehaviour
{
    /// <summary>
    /// Called when the player makes contact with the associated trigger
    /// </summary>
    /// <param name="other"> Information about the other collider in the collision </param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            VfxManager.Instance.InvokeOnLightShift();
            Destroy(this.gameObject);
        }
    }
}
