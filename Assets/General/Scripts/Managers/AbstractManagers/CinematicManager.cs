/*****************************************************************************
// File Name :         CinematicManager.cs
// Author :            Charlie Polonus
// Creation Date :     3/2/25
//
// Brief Description : Manages the different cutscenes that play throughout
                       the game
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CinematicManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private Canvas _cinematicCanvas;

    [Header("Settings")]
    [SerializeField] private Cinematic[] _cinematics;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _cinematicCanvas.enabled = _videoPlayer.isPlaying;
    }

    public void PlayCinematic(string cinematicName)
    {
        foreach (Cinematic curCinematic in _cinematics)
        {
            if (curCinematic.Name == cinematicName)
            {
                StartVideo(curCinematic.Clip);
                return;
            }
        }
    }

    private void StartVideo(VideoClip clip)
    {
        _videoPlayer.clip = clip;
        _videoPlayer.Play();
    }
}

[System.Serializable]
public struct Cinematic
{
    [SerializeField] private VideoClip _clip;
    [SerializeField] private string _name;

    public VideoClip Clip => _clip;
    public string Name => _name;
}
