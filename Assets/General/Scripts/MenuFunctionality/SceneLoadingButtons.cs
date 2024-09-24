using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality to buttons to load scenes
/// </summary>
public class SceneLoadingButtons : MonoBehaviour
{
    [SerializeField] private int _sceneToLoad;
    [SerializeField] private int _sceneTransitionID;

    /// <summary>
    /// The function called by the button to load the desired scene
    /// </summary>
    public void LoadDesiredScene()
    {
        SceneLoadingManager.Instance.StartAsyncSceneLoadViaID(_sceneToLoad, _sceneTransitionID);
    }
}
