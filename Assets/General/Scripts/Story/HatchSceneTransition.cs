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

public class HatchSceneTransition : MonoBehaviour, IPlayerInteractable
{
    [SerializeField] private int _sceneTransitionPointer;
    public void OnInteractedByPlayer()
    {
        Debug.Log(AframaxSceneManager.Instance.MazeSceneIndex);
        AframaxSceneManager.Instance.StartAsyncSceneLoadViaID
            (AframaxSceneManager.Instance.MazeSceneIndex, _sceneTransitionPointer);
    }
}
