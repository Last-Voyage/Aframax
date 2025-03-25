/**********************************************************************************************************************
// File Name :         HatchSceneTransition.cs
// Author :            Nick Rice
// Creation Date :     1/27/25
// 
// Brief Description : Changes the scene for the player when they interact with the hatch
**********************************************************************************************************************/

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Changes the scene for the player when they interact with the hatch
/// </summary>
public class HatchSceneTransition : MonoBehaviour
{
    [SerializeField] private int _sceneToLoadIndex;
    [SerializeField] private int _sceneTransitionPointer;
    public void EnterHatch()
    {
        AframaxSceneManager.Instance.StartAsyncSceneLoadViaID
            (_sceneToLoadIndex, _sceneTransitionPointer);
    }
}
