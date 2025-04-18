/**********************************************************************************************************************
// File Name :          FakeWindowObject.cs
// Author :             Miles Rogers
// Creation Date :      4/15/2025
//
// Brief description :  Attach this script to objects that need to render fake windows. It ensures the existence
//                      of the FakeWindowManager context and updates the relevant values in the window mesh's
//                      material.
**********************************************************************************************************************/

using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Script for objects that need to render fake windows. Ensures the existence of the FakeWindowManager
/// context and updates the relevant values in the window mesh's material.
/// </summary>
public class FakeWindowObject : MonoBehaviour
{
    /// <summary>
    /// Reference to the FakeWindowManager global singleton
    /// </summary>
    private FakeWindowManager _fakeWindowManager;

    /// <summary>
    /// Ensure existence of FakeWindowManager, which spawns
    /// the capture container
    /// </summary>
    private void OnEnable()
    {
        _fakeWindowManager = FakeWindowManager.Instance;
        Debug.Assert(!_fakeWindowManager.IsUnityNull());
        _fakeWindowManager.SetCurrentFakeWindow(this);
    }
}
