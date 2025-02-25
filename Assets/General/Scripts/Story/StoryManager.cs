/*****************************************************************************
// Name :           StoryManager.cs
// Author :         Charlie Polonus
// Contributer :    Ryan Swanson
// Created :        11/6/2024
// Description :    Handles the story beats and story beat events for during
                    the game
*****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager for all the story beats and story events
/// </summary>
public class StoryManager : MonoBehaviour
{
    // The singleton StoryManager
    public static StoryManager Instance;

    // References to other scripts that will be used
    private DialoguePopUps _dialogueManager;

    // The StoryBeat and coroutine system
    public List<StoryBeat> StoryBeats;
    private List<Coroutine> _beatEventsCoroutines;
    private List<StoryBeat> _activeStoryBeats;
    private List<StoryBeat> _pendingStoryBeats;

    // Editor settings
    public int OpenStoryBeat;

    // The current index of the story we are on. Begins at -1 as the first story index is 0. So -1 is before any beats
    private int _currentStoryIndex = -1;

    /// <summary>
    /// Run initialization functions on the StoryManager
    /// </summary>
    private void Awake()
    {
        DefineSingleton();
    }

    /// <summary>
    /// Defines the Instance singleton variable
    /// </summary>
    private void DefineSingleton()
    {
        // Only sets it as the singleton if there isn't one already
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Plays as soon as Unity starts playing
    /// </summary>
    private void Start()
    {
        // Set up the reference for the dialogue manager
        _dialogueManager = FindObjectOfType<DialoguePopUps>();

        _beatEventsCoroutines = new();
        _activeStoryBeats = new();
        _pendingStoryBeats = new();

        // Run through each story beat, triggering the first one that is set to play on start
        foreach (StoryBeat curBeat in StoryBeats)
        {
            if (curBeat.TriggerOnStart)
            {
                TriggerStoryBeat(curBeat);
                return;
            }
        }
    }

    /// <summary>
    /// Handle any existing and unnecessary or pending coroutines
    /// </summary>
    private void FixedUpdate()
    {
        PurgeEmptyCoroutines();

        StartPendingCoroutines();
    }

    /// <summary>
    /// Check the pending list and start any ready to go beats
    /// </summary>
    private void StartPendingCoroutines()
    {
        // Run through each beat and check its status
        for (int i = 0; i < _pendingStoryBeats.Count; i++)
        {
            StoryBeat beat = _pendingStoryBeats[i];

            // Start the coroutine on the specific story beat
            _beatEventsCoroutines.Add(StartCoroutine(nameof(TriggerBeatEvents), beat));
            _activeStoryBeats.Add(beat);

            _currentStoryIndex = StoryBeats.IndexOf(beat);
        }

        // Purge the pending story beat list
        _pendingStoryBeats = new();
    }

    /// <summary>
    /// Check for any empty coroutines, and delete any that say they're empty
    /// </summary>
    private void PurgeEmptyCoroutines()
    {
        // Make a list for all the beats to delete
        List<int> indiciesToRemove = new();

        // Add to the list each index that had a nonexistant coroutine
        for (int i = 0; i < _beatEventsCoroutines.Count; i++)
        {
            if (_beatEventsCoroutines[i] == null)
            {
                indiciesToRemove.Add(i);
            }
        }

        // Remove each coroutine that was marked as null
        for (int i = 0; i < indiciesToRemove.Count; i++)
        {
            _beatEventsCoroutines.RemoveAt(i);
            _activeStoryBeats.RemoveAt(i);
        }
    }

    /// <summary>
    /// Set all currently active beats to "outdated"
    /// </summary>
    private void OutdateAllCoroutines()
    {
        // For each active beat, tell it to set itself as outdated
        for (int i = 0; i < _beatEventsCoroutines.Count; i++)
        {
            _activeStoryBeats[i].Outdated = true;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// FOR TESTING ONLY
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            ProgressNextStoryBeat();
        }
    }
#endif

    /// <summary>
    /// Moves to the next beat index in order
    /// </summary>
    public void ProgressNextStoryBeat()
    {
        TriggerStoryBeat(++_currentStoryIndex);
    }

    /// <summary>
    /// Trigger a story beat by object
    /// </summary>
    /// <param name="beat">The index of the story beat</param>
    public void TriggerStoryBeat(StoryBeat beat)
    {
        // Edge case if the beat doesn't exist
        if (beat == null)
        {
            return;
        }

        OutdateAllCoroutines();

        _pendingStoryBeats.Add(beat);
    }

    /// <summary>
    /// Trigger a story beat by index
    /// </summary>
    /// <param name="index">The index of the story beat</param>
    public void TriggerStoryBeat(int index)
    {
        TriggerStoryBeat(StoryBeats[index]);
    }

    /// <summary>
    /// Start a story beat by name
    /// </summary>
    /// <param name="beatName">The name of the story beat</param>
    public void TriggerStoryBeat(string beatName)
    {
        // Iterate through each story beat, activating the one that matches the name
        foreach (StoryBeat curBeat in StoryBeats)
        {
            // Disregard case of the names
            if (curBeat.BeatName.ToLower() == beatName.ToLower())
            {
                TriggerStoryBeat(curBeat);
                return;
            }
        }
    }

    /// <summary>
    /// Start a specific story beat's events over time
    /// </summary>
    /// <param name="events">The events to run</param>
    /// <returns></returns>
    private IEnumerator TriggerBeatEvents(StoryBeat beat)
    {
        // Stop dialogue from playing if there's already a beat playing dialogue
        _dialogueManager.StopDialogue();

        List<StoryBeatEvent> events = beat.StoryBeatEvents;

        // Iterate through each event
        for (int i = 0; i < events.Count; i++)
        {
            float curDelayTime = events[i].DelayTime;

            // Trigger the event, then start a delay before triggering the next event
            events[i].TriggerEvent(beat.Outdated);

            // Run through the events of the beat
            while (curDelayTime > 0)
            {
                // If at any point the beat is considered outdated, immediately flash through every event at once
                if (beat.Outdated)
                {
                    break;
                }

                curDelayTime -= Time.deltaTime;
                yield return null;
            }
        }

        // Tell the active beat list to stop considering this beat as playing
        _beatEventsCoroutines[_activeStoryBeats.IndexOf(beat)] = null;
    }

    /// <summary>
    /// This saves the current story beat when a new checkpoint is hit
    /// </summary>
    private void SaveData()
    {
        SaveManager.Instance.GetGameSaveData().SetCurrentStoryBeat(_currentStoryIndex);
    }

    private void LoadData()
    {
        _currentStoryIndex = SaveManager.Instance.GetGameSaveData().GetCurrentStoryBeat();
    }

    /// <summary>
    /// This will save data when a new checkpoint is reached; and it will load in save data
    /// (when the player returns to a scene they died in or left)
    /// </summary>
    private void OnEnable()
    {
        SaveManager.Instance.GetOnNewCheckpoint()?.AddListener(SaveData);
        SaveManager.Instance.GetOnLoadSaveData()?.AddListener(LoadData);
    }

    /// <summary>
    /// Disables listeners to prevent memory leaks
    /// </summary>
    private void OnDisable()
    {
        SaveManager.Instance.GetOnNewCheckpoint()?.RemoveListener(SaveData);
        SaveManager.Instance.GetOnLoadSaveData()?.RemoveListener(LoadData);
    }
}
