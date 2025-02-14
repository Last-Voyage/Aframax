/*****************************************************************************
// Name :           StoryBeatEvent.cs
// Author :         Charlie Polonus
// Contributer :    Ryan Swanson
// Created :        11/6/2024
// Description :    Handles specific events for each story beat
*****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    public ScriptableDialogueUi Dialogue;

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