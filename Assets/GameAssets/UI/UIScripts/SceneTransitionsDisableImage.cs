/******************************************************************************
// File Name:       SceneTransitionDisableImage.cs
// Author:          Jeremiah Peters
// Creation Date:   February 21, 2025
//
// Description:     turns off the scene transition image component when transition is finished
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionDisableImage : StateMachineBehaviour
{
    /// <summary>
    /// disables image component 
    /// for some reason, an image component with no image defaults to a solid white square
    /// by disabling the image component, the object can remain enabled and not block buttons
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SceneTransitionBehaviour.Instance.GetComponent<Image>().enabled = false;
    }
}
