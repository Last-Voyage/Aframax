/*****************************************************************************
// Name :           StoryManager.cs
// Author :         Charlie Polonus
// Contributer :    Ryan Swanson
// Created :        11/6/2024
// Description :    Handles the story beats and story beat events for during
                    the game
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The manager for all the story beats and story events
/// </summary>
public class StoryManager : MonoBehaviour
{
    // The singleton StoryManager
    public static StoryManager Instance;

    // The StoryBeat and coroutine system
    public List<StoryBeat> StoryBeats;
    private Coroutine _beatEventsCoroutine;

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
        print("I am now on act " + _currentStoryIndex);
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

        // Start the coroutine on the specific story beat
        _beatEventsCoroutine = StartCoroutine(nameof(TriggerBeatEvents), beat.StoryBeatEvents);

        _currentStoryIndex = StoryBeats.IndexOf(beat);
    }

    /// <summary>
    /// Trigger a story beat by index
    /// </summary>
    /// <param name="index">The index of the story beat</param>
    public void TriggerStoryBeat(int index)
    {
        // Edge case if the index is out of bounds
        if (index < 0 || index >= StoryBeats.Count)
        {
            return;
        }

        // Start the coroutine on the specific story beat
        _beatEventsCoroutine = StartCoroutine(nameof(TriggerBeatEvents), StoryBeats[index].StoryBeatEvents);

        _currentStoryIndex = index;
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
                // Start the coroutine on the specific story beat
                _beatEventsCoroutine = StartCoroutine(nameof(TriggerBeatEvents), curBeat.StoryBeatEvents);
                return;
            }
        }
    }

    /// <summary>
    /// Start a specific story beat's events over time
    /// </summary>
    /// <param name="events">The events to run</param>
    /// <returns></returns>
    private IEnumerator TriggerBeatEvents(List<StoryBeatEvent> events)
    {
        // Iterate through each event
        for (int i = 0; i < events.Count; i++)
        {
            // Trigger the event, then start a delay before triggering the next event
            events[i].TriggerEvent();
            yield return new WaitForSeconds(events[i].DelayTime);
        }
    }
}

/// <summary>
/// An individual story beat
/// </summary>
[System.Serializable]
public class StoryBeat
{
    // The name of the story beat
    public string BeatName;
    public string BeatDescription;
    public bool TriggerOnStart;

    // The list of story beat events in the beat
    public List<StoryBeatEvent> StoryBeatEvents;
}

/// <summary>
/// An individual story beat event
/// </summary>
[System.Serializable]
public class StoryBeatEvent
{
    // Enumerable for setting the type of beat event
    public enum EBeatEventType { Dialogue, BoatSpeed, BossAttack, Function }

    // Editor settings
    public bool IsMinimized;

    // General settings
    public string EventName;
    public EBeatEventType EventType;
    public float DelayTime;

    // Dialogue settings
    public ScriptableDialogueUI Dialogue;

    // Boat speed settings
    public float BoatSpeed;
    public float SpeedChangeTime;

    // Boss attack settings
    public List<BaseBossAttack> BossAttacks;

    // Function settings
    public UnityEvent OnBeatEvent;

    /// <summary>
    /// Start the story beat event
    /// </summary>
    public void TriggerEvent()
    {
        // Based on the event type, do a specific action
        switch (EventType)
        {
            // If it's a dialogue event, start the dialogue
            case EBeatEventType.Dialogue:
                GameStateManager.Instance.GetOnNewDialogueChain()?.Invoke(Dialogue);
                break;

            // If it's a boat speed event, change the speed of the boat
            case EBeatEventType.BoatSpeed:
                BoatMover.Instance.ChangeSpeed(BoatSpeed, SpeedChangeTime);
                break;

            // If it's a boss attack event, start the attacks of all the individual attackers
            case EBeatEventType.BossAttack:
                foreach (BaseBossAttack currentAttack in BossAttacks)
                {
                    currentAttack.InvokeAttackBegin();
                }
                break;

            // If it's a UnityEvent event, invoke the UnityEvent
            case EBeatEventType.Function:
                OnBeatEvent?.Invoke();
                break;

            // If it's not a predefined type, send an error line
            default:
                Debug.LogError("Unable to read the event type " + EventType.ToString());
                break;
        }
    }
}