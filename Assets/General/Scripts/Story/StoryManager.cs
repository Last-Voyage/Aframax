/*****************************************************************************
// Name :           StoryManager.cs
// Author :         Charlie Polonus
// Created :        11/6/2024
// Description :    Handles the story beats and story beat events for during
                    the game
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryManager : MonoBehaviour
{
    // The singleton StoryManager
    public static StoryManager main;

    // The StoryBeat and coroutine system
    public List<StoryBeat> StoryBeats;
    private Coroutine _beatEventsCoroutine;

    // Editor settings
    public int OpenStoryBeat;

    /// <summary>
    /// Run initialization functions on the StoryManager
    /// </summary>
    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(this);
        }
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
            if (curBeat.Name.ToLower() == beatName.ToLower())
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
        WaitForSeconds postEventDelay;

        // Iterate through each event
        for (int i = 0; i < events.Count; i++)
        {
            // Trigger the event, then start a delay before triggering the next event
            events[i].TriggerEvent();
            postEventDelay = new(events[i].DelayTime);
            yield return postEventDelay;
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
    public string Name;

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
    public enum BeatEventType { Dialogue, BoatSpeed, BossAttack, Function }

    // Editor settings
    public bool Minimized;

    // General settings
    public string Name;
    public BeatEventType EventType;
    public float DelayTime;

    // Dialogue settings
    public DialogueLine DialogueLine;

    // Boat speed settings
    public float BoatSpeed;
    public float SpeedChangeTime;

    // Boss attack settings
    public List<BaseBossAttack> BossAttacks;

    // Function settings
    public UnityEvent BeatEvent;

    /// <summary>
    /// Start the story beat event
    /// </summary>
    public void TriggerEvent()
    {
        // Based on the event type, do a specific action
        switch (EventType)
        {
            // If it's a dialogue event, start the dialogue
            case BeatEventType.Dialogue:
                // TODO: Run dialogue
                // (DialogueManager.RunDialogue(DialogueLine);) as an example
                break;

            // If it's a boat speed event, change the speed of the boat
            case BeatEventType.BoatSpeed:
                Object.FindObjectOfType<BoatMover>().ChangeSpeed(BoatSpeed, SpeedChangeTime);
                break;

            // If it's a boss attack event, start the attacks of all the individual attackers
            case BeatEventType.BossAttack:
                foreach (BaseBossAttack currentAttack in BossAttacks)
                {
                    currentAttack.InvokeAttackBegin();
                }
                break;

            // If it's a UnityEvent event, invoke the UnityEvent
            case BeatEventType.Function:
                BeatEvent.Invoke();
                break;

            // If it's not an event that can exist, print a little smiley face
            default:
                Debug.Log(":P");
                break;
        }
    }
}