/*****************************************************************************
// File Name :         PlayerBreathing.cs
// Author :            Ryan Swanson
// Creation Date :     4/2/2025
//
// Brief Description : Contains the functionality for switching between variable breathing levels
*****************************************************************************/

using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Contains the functionsality for switching breathing levels
/// </summary>
public class PlayerBreathing : MonoBehaviour
{
    [Tooltip("The level of intensity of the breathing. Capped at 3 as there are currently 4 levels")]
    [Range(0, 3)]
    private int _currentBreathingIntensity;

    private EventInstance[] _breathingInstances;
    private bool[] _storedBreathingLevels;

    private Coroutine _breathingCoroutine;
    private WaitForSeconds _breathingFadeIn;
    private WaitForSeconds _breathingFadeOut;

    /// <summary>
    /// Performs any needed set up before gameplay
    /// </summary>
    private void Start()
    {
        SetUpAudioInstances();
        SubscribeToEvents();
        //Activates the minimum breathing level
        AdjustStoredBreathingLevel(0, true);
    }

    /// <summary>
    /// Unsubscribed to events on destruction
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    /// <summary>
    /// Establishes the wait for seconds variables
    /// </summary>
    private void SetUpWaitForSeconds()
    {
        _breathingFadeIn = new WaitForSeconds(FmodPersistentAudioEvents.Instance.BreathingFadeInTime);
        _breathingFadeOut = new WaitForSeconds(FmodPersistentAudioEvents.Instance.BreathingFadeOutTime);
    }

    /// <summary>
    /// Creates all audio instances for the breathing
    /// </summary>
    private void SetUpAudioInstances()
    {
        //Creates a copy of the breathing levels array from the PersistentAudioEvents
        EventReference[] breathingReferences = FmodPersistentAudioEvents.Instance.BreathingLevels;
        
        _breathingInstances = new EventInstance[breathingReferences.Length];
        _storedBreathingLevels = new bool[breathingReferences.Length];

        for (int i = 0; i < breathingReferences.Length; i++)
        {
            _breathingInstances[i] = PersistentAudioManager.
                Instance.CreateInstanceFromReference(breathingReferences[i]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            AdjustStoredBreathingLevel(1,true);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            AdjustStoredBreathingLevel(2, true);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            AdjustStoredBreathingLevel(3, true);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            AdjustStoredBreathingLevel(1, false);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            AdjustStoredBreathingLevel(2, false);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            AdjustStoredBreathingLevel(3, false);
        }
    }

    private void PlayBreathingAtIntensity(int newIntensity)
    {
        if(newIntensity == _currentBreathingIntensity)
        {
            return;
        }

        if (_breathingCoroutine != null)
        {
            StopCoroutine(_breathingCoroutine);
        }

        _breathingCoroutine = StartCoroutine(SwitchBreathing(newIntensity));
    }

    /// <summary>
    /// The process of switching to a different breathing level
    /// </summary>
    /// <param name="newIntensity"> The new level to switch to </param>
    /// <returns></returns>
    private IEnumerator SwitchBreathing(int newIntensity)
    {
        PersistentAudioManager.Instance.FadeOutLoopingOneShot(
            _breathingInstances[_currentBreathingIntensity],
            FmodPersistentAudioEvents.Instance.BreathingFadeOutTime);

        yield return _breathingFadeIn;
        _currentBreathingIntensity = newIntensity;

        PersistentAudioManager.Instance.FadeInLoopingOneShot(
            _breathingInstances[_currentBreathingIntensity],
            FmodPersistentAudioEvents.Instance.BreathingFadeInTime);

        yield return _breathingFadeOut;

        print("Completed switch to " + newIntensity);
    }

    /// <summary>
    /// Determines which breathing level should be played
    /// </summary>
    private void DetermineBreathingLevel()
    {
        //Iterates through the breathing levels starting at the highest level and working down
        for(int i = _storedBreathingLevels.Length-1; i > 0; i--)
        {
            //If that breathing level is on
            if(_storedBreathingLevels[i])
            {
                //Switch to playing that breathing level
                PlayBreathingAtIntensity(i);
                //Leave the for loop as we have found the highest level
                return;
            }
        }
    }

    /// <summary>
    /// Adjusts if a breathing level is active or not
    /// </summary>
    /// <param name="intensity"> The breathing level intensity </param>
    /// <param name="levelActive"> If that breathing level intensity is active </param>
    private void AdjustStoredBreathingLevel(int intensity, bool levelActive)
    {
        _storedBreathingLevels[intensity] = levelActive;
        DetermineBreathingLevel();
    }

    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().
            AddListener(delegate { AdjustStoredBreathingLevel(1,true); });

        PlayerManager.Instance.GetOnHarpoonFocusEndEvent().
            AddListener(delegate { AdjustStoredBreathingLevel(1, false); });

        PlayerManager.Instance.GetOnHarpoonFiredEvent().
            AddListener(delegate { AdjustStoredBreathingLevel(1,false); });

        PersistentAudioManager.Instance.GetOnBossMusicStartedEvent().
            AddListener(delegate {AdjustStoredBreathingLevel(2,true); });

        PersistentAudioManager.Instance.GetOnBossMusicEndedEvent().
            AddListener(delegate { AdjustStoredBreathingLevel(2, false); });
    }

    private void UnsubscribeToEvents()
    {

    }
}
