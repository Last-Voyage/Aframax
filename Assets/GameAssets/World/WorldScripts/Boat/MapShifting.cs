/*****************************************************************************
// File Name :         MapShifting.cs
// Author :            Mark Hanson
// Creation Date :     11/7/2024
//
// Brief Description : The array of different boat layouts that are accessible in other scripts
*****************************************************************************/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// The class holding the functionality of map shifting
/// </summary>
public class MapShifting : MonoBehaviour
{
    //A way of adding layouts into the scene outside of this script
    [Tooltip("Add all layouts of boat inside of here")]
    [SerializeField] private protected GameObject[] _boatLayouts;
    private protected int _currentLayout = 0;
    
    //For calling outside of script into another script
    private UnityEvent _onMapShifting = new();
    private UnityEvent _onMapReset = new();
    
    /// <summary>
    /// Allow for first layout to turn on if not on already
    /// </summary>
    void Start()
    {
        _boatLayouts[_currentLayout].SetActive(true);
        
        SubscribeToEvents();
    }
/// <summary>
/// THIS IS FOR TESTING ONLY
/// </summary>
    void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            LayoutSwapForward();
        }
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            LayoutSwapBackwards();
        }
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            LayoutSwapToOriginal();
        }
    }
    

    /// <summary>
    /// Turns off current layout of boat and goes to the next one
    /// </summary>
    private  void LayoutSwapForward()
    {
        if (_currentLayout < _boatLayouts.Length -1)
        {
            _boatLayouts[_currentLayout].SetActive(false);
        
            ++_currentLayout;
        
            _boatLayouts[_currentLayout].SetActive(true);
        }
    }
    
    /// <summary>
    /// Turns off current layout of boat and goes back one
    /// </summary>
    private void LayoutSwapBackwards()
    {
        if (_currentLayout > 0)
        {
            _boatLayouts[_currentLayout].SetActive(false);
        
            --_currentLayout;
        
            _boatLayouts[_currentLayout].SetActive(true);
        }
    }
    
    /// <summary>
    /// Turns off current layout of boat and goes all the way back to the
    /// Original
    /// </summary>
    private void LayoutSwapToOriginal()
    {
        _boatLayouts[_currentLayout].SetActive(false);
        
        _currentLayout = 0;
        
        _boatLayouts[_currentLayout].SetActive(true);
    }
    
    /// <summary>
    /// Attach functionality on to the events 
    /// </summary>
    private void SubscribeToEvents()
    {
        _onMapReset.AddListener(LayoutSwapToOriginal);
        _onMapShifting.AddListener(LayoutSwapForward);
    }
    
    /// <summary>
    /// Detach functionality away from the events
    /// </summary>
    private void OnDestroy()
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