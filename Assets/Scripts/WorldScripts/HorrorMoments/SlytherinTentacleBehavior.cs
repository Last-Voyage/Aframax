/**********************************************************************************************************************
// File Name :          SlytherinTentacleBehavior.cs
// Author :             Andrew Stapay
// Creation Date :      2/4/25
//
// Brief description :  Triggers tentacle movement for the Slytherin Horror Moment
**********************************************************************************************************************/
using Cinemachine;
using System.Collections;
using UnityEngine;

/// <summary>
/// Moves and destroys the tentacle for the Slytherin Horror Moment
/// </summary>
public class SlytherinTentacleBehavior : MonoBehaviour
{
    // Animation trigger
    private const string _TENTACLE_MOVE_TRIGGER = "MoveTentacle";
    [SerializeField] private CinemachineVirtualCamera _dragCam;
    private CinemachineVirtualCamera _playerCam;

    /// <summary>
    /// Called to make the tentacle play its animation
    /// </summary>
    public void MoveAway()
    {
        // Everything should already be set up, just play the animation
        Animator animator = GetComponent<Animator>();

        // Disable things from other objects
        CameraManager.Instance.InvokeOnCinematicStart();

        //disable player movement and pan camera to cinematic spot
        //get the player virtual camera
        _playerCam = PlayerCameraController.Instance.PlayerVirtualCamera;

        _dragCam.enabled = true;
        _playerCam.enabled = false;

        animator.SetTrigger(_TENTACLE_MOVE_TRIGGER);
    }

    /// <summary>
    /// Called to destroy the tentacle once it has completed its motion
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator DestroyTentacle(float time)
    {
        yield return new WaitForSeconds(time);

        _playerCam.enabled = true;
        _dragCam.enabled = false;

        // Enabling the things we disabled with the previous event
        CameraManager.Instance.InvokeOnCinematicEnd();

        Destroy(transform.parent.GetChild(0).gameObject);
        Destroy(this.gameObject);
    }
}
