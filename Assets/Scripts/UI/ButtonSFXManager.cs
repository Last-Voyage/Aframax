/******************************************************************************
// File Name:       ButtonSFXManager.cs
// Author:          Jeremiah Peters
// Creation Date:   4/14/2025
//
// Description:     Enables menu buttons to play sound effects
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// used for buttons to play sfx
/// </summary>
public class ButtonSFXManager : MonoBehaviour
{
    //the index of sfx to use
    [SerializeField] private int _clickSFXIndex;

    //used to avoid settings buttons making noise when initializing
    private float _startTime;

    /// <summary>
    /// plays the designated button sfx (currently there is only one)
    /// changed to not work if startTime = 0
    /// this will stop any objects that aren't enabled yet from playing sfx
    /// </summary>
    public void PlayClickSFX()
    {
        if (_startTime != 0)
        {
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(FmodSfxEvents.Instance._buttonUsedSFX[_clickSFXIndex], Vector3.zero);
        }
    }

    /// <summary>
    /// for objects on the title screen that need to play click sfx
    /// </summary>
    public void AlwaysPlayClickSFX()
    {
        RuntimeSfxManager.APlayOneShotSfx?.Invoke(FmodSfxEvents.Instance._buttonUsedSFX[_clickSFXIndex], Vector3.zero);
    }

    private void OnEnable()
    {
        _startTime = Time.time;
    }
}
