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

    // References
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

    private void FixedUpdate()
    {
        PurgeEmptyCoroutines();

        StartPendingCoroutines();

        print(_pendingStoryBeats.Count);
        print(_activeStoryBeats.Count);
        print(_beatEventsCoroutines.Count);
    }

    private void StartPendingCoroutines()
    {
        for (int i = 0; i < _pendingStoryBeats.Count; i++)
        {
            StoryBeat beat = _pendingStoryBeats[i];

            // Start the coroutine on the specific story beat
            _beatEventsCoroutines.Add(StartCoroutine(nameof(TriggerBeatEvents), beat));
            _activeStoryBeats.Add(beat);

            _currentStoryIndex = StoryBeats.IndexOf(beat);
        }

        _pendingStoryBeats = new();
    }

    private void PurgeEmptyCoroutines()
    {
        List<int> indiciesToRemove = new();

        for (int i = 0; i < _beatEventsCoroutines.Count; i++)
        {
            if (_beatEventsCoroutines[i] == null)
            {
                indiciesToRemove.Add(i);
            }
        }

        for (int i = 0; i < indiciesToRemove.Count; i++)
        {
            _beatEventsCoroutines.RemoveAt(i);
            _activeStoryBeats.RemoveAt(i);
        }
    }

    private void OutdateAllCoroutines()
    {
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
        List<StoryBeatEvent> events = beat.StoryBeatEvents;
        _dialogueManager.StopDialogue();

        // Iterate through each event
        for (int i = 0; i < events.Count; i++)
        {
            float curDelayTime = events[i].DelayTime;

            // Trigger the event, then start a delay before triggering the next event
            events[i].TriggerEvent(beat.Outdated);

            while (curDelayTime > 0)
            {
                if (beat.Outdated)
                {
                    break;
                }

                curDelayTime -= Time.deltaTime;
                yield return null;
            }
        }

        _beatEventsCoroutines[_activeStoryBeats.IndexOf(beat)] = null;
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

    public bool Outdated = false;

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
    public void TriggerEvent(bool outdated = false)
    {
        // Based on the event type, do a specific action
        switch (EventType)
        {
            // If it's a dialogue event, start the dialogue
            case EBeatEventType.Dialogue:
                if (!outdated)
                {
                    GameStateManager.Instance.GetOnNewDialogueChain()?.Invoke(Dialogue);
                }
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