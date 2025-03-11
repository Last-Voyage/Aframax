/*********************************************************************************************************************
// File Name :         FmodPersistentAudioEvents
// Author :            Andrea Swihart-DeCoster
// Contributors :      Ryan Swanson
// Creation Date :     10/23/24
//
// Brief Description : Stores all persistent sounds.
*********************************************************************************************************************/

using System.Collections;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Handler of random persistent sound effects at random times
/// </summary>
[System.Serializable]
public class IntervalFMODEvent
{
    [field:SerializeField] public int MinTimeBetweenEvents { get; private set; }
    [field:SerializeField] public int MaxTimeBetweenEvents { get; private set; }

    [Tooltip("All Sfx to be played")]
    [field: SerializeField]
    public EventReference IntervalEvent { get; private set; }

    public Coroutine IntervalCoroutine;
    
    /// <summary>
    /// The Coroutine to loops through the array with different timers per sound call
    /// </summary>
    /// <returns></returns>
    public IEnumerator RandomPersistentAudioLoop()
    {
        if (IntervalEvent.IsNull)
        {
            yield break;
        }
        
        while (true)
        {
            int interval = Random.Range(MinTimeBetweenEvents, MaxTimeBetweenEvents);
 
            yield return new WaitForSeconds(interval);
            
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(IntervalEvent, PlayerMovementController.Instance.transform.position);
        }
    }
}

/// <summary>
/// Stores all FMOD ambient / background / music audio events
/// </summary>
public class FmodPersistentAudioEvents : MonoBehaviour
{
    public static FmodPersistentAudioEvents Instance;

    [field: Header("Ambient Persistent Audio")]
    [field: Tooltip("Any audio added here will play throughout the full game")]
    [field: SerializeField] public EventReference[] PersistentGameBackgroundSounds { get; private set; }
    [field: SerializeField] public EventReference LimbIdle { get; private set; }
    
    [field: Header("Random Interval Looping Audio")]
    [field: Tooltip("Any audio added here will play throughout the full game at random intervals")]
    [field: SerializeField] public IntervalFMODEvent[] IntervalPersistentEvents { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference[] MusicInGame { get; private set; }
    [field: SerializeField] public float MusicFadeOutTime { get; private set; }
    [field: SerializeField] public float MusicFadeInTime { get; private set; }
    public void SetUpInstance()
    {
        Instance = this;
    }
}
