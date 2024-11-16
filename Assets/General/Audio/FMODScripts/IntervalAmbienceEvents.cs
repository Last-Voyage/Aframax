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

namespace FMODUnity
{
    /// <summary>
    /// Handler of random ambient sound effects at random times
    /// </summary>
    [Serializable]
    public struct FModEventReference
    {

        public int MinTimeBetweenEvents { get; set; }
        public int MaxTimeBetweenEvents { get; set; }

        [Tooltip("All Sfx to be played")]
        [field: SerializeField]
        public FModEventReference[] FmodEventsArray { get; set; }

        /// <summary>
        /// The Coroutine to loops through the array with different timers per sound call
        /// </summary>
        /// <returns></returns>
        public IEnumerator RandomAmbienceLoop()
        {
            float timer = 0f;
            while (true)
            {
                for (int i = 0; i < FmodEventsArray.Length; i++)
                {
                    if (timer > Random.Range(MinTimeBetweenEvents, MaxTimeBetweenEvents))
                    {

                        timer = 0f;
                    }

                    timer += Time.deltaTime;
                    yield return null;
                }
            }
        }
    }
}
