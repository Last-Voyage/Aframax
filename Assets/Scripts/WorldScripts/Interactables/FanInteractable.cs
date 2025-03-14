/******************************************************************************
// File Name:       FanInteractable.cs
// Author:          Nick Rice
// Creation Date:   February 4th, 2025
//
// Description:     A fan oscillates back and forth, can be turned off, and plays 
//                  sfx
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A fan oscillates back and forth, can be turned off, and plays sfx
/// </summary>
public class FanInteractable : MonoBehaviour, IPlayerInteractable
{
    private IEnumerator turnTheFan;

    [SerializeField]
    private float _maxRightTurn;
    [SerializeField]
    private float _maxLeftTurn;

    private Vector3 _maxRightRotation;
    private Vector3 _maxLeftRotation;

    [SerializeField]
    private GameObject topFan;

    private Transform makeRotate;

    public float timerCounter = 0f;
    
    /// <summary>
    /// Grabs the transform of the turning part of the fan
    /// Initializes the vectors for where the fan is supposed to turn to
    /// </summary>
    private void Start()
    {
        makeRotate = topFan.transform;
        _maxRightRotation = new Vector3(0, _maxRightTurn, 0);
        _maxLeftRotation = new Vector3(0, _maxLeftTurn, 0);
    }

    /// <summary>
    /// Turns the fan on or off; inverses what it currently is
    /// </summary>
    public void OnInteractedByPlayer()
    {
        if (turnTheFan == null)
        {
            turnTheFan = FanTurning();
            StartCoroutine(turnTheFan);
        }
        else
        {
            StopCoroutine(turnTheFan);
            turnTheFan = null;
        }
    }

    /// <summary>
    /// This turns the fan
    /// </summary>
    private IEnumerator FanTurning()
    {
        while (true)
        {
            makeRotate.eulerAngles = 
                Vector3.Lerp(_maxRightRotation,_maxLeftRotation, (Mathf.Sin(.5f * timerCounter) +1)/2);
            timerCounter += Time.deltaTime;
            yield return null;
        }
    }
}
