/*****************************************************************************
// File Name :         DialogueSfxManager.cs
// Author :            Andrew Stapay
// Creation Date :     4/17/25
//
// Brief Description : Handles the progression of dialogue during runtime
*****************************************************************************/
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Handles the creation and progression of dialogue during the game
/// </summary>
public class DialogueSfxManager : MainUniversalManagerFramework
{
    public static DialogueSfxManager Instance;

    private List<ScriptableDialogueUi> _dialogueQueue = new List<ScriptableDialogueUi>();
    private Coroutine _dialogueCoroutine = null;

    private EventInstance _currentDialogueEventInstance;

    [SerializeField] private float _nextDialogueWaitTime = 0.5f;
    private WaitForSeconds _waitForNextDialogue;

    /// <summary>
    /// Initializes the WaitForSeconds for waiting for the next dialogue start
    /// </summary>
    private void CreateWaitForSeconds()
    {
        _waitForNextDialogue = new WaitForSeconds(_nextDialogueWaitTime);
    }

    /// <summary>
    /// Adds a new dialogue line to the queue and, if necessary, starts the dialogue progression coroutine
    /// </summary>
    /// <param name="dialogue"> The dialogue to be added to the queue </param>
    private void EnqueueDialogue(ScriptableDialogueUi dialogue)
    {
        _dialogueQueue.Add(dialogue);

        if (_dialogueCoroutine.IsUnityNull())
        {
            _dialogueCoroutine = StartCoroutine(ProgressDialogue());
        }
    }

    /// <summary>
    /// Progresses the dialogue that is queued
    /// </summary>
    private IEnumerator ProgressDialogue()
    {
        while (_dialogueQueue.Count > 0)
        {
            // Pop the next dialogue line from the queue
            ScriptableDialogueUi currentDialogue = _dialogueQueue[0];
            _dialogueQueue.RemoveAt(0);

            // Play the line
            GameStateManager.Instance.GetOnDialogueProgress()?.Invoke(currentDialogue);

            // We would like to get a copy of the instance here for tracking
            // I would normally make this a private variable, but VS didn't like that when I tried it
            yield return new WaitUntil(CheckForReceivedInstance);

            // Wait until the line is finished playing
            FMOD.Studio.PLAYBACK_STATE state;
            do
            {
                _currentDialogueEventInstance.getPlaybackState(out state);
                yield return null;
            } while (state != FMOD.Studio.PLAYBACK_STATE.STOPPED);

            // Let's wait like a half second so that the pop up will complete
            // Sometimes that'll happen in-engine, no idea if it's the same in build
            yield return _waitForNextDialogue;

            // Release the instance
            _currentDialogueEventInstance.release();
        }

        // Reset the coroutine
        _dialogueCoroutine = null;
    }

    /// <summary>
    /// Checks to see if we have received an instance for the current dialogue
    /// </summary>
    /// <returns> True if the current event is valid, false otherwise </returns>
    private bool CheckForReceivedInstance()
    {
        return _currentDialogueEventInstance.isValid();
    }

    /// <summary>
    /// Sets the current dialogue event to keep track of it
    /// </summary>
    /// <param name="eventInstance"> The event instance to keep track of </param>
    public void SetCurrentDialogueEventInstance(EventInstance eventInstance)
    {
        _currentDialogueEventInstance = eventInstance;
    }

    #region BaseManager
    /// <summary>
    /// Establishes the instance for this manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        if (Instance.IsUnityNull())
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Performs needed set up by setting starting values
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        CreateWaitForSeconds();
    }

    /// <summary>
    /// Subscribes to events that take place in gameplay
    /// </summary>
    protected override void SubscribeToGameplayEvents()
    {
        base.SubscribeToGameplayEvents();
        GameStateManager.Instance.GetOnNewDialogueChain().AddListener(EnqueueDialogue);
    }

    /// <summary>
    /// Unsubscribes to events that take place in gameplay
    /// </summary>
    protected override void UnsubscribeToGameplayEvents()
    {
        base.UnsubscribeToGameplayEvents();
        GameStateManager.Instance.GetOnNewDialogueChain().RemoveListener(EnqueueDialogue);
    }
    #endregion
}
