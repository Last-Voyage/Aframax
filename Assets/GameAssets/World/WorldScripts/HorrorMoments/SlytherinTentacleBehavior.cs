/**********************************************************************************************************************
// File Name :          LightShift.cs
// Author :             Andrew Stapay
// Creation Date :      2/4/25
//
// Brief description :  Triggers tentacle movement for the Slytherin Horror Moment
**********************************************************************************************************************/
using UnityEngine;

/// <summary>
/// Moves and destroys the tentacle for the Slytherin Horror Moment
/// </summary>
public class SlytherinTentacleBehavior : MonoBehaviour
{
    // Animation trigger
    private const string _TENTACLE_MOVE_TRIGGER = "MoveTentacle";

    /// <summary>
    /// Called to make the tentacle play its animation
    /// </summary>
    public void MoveAway()
    {
        // Everything should already be set up, just play the animation
        Animator animator = GetComponent<Animator>();

        animator.SetTrigger(_TENTACLE_MOVE_TRIGGER);
    }

    /// <summary>
    /// Called to destroy the tentacle once it has completed its motion
    /// Called using Unity Animation Events, so it seems as though it is never reference, but believe me, it's used
    /// </summary>
    private void DestroyTentacle()
    {
        Destroy(this.gameObject);
    }
}
