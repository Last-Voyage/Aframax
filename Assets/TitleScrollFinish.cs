/******************************************************************************
// File Name:       TitleScrollFinish.cs
// Author:          Jeremiah Peters
// Creation Date:   March 29, 2025
//
// Description:     enables controls when the title screen finishes scrolling
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleScrollFinish : StateMachineBehaviour
{
    [SerializeField] private GameObject _setUpPlayerControls;
    /// <summary>
    /// activates the event system, enabling controls
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _setUpPlayerControls = GameObject.Find("EventSystem");
        if (_setUpPlayerControls != null)
        {
            _setUpPlayerControls.GetComponent<EventSystem>().enabled = true;
        }
    }
}
