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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles all SFX during runtime and how / where they play.
/// </summary>
public class RuntimeSfxManager : AudioManager
{
    public static Action<EventReference, Vector3> APlayOneShotSfx;
    public static Action<EventInstance, Vector3> APlayOneShotSfxInstance;
    public static Action<EventReference, GameObject> APlayOneShotSfxAttached;

    private EventInstance _walkingEventInstance;
    private EventReference _defaultWalkingSfx;

    private Coroutine _footstepsCoroutine;

    public static RuntimeSfxManager Instance;

    private WaitForSeconds footstepDelay;
    private WaitForSeconds firstFootstepDelay;
    //determins if the foot steps should be played or not
    public bool CanPlayFootSteps = true;

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
            APlayOneShotSfxInstance += PlayOneShotSfxInstance;
            APlayOneShotSfxAttached += PlayOneShotSfxAttached;

            return;
        }

        APlayOneShotSfx -= PlayOneShotSfx;
        APlayOneShotSfxInstance -= PlayOneShotSfxInstance;
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
    /// Plays an audio event via event instance at a specific location
    /// </summary>
    /// <param name="eventInstance"> The audio event to play </param>
    /// <param name="worldPosition"> The location for the audio to play at </param>
    private void PlayOneShotSfxInstance(EventInstance eventInstance, Vector3 worldPosition = new Vector3())
    {
        if (eventInstance.IsUnityNull())
        {
            return;
        }

        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPosition));
        eventInstance.start();
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
    public void InitializeFootstepInstances()
    {
        _defaultWalkingSfx = FmodSfxEvents.Instance.DefaultWalking;

        if (_defaultWalkingSfx.IsNull)
        {
            return;
        }
        _walkingEventInstance = RuntimeManager.CreateInstance(_defaultWalkingSfx);

        foreach(FootStepType footStepType in FmodSfxEvents.Instance.MaterialFootsteps)
        {
            footStepType.CreateInstance();
        }
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
        if (CanPlayFootSteps == false) return;
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
            EventInstance walkInstance = DetermineFootstepAudio();

            if (!walkInstance.isValid())
            {
                return;
            }

            walkInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            walkInstance.start();
        }
    }

    /// <summary>
    /// Determines what footstep audio we should play based on what we are standing on
    /// </summary>
    /// <returns></returns>
    private EventInstance DetermineFootstepAudio()
    {
        foreach(FootStepType footStepType in FmodSfxEvents.Instance.MaterialFootsteps)
        {
            //Skip this footstep type if it is missing values
            if(!footStepType.AssociatedInstance.isValid() || 
                footStepType.AssociatedMaterial.IsUnityNull())
            {
                continue;
            }

            PlayerMovementController.CurrentGround.
                TryGetComponent<Renderer>(out Renderer groundRenderer);

            // Iterates through each material
            foreach (Material mat in groundRenderer.sharedMaterials)
            {
                if (footStepType.AssociatedMaterial.name == mat.name)
                {
                    return footStepType.AssociatedInstance;
                }
            }
            
        }
        return _walkingEventInstance;
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
