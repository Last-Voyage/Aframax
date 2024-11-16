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
using FMOD.Studio;
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

        [field:SerializeField] public  int MinTimeBetweenEvents { get; set; }
        [field:SerializeField] public int MaxTimeBetweenEvents { get; set; }

        [Tooltip("All Sfx to be played")]
        [field: SerializeField]
        public EventReference[] FmodEvents { get; set; }
        public Coroutine PlaySfxCoroutine { get; set; }
    }
}
