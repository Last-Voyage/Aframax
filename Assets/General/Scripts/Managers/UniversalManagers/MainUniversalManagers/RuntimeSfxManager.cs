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

/// <summary>
/// Handles all SFX during runtime and how / where they play.
/// </summary>
public class RuntimeSfxManager : AudioManager
{
    public static Action<EventReference, Vector3> APlayOneShotSfx;

    private EventInstance _hardSurfaceWalkingEventInstance;

    private Coroutine _footstepsCoroutine;

    public static RuntimeSfxManager Instance;

    private bool _shouldFootstepsPlay = true;

    private WaitForSeconds footstepDelay;
    private WaitForSeconds firstFootstepDelay;

    #region Enable and Action Subscriptions
    /// <summary>
    /// Subscribes to any needed actions and initializes the footsteps
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        SubscribeToActions(true);
    }

    /// <summary>
    /// Establishes the instance for the runtime sfx manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SubscribeToActions(false);
    }

    /// <summary>
    /// Subscribes to events in gameplay scenes
    /// </summary>
    protected override void SubscribeToGameplayEvents()
    {
        base.SubscribeToGameplayEvents();
        
        InitializeFootstepInstance();
        PlayerManager.Instance.GetOnMovementStartEvent().AddListener(PlayFootSteps);
        PlayerManager.Instance.GetOnMovementEndEvent().AddListener(StopFootsteps);

        AframaxSceneManager.Instance.GetOnLeavingGameplayScene.AddListener(StopFootsteps);
    }

    /// <summary>
    /// Unsubscribes to events in gameplay scenes
    /// </summary>
    protected override void UnsubscribeToGameplayEvents()
    {
        base.UnsubscribeToGameplayEvents();
        
        PlayerManager.Instance.GetOnMovementStartEvent().RemoveListener(PlayFootSteps);
        PlayerManager.Instance.GetOnMovementEndEvent().RemoveListener(StopFootsteps);
        
        AframaxSceneManager.Instance.GetOnLeavingGameplayScene.RemoveListener(StopFootsteps);
        
        ReleaseFootstepInstance();
    }

    /// <summary>
    /// Subscribes and unsubscribes from actions
    /// </summary>
    /// <param name="val"> if true, subscribes </param>
    private void SubscribeToActions(bool val)
    {
        if (val)
        {
            APlayOneShotSfx += PlayOneShotSFX;
            
            return;
        }

        APlayOneShotSfx -= PlayOneShotSFX;
    }

    private void Start()
    {
        footstepDelay = new WaitForSeconds(FmodSfxEvents.Instance.FootstepDelay);
        firstFootstepDelay = new WaitForSeconds(FmodSfxEvents.Instance.FirstFootstepDelay);
    }

    #endregion Enable and Action Subscriptions

    #region FMOD Audio Functionality

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Plays an audio event at a specific position
    /// </summary>
    /// <param name="eventReference">reference to the FMOD SFX event </param>
    /// <param name="worldPosition"> position where the sound plays in the world </param>
    private void PlayOneShotSFX(EventReference eventReference, Vector3 worldPosition = new Vector3())
    {
        if (eventReference.IsNull)
        {
            Debug.LogWarning("FMOD Event is null. Make sure it's assigned in the Audio Manager!");
            return;
        }

        RuntimeManager.PlayOneShot(eventReference, worldPosition);
    }

    #region Footsteps

    /// <summary>
    /// Initializes the footstep instance
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
    /// Releases the hard surface event instance
    /// </summary>
    private void ReleaseFootstepInstance()
    {
        if (!_hardSurfaceWalkingEventInstance.isValid())
        {
            return;
        }
        _hardSurfaceWalkingEventInstance.release();
    }

    /// <summary>
    /// Plays footsteps when the player moves
    /// </summary>
    private void PlayFootSteps()
    { 
        StopFootsteps();
        _footstepsCoroutine = StartCoroutine(LoopFootSteps());
    }

    /// <summary>
    /// Stops the footstep coroutine
    /// </summary>
    private void StopFootsteps()
    {
        if (_footstepsCoroutine == null)
        {
            return;
        }
        
        StopCoroutine(_footstepsCoroutine);
        _footstepsCoroutine = null;
    }

    /// <summary>
    /// Plays a single instance of the player footstep if the player is grounded
    /// </summary>
    private void PlayFootStep()
    {
        if (PlayerMovementController.IsGrounded && PlayerMovementController.IsMoving)
        {
            if (FmodSfxEvents.Instance.HardSurfaceWalking.IsNull)
            {
                return;
            }

            _hardSurfaceWalkingEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            _hardSurfaceWalkingEventInstance.start();
        }
    }

    /// <summary>
    /// Plays the footstep SFX repeatedly while moving
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoopFootSteps()
    {
        yield return firstFootstepDelay;

        PlayFootStep();

        while (true)
        {
            PlayFootStep();
            
            yield return footstepDelay;
        }
    }

    #endregion Footsteps

    #endregion
}
