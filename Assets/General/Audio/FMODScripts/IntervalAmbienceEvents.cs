/******************************************************************************
// File Name:       IntervalAmbienceEvents.cs
// Author:          Mark Hanson
// Creation Date:   11/14/2024
//
// Description:     Handler of random ambient sound effects at random times
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Random = UnityEngine.Random;
/// <summary>
/// Handler of random ambient sound effects at random times
/// </summary>
public class IntervalAmbienceEvents : MonoBehaviour
{
  /// <summary>
  /// Struct to hold min and max time for random range
  /// </summary>
    public struct FModEventReference
    {
        public int MinTimeBetweenEvents;
        public int MaxTimeBetweenEvents;
    }

    public FModEventReference FmodEvents { get; set; }
   [SerializeField] private EventReference[] _ambienceArray;

   private Coroutine _ambienceCoroutine;
   /// <summary>
   /// Start coroutine 
   /// </summary>
   private void Start()
   {
       if (_ambienceCoroutine == null)
       {
           StartCoroutine(PlayIntervalAudio());  
       }
   }
/// <summary>
/// The Coroutine to loop through the array with different timers per sound call
/// </summary>
/// <returns></returns>
   private IEnumerator PlayIntervalAudio()
    {
        while (true)
        {
            float Timer = 0.0f;
            for (int i = 0; i < _ambienceArray.Length; i++)
            {
                if (Timer > Random.Range(FmodEvents.MinTimeBetweenEvents, FmodEvents.MaxTimeBetweenEvents))
                {
                    RuntimeSfxManager.APlayOneShotSfx?
                        .Invoke(_ambienceArray[i], gameObject.transform.position);

                }
                Timer += Time.deltaTime;
            }
        }
    }
}
