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

    private Coroutine _footstepsCoroutine;

    //TODO Remove after LV-324
    public static RuntimeSfxManager SFXInstance;

    #region Enable and Action Subscriptions
    /// <summary>
    /// Subscribes to any needed actions and initializes the footsteps
    /// </summary>
    public override void SetupMainManager()
    {
        base.SetupMainManager();
        //TODO Uncomment after LV-324
        //InitializeFootstepInstance();
        SubscribeToActions(true);
        //TODO Remove after LV-324
        SFXInstance = this;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SubscribeToActions(false);
    }

    /// <summary>
    /// Subscribes and unsubscribes from actions
    /// </summary>
    /// <param name="val"> if true, subscribes </param>
    private void SubscribeToActions(bool val)
    {
        if (val)
        {
            APlayOneShotSFX += PlayOneShotSFX;
            
            return;
        }

        APlayOneShotSFX -= PlayOneShotSFX;
        
    }

    //TODO Remove after LV-324
    public void SubscribeToGameplayActions(bool val)
    {
        if(val)
        {
            InitializeFootstepInstance();
            PlayerManager.Instance.GetOnMovementStartEvent().AddListener(PlayFootSteps);
            PlayerManager.Instance.GetOnMovementEndEvent().AddListener(StopFootsteps);
            return;
        }

        PlayerManager.Instance.GetOnMovementStartEvent().RemoveListener(PlayFootSteps);
        PlayerManager.Instance.GetOnMovementEndEvent().RemoveListener(StopFootsteps);
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

    #region Footsteps

    /// <summary>
    /// Initializes the foot step instance
    /// </summary>
    private void InitializeFootstepInstance()
    {
        if (FmodSfxEvents.Instance.HardSurfaceWalking.IsNull)
        {
            return;
        }

        _hardSurfaceWalkingEventInstance = RuntimeManager.CreateInstance(FmodSfxEvents.Instance.HardSurfaceWalking);
    }

    /// <summary>
    /// Plays foot steps when the player moves
    /// </summary>
    private void PlayFootSteps()
    { 
        _footstepsCoroutine = StartCoroutine(LoopFootSteps());
    }

    /// <summary>
    /// Stops the footstep coroutine
    /// </summary>
    private void StopFootsteps()
    {
        StopCoroutine(_footstepsCoroutine);
    }

    /// <summary>
    /// Plays a single instance of the player footstep if the player is grounded
    /// </summary>
    private void PlayFootStep()
    {
        if (PlayerMovementController.IsGrounded)
        {
            if (FmodSfxEvents.Instance.HardSurfaceWalking.IsNull)
            {
                return;
            }

            _hardSurfaceWalkingEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            _hardSurfaceWalkingEventInstance.start();
            _hardSurfaceWalkingEventInstance.release();
        }
    }

    /// <summary>
    /// Plays the footstep SFX repeatedly while moving
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoopFootSteps()
    {
        float timer = 0.0f;

        while (true)
        {
            if (timer > FmodSfxEvents.Instance.FootstepSpeed)
            {
                PlayFootStep();
                timer = 0.0f;
            }

            timer += Time.deltaTime;

            yield return null;
        }
    }

    #endregion Footsteps

    #endregion
}
