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
using UnityEngine;
using UnityEngine.Serialization;

public class FanInteractable : MonoBehaviour, IPlayerInteractable
{
    private IEnumerator turnTheFan;

    [SerializeField]
    private float _maxRightTurn;
    [SerializeField]
    private float _maxLeftTurn;

    private Transform _maxRightFanPosition;
    private Transform _maxLeftFanPosition;

    private Quaternion _newMaxRightFanPos = new Quaternion();
    private Quaternion _newMaxLeftFanPos = new Quaternion();

    [SerializeField]
    private GameObject topFan;

    private Transform makeRotate;
    
    private void Start()
    {
        makeRotate = topFan.transform;
        _maxRightFanPosition = makeRotate;
        _maxLeftFanPosition = makeRotate;

        _newMaxRightFanPos.eulerAngles = new Vector3(0,_maxRightTurn,0);
        _newMaxLeftFanPos.eulerAngles = new Vector3(0,_maxLeftTurn,0);
        
        //_maxLeftFanPosition.eulerAngles = new Vector3(0,_maxLeftTurn,0);
        //_maxRightFanPosition.eulerAngles = new Vector3(0,_maxRightTurn,0);

        makeRotate.rotation = _newMaxLeftFanPos;
        Debug.Log(makeRotate.rotation);
    }

    private void Update()
    {
        makeRotate.rotation = Quaternion.Lerp(_newMaxRightFanPos, _newMaxLeftFanPos, .5f);
    }

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

    private IEnumerator FanTurning()
    {
        while (true)
        {
            //Quaternion.Euler(topFan.transform.rotation);
            
            //topFan.transform.rotation.y = Mathf.Sin();
            yield return null;
        }
        
    }
}
