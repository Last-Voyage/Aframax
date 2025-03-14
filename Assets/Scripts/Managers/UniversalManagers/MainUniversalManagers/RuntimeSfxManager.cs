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
using UnityEngine.InputSystem;

/// <summary>
/// Handles all SFX during runtime and how / where they play.
/// </summary>
public class RuntimeSfxManager : AudioManager
{
    public static Action<EventReference, Vector3> APlayOneShotSfx;

    public static Action<EventReference, GameObject> APlayOneShotSfxAttached;

    private EventInstance _walkingEventInstance;
    private EventReference _currentWalkingSfx;

    private Coroutine _footstepsCoroutine;

    public static RuntimeSfxManager Instance;

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
            APlayOneShotSfx += PlayOneShotSfx;
            APlayOneShotSfxAttached += PlayOneShotSfxAttached;

            return;
        }

        APlayOneShotSfx -= PlayOneShotSfx;
        APlayOneShotSfxAttached -= PlayOneShotSfxAttached;
    }

    /// <summary>
    /// Preset variables at the start of the game
    /// </summary>
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
    private void PlayOneShotSfx(EventReference eventReference, Vector3 worldPosition = new Vector3())
    {
        if (CheckForNullSfx(eventReference))
        {
            return;
        }

        RuntimeManager.PlayOneShot(eventReference, worldPosition);
    }

    /// <summary>
    /// added to play a sound instantly
    /// </summary>
    /// <param name="eventInstance"></param>
    public void PlayOneShotEventInstance(EventInstance eventInstance)
    {
        eventInstance.start();
    }

    /// <summary>
    /// Plays an audio event on a specific object
    /// </summary>
    /// <param name="eventReference">reference to the FMOD SFX event</param>
    /// <param name="attachedObject">object that the audio is playing attached to</param>
    private void PlayOneShotSfxAttached(EventReference eventReference, GameObject attachedObject)
    {
        if (CheckForNullSfx(eventReference))
        {
            return;
        }

        RuntimeManager.PlayOneShotAttached(eventReference, attachedObject);
    }

    /// <summary>
    /// Fades in a sfx using an instance
    /// </summary>
    /// <param name="eventInstance"> The sfx to fade in </param>
    /// <param name="fadeTime"> The time to fade in </param>
    public void FadeInLoopingOneShot(EventInstance eventInstance, float fadeTime)
    {
        eventInstance.start();

        StartCoroutine(FadeEventInstance(eventInstance, fadeTime,1));
    }

    /// <summary>
    /// Fades out a sfx using an instance
    /// </summary>
    /// <param name="inst"> The sfx to fade out </param>
    /// <param name="fadeTime"> The time to fade out </param>
    public void FadeOutLoopingOneShot(EventInstance inst, float fadeTime)
    {
        StartCoroutine(FadeEventInstance(inst, fadeTime,0));
    }

    /// <summary>
    /// Moves an instances volume from where it starts to an end
    /// </summary>
    /// <param name="eventInstance"> The instance to adjust the volume </param>
    /// <param name="fadeTime"> The time to fade </param>
    /// <param name="endVol"> The ending volume </param>
    /// <returns></returns>
    private IEnumerator FadeEventInstance(EventInstance eventInstance, float fadeTime, float endVol)
    {
        float progress = 0;
        float currentVol;
        eventInstance.getVolume(out float startingVol);
        while(progress<1)
        {
            progress += Time.deltaTime / fadeTime;

            currentVol = Mathf.Lerp(startingVol, endVol,progress);
            currentVol = Mathf.Clamp(currentVol, 0, 1);

            eventInstance.setVolume(currentVol);
            yield return null;
        }
    }

    /// <summary>
    /// Checks for if the audio is null before playing it
    /// </summary>
    /// <param name="eventReference">The EventReference we are checking is null</param>
    /// <returns>Returns if the sfx are null or not</returns>
    private bool CheckForNullSfx(EventReference eventReference)
    {
        if (eventReference.IsNull)
        {
            Debug.LogWarning("FMOD Event is null. Make sure it's assigned in the Audio Manager!");
            return true;
        }
        return false;
    }

    #region Footsteps

    /// <summary>
    /// Initializes the footstep instance
    /// </summary>
    public void InitializeFootstepInstance()
    {
        if(GameStateManager.Instance.IsPlayerAboveDeck())
        {
            _currentWalkingSfx = FmodSfxEvents.Instance.AboveDeckWalking;
        }
        else
        {
            _currentWalkingSfx = FmodSfxEvents.Instance.BelowDeckWalking;
        }

        if (_currentWalkingSfx.IsNull)
        {
            return;
        }
        _walkingEventInstance = RuntimeManager.CreateInstance(_currentWalkingSfx);
    }

    /// <summary>
    /// Releases the hard surface event instance
    /// </summary>
    private void ReleaseFootstepInstance()
    {
        if (!_walkingEventInstance.isValid())
        {
            return;
        }
        _walkingEventInstance.release();
    }

    /// <summary>
    /// Plays footsteps when the player moves
    /// </summary>
    /// <param name="unused"> Unused parameter that was added as a consequence of UnityEvents </param>
    private void PlayFootSteps(InputAction unused)
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
            if (_currentWalkingSfx.IsNull)
            {
                return;
            }

            _walkingEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            _walkingEventInstance.start();
        }
    }

    /// <summary>
    /// Plays the footstep SFX repeatedly while moving
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoopFootSteps()
    {
        // Update the initial footstep speed
        float currentSpeedMultiplier = PlayerMovementController.Instance.CurrentFocusMoveSpeedMultiplier;
        firstFootstepDelay = new WaitForSeconds(FmodSfxEvents.Instance.FirstFootstepDelay
            * (1 + (1 - currentSpeedMultiplier)));

        yield return firstFootstepDelay;

        PlayFootStep();

        while (true)
        {
            PlayFootStep();

            // Update the footstep speed
            currentSpeedMultiplier = PlayerMovementController.Instance.CurrentFocusMoveSpeedMultiplier;
            footstepDelay = new WaitForSeconds(FmodSfxEvents.Instance.FootstepDelay
                * (1 + (1 - currentSpeedMultiplier)));

            yield return footstepDelay;
        }
    }

    #endregion Footsteps

    #endregion
}
