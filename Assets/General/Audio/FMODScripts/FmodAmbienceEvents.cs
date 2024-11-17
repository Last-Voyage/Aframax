/*********************************************************************************************************************
// File Name :         FmodAmbienceEvents
// Author :            Andrea Swihart-DeCoster
// Contributors :      Ryan Swanson
// Creation Date :     10/23/24
//
// Brief Description : Stores all ambient sounds.
*********************************************************************************************************************/

using System.Collections;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Handler of random ambient sound effects at random times
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
    public IEnumerator RandomAmbienceLoop()
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
/// Stores all FMOD ambient / background audio events
/// </summary>
public class FmodAmbienceEvents : MonoBehaviour
{
    public static FmodAmbienceEvents Instance;

    [field: Header("Ambient Background Audio")]
    [field: Tooltip("Any audio added here will play throughout the full game")]
    [field: SerializeField] public EventReference[] AmbientGameBackgroundSounds { get; private set; }
    [field: SerializeField] public EventReference LimbIdle { get; private set; }
    
    [field: Header("Random Interval Ambience")]
    [field: Tooltip("Any audio added here will play throughout the full game at random intervals")]
    [field: SerializeField] public IntervalFMODEvent[] IntervalAmbientEvents { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(this);
        }
    }
}
