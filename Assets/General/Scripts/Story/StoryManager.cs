using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class StoryManager : MonoBehaviour
{
    public static StoryManager main;

    [Header("References")]
    [SerializeField] private TMP_Text _dialogueTextField;

    [Header("Settings")]
    public List<StoryBeat> StoryBeats;
    private Coroutine _beatEventsCoroutine;

    [Header("Temporary")]
    public List<UnityEvent> TempEvents;

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

    public void TriggerStoryBeat(int index)
    {
        _beatEventsCoroutine = StartCoroutine(nameof(TriggerBeatEvents), StoryBeats[index].StoryBeatEvents);
    }
    public void TriggerStoryBeat(string beatName)
    {
        foreach (StoryBeat curBeat in StoryBeats)
        {
            if (curBeat.Name.ToLower() == beatName.ToLower())
            {
                _beatEventsCoroutine = StartCoroutine(nameof(TriggerBeatEvents), curBeat.StoryBeatEvents);
                return;
            }
        }
    }

    private IEnumerator TriggerBeatEvents(List<StoryBeatEvent> events)
    {
        WaitForSeconds postEventDelay;

        for (int i = 0; i < events.Count; i++)
        {
            events[i].TriggerEvent();
            postEventDelay = new(events[i].DelayTime);
            yield return postEventDelay;
        }
    }
}

[System.Serializable]
public class StoryBeat
{
    public string Name;

    public List<StoryBeatEvent> StoryBeatEvents;
}

[System.Serializable]
public class StoryBeatEvent
{
    public enum BeatEventType { Dialogue, BoatSpeed, BossAttack, Function }

    [Header("Generic Variables")]
    public BeatEventType EventType;
    public float DelayTime;

    [Header("Dialogue Variables")]
    public DialogueLine DialogueLine;
    //public List<DialogueLine> DialogueLines = new();
    //public bool IsShuffled;
    //public bool IsLoop;
    //public float MinimumWaitTime;
    //public float MaximumWaitTime;

    [Header("Boat Speed Variables")]
    public float BoatSpeed;
    public float SpeedChangeTime;

    public List<BaseBossAttack> BossAttacks;

    [Header("Function Variables")]
    public UnityEvent BeatEvent;

    public void TriggerEvent()
    {
        switch (EventType)
        {
            case BeatEventType.Dialogue:

                // TODO: Run dialogue
                // (DialogueManager.RunDialogue(DialogueLine);) as an example
                
                break;
            case BeatEventType.BoatSpeed:
                
                Object.FindObjectOfType<BoatMover>().ChangeSpeed(BoatSpeed, SpeedChangeTime);
                
                break;
            case BeatEventType.BossAttack:

                foreach (BaseBossAttack currentAttack in BossAttacks)
                {
                    currentAttack.InvokeAttackBegin();
                }

                break;
            case BeatEventType.Function:
                
                BeatEvent.Invoke();
                
                break;
            default:
                
                break;
        }
    }
}