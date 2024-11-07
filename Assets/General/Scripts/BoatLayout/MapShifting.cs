/*****************************************************************************
// File Name :         MapShifting.cs
// Author :            Mark Hanson
// Creation Date :     11/7/2024
//
// Brief Description : The array of different boat layouts that are accessible in other scripts
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapShifting : MonoBehaviour
{
    //A way of adding layouts into the scene outside of this script
    [Tooltip("Add all layouts of boat inside of here")]
    [SerializeField]private protected GameObject[] _listOfLayouts;
    private protected int _currentLayout = 0;
    //For calling outside of script into another script
    private UnityEvent _onMapShifting = new();
    private UnityEvent _onMapReset = new();
    /// <summary>
    /// Allow for first layout to turn on if not on already
    /// </summary>
    void Start()
    {
        _listOfLayouts[_currentLayout].SetActive(true);
        Subscribe();
    }
    /// <summary>
    /// Turns off current layout of boat and goes to the next one
    /// </summary>
    private protected void LayoutSwapForward()
    {
        _listOfLayouts[_currentLayout].SetActive(false);
        ++_currentLayout;
        _listOfLayouts[_currentLayout].SetActive(true);
    }
    /// <summary>
    /// Turns off current layout of boat and goes all the way back to the
    /// Original
    /// </summary>
    private protected void LayoutSwapToOriginal()
    {
        _listOfLayouts[_currentLayout].SetActive(false);
        _currentLayout = 0;
        _listOfLayouts[_currentLayout].SetActive(true);
    }
    /// <summary>
    /// Attach functionality on to the events 
    /// </summary>
    protected virtual void Subscribe()
    {
        _onMapReset.AddListener(LayoutSwapToOriginal);
        _onMapShifting.AddListener(LayoutSwapForward);
    }
    /// <summary>
    /// Detach functionality away from the events
    /// </summary>
    protected virtual void Unsubscribe()
    {
        _onMapReset.RemoveListener(LayoutSwapToOriginal);
        _onMapShifting.RemoveListener(LayoutSwapForward);
    }
    #region Events
/// <summary>
/// For map shifting to be used outside this script
/// </summary>
    private void InvokeOnMapShifting()
    {
        _onMapShifting?.Invoke();
    }
/// <summary>
///  For map resetting to be used outside this script
/// </summary>
    private void InvokeOnMapReset()
    {
        _onMapReset?.Invoke();
    }
    #endregion
    #region Getters
    public UnityEvent OnMapShifting => _onMapShifting;
    public UnityEvent OnMapReset => _onMapReset;
    #endregion
}
