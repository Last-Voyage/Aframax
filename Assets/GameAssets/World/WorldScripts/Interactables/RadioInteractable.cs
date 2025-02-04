/******************************************************************************
// File Name:       RadioInteractable.cs
// Author:          Nick Rice
// Creation Date:   February 4th, 2025
//
// Description:     
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class RadioInteractable : MonoBehaviour, IPlayerInteractable
{
    private RuntimeSfxManager _sfxManager = RuntimeSfxManager.Instance;
    private Action<EventReference, Vector3> radioSong;
    private WaitForSeconds _sfxTotalTime;
    private EventReference _actualSong;

    private void Start()
    {
        radioSong = RuntimeSfxManager.APlayOneShotSfx;

        _actualSong = FmodSfxEvents.Instance.PlayerHeartBeat;
        
        //float tempTime = ;
        //radioSong = new Action<EventReference, Vector3>(_actualSong, gameObject.transform.position);
        //_sfxTotalTime = new WaitForSeconds();
    }

    private IEnumerator _makeRadioPlay;
    
    public void OnInteractedByPlayer()
    {
        if (_makeRadioPlay.IsUnityNull())
        {
            _makeRadioPlay = PlayRadio();
            StartCoroutine(_makeRadioPlay);
            
        }
    }

    private IEnumerator PlayRadio()
    {
        while (true)
        {
            //_sfxManager.
            yield return null;
        }
        
        _makeRadioPlay = null;
    }
}
