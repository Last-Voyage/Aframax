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

public class ButtonSFXManager : MonoBehaviour
{
    //the index of sfx to use
    [SerializeField] private int _clickSFXIndex;
    /// <summary>
    /// plays the designated button sfx (currently there is only one)
    /// </summary>
    public void PlayClickSFX()
    {
        RuntimeSfxManager.APlayOneShotSfx?.Invoke(FmodSfxEvents.Instance._buttonUsedSFX[_clickSFXIndex], Vector3.zero);
    }
}
