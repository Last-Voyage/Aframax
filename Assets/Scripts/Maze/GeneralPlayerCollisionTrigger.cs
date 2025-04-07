/*****************************************************************************
// File Name :         GeneralPlayerCollisionTrigger.cs
// Author :            Ryan Swanson
// Contributor:        
// Creation Date :     4/6/25
//
// Brief Description : Provides a general collision trigger for design
*****************************************************************************/

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides a general collision trigger for design to use how they want
/// </summary>
public class GeneralPlayerCollisionTrigger : MonoBehaviour
{
    [SerializeField] private bool _doesDestroyOnContact;
    [SerializeField] private UnityEvent _onPlayerCollision;
    /// <summary>
    /// Called when contacting the player
    /// </summary>
    public void PlayerContact()
    {
        _onPlayerCollision?.Invoke();
        if(_doesDestroyOnContact )
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called to remove any listeners
    /// </summary>
    private void OnDestroy()
    {
        _onPlayerCollision?.RemoveAllListeners();
    }
}
