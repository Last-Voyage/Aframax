/******************************************************************************
// File Name:       SFXManager.cs
// Author:          Andrea Swihart-DeCoster
// Creation Date:   October 1st, 2024
//
// Description:     Manages sound effects during runtime.
******************************************************************************/

using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles all SFX during runtime and how / where they play.
/// </summary>
public class RuntimeSfxManager : AudioManager
{
    public static Action<EventReference, Vector3> APlayOneShotSFX;

    private EventInstance _hardSurfaceWalkingEventInstance;

    #region Enable and Action Subscriptions

    private void OnEnable()
    {
        SubscribeToActions(true);
    }
    
    private void OnDisable()
    {
        SubscribeToActions(false);
    }

    /// <summary>
    /// Subscribes and unsubscribes from actions
    /// </summary>
    /// <param name="val"> if true, subscribes </param>
    private void SubscribeToActions(bool val)
    {
        if(val)
        {
            APlayOneShotSFX += PlayOneShotSFX;
            PlayerMovementController.UpdateMovingState.AddListener(PlayFootSteps);
            return;
        }

        APlayOneShotSFX -= PlayOneShotSFX;
        PlayerMovementController.UpdateMovingState.RemoveListener(PlayFootSteps);
    }

    #endregion Enable and Action Subscriptions

    #region FMOD Audio Functionality

    /// <summary>
    /// Plays an audio event at a specific position
    /// </summary>
    /// <param name="eventReference">reference to the FMOD SFX event </param>
    private void PlayOneShotSFX(EventReference eventReference, Vector3 worldPosition = new Vector3())
    {
        if (eventReference.IsNull)
        {
            Debug.LogWarning(eventReference + " is null.");
            return;
        }

        RuntimeManager.PlayOneShot(eventReference, worldPosition);
    }

    /// <summary>
    /// Plays footsteps when the player moves
    /// </summary>
    private void PlayFootSteps(bool isMoving)
    {
        if (isMoving)
        {
            StartCoroutine(PlayFootSteps());
        }
        else
        {

        }
    }

    /// <summary>
    /// Plays a single instance of the player footstep if the player is grounded
    /// </summary>
    private void PlayFootStep()
    {
        if (PlayerMovementController.IsGrounded)
        {
            Debug.Log("Footstep");
            _hardSurfaceWalkingEventInstance = RuntimeManager.CreateInstance(FmodSfxEvents.Instance.HardSurfaceWalking);
            _hardSurfaceWalkingEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            _hardSurfaceWalkingEventInstance.start();
            _hardSurfaceWalkingEventInstance.release();
        }
    }

    /// <summary>
    /// Plays the footstep SFX repeatedly while moving
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayFootSteps()
    {
        float timer = 0.0f;

        if (timer > FmodSfxEvents.Instance.FootstepSpeed)
        {
            PlayFootStep();
            timer = 0.0f;
        }

        timer += Time.deltaTime;

        yield return null;
    }

    #endregion
}
