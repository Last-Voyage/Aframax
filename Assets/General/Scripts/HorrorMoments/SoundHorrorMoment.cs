/*****************************************************************************
// File Name :         SoundHorrorMoment.cs
// Author :            Nabil Tagba
// Creation Date :     1/31/2024
//
// Brief Description : plays a certain sound to scare the player when they come
//in contact with the trigger
*****************************************************************************/
using UnityEngine;
using FMODUnity;
using System.Collections.Generic;

/// <summary>
/// plays a certain sound of the designers choice
/// scare the player when they come
//in contact with the trigger
/// </summary>
public class SoundHorrorMoment : MonoBehaviour
{
    private bool _hasPlayedHorrorSound = false;

    [SerializeField] private List<EventReference> _horrorMomentSounds;
    [SerializeField] private int _soundToUseIndex;
    /// <summary>
    /// happens when the player comes in contact with
    /// the collider
    /// </summary>
    /// <param name="other"> reference to the other object colided with</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !_hasPlayedHorrorSound) 
        {
            //play horror moment souund
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(_horrorMomentSounds[_soundToUseIndex], transform.position);
            _hasPlayedHorrorSound = true;
        }
    }
}
