/*****************************************************************************
// File Name :         TutorialPopUp.cs
// Author :            Charlie Polonus
// Creation Date :     3/2/25
//
// Brief Description : Controls a tutorial pop up in-engine.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class TutorialPopUp : NoteInteractable
{
    [Header("References")]
    [SerializeField] private VideoPlayer _video;
    [SerializeField] private GameObject[] _pages;
    private int _activePage;

    // Start is called before the first frame update
    void Awake()
    {
        _video.targetCamera = Camera.current;
        _activePage = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
