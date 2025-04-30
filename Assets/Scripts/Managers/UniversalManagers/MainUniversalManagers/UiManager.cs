/******************************************************************************
// File Name:       UiManager.cs
// Author:          Nick Rice
// Creation Date:   April 14, 2025
//
// Description:     Contains the functionality to set up and get access to Ui changes
******************************************************************************/

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Contains the functionality to set up and get access to Ui changes
/// </summary>
public class UiManager : MainUniversalManagerFramework
{
    public static UiManager Instance;

    private static bool _isUsingController;
    private readonly UnityEvent _onSwapInput = new();

    private Stack<GameObject> _previousUiSelections = new ();
    public Stack<Button> _backButtons = new();
    private PlayerInputMap _playerInput;

    /// <summary>
    /// Makes the controller toggle save between game sessions
    /// </summary>
    private void Awake()
    {
        _isUsingController = SaveManager.Instance.GetGameSaveData().IsUsingController;
        _playerInput = new PlayerInputMap();
        _backButtons.Clear();
        _previousUiSelections.Clear();
    }
    
    #region Back Button

    /// <summary>
    /// This will add another button onto the stack, and add a listener if there are no other buttons on the stack
    /// The listener is for leaving the current UI screen
    /// </summary>
    /// <param name="button"></param>
    public void AddToBackStack(Button button)
    {
        if (IsBackButtonStackEmpty())
        {
            _playerInput.Player.UIBack.Enable();
            _playerInput.Player.UIBack.performed += ctx => ActivateBackButton();
        }
        _backButtons.Push(button);
    }

    /// <summary>
    /// This will activate the back button, and remove the listener if there are none left
    /// </summary>
    public void ActivateBackButton()
    {
        if (_backButtons.TryPop(out Button button))
        {
            button.onClick.Invoke();
        }

        if (IsBackButtonStackEmpty())
        {
            _playerInput.Player.UIBack.performed -= ctx => ActivateBackButton();
        }
    }

    /// <summary>
    /// This checks if there are any back buttons on the stack
    /// </summary>
    /// <returns></returns>
    private bool IsBackButtonStackEmpty()
    {
        return _backButtons.Count == 0;
    }
    
    #endregion

    #region Previously Selected UI

    /// <summary>
    /// Saves the previously selected ui element to a stack
    /// </summary>
    /// <param name="uiElement">The ui element added to the stack</param>
    public void AddToSelectionStack(GameObject uiElement)
    {
        _previousUiSelections.Push(uiElement); 
    }

    /// <summary>
    /// Sets the current selected ui to what was last selected on the previous page
    /// </summary>
    public void SelectUiOnPreviousPage()
    {
        if (_previousUiSelections.TryPop(out GameObject uiElement))
        {
            EventSystem.current.SetSelectedGameObject(uiElement);
        }
    }

    #endregion
    /// <summary>
    /// This sends out the event to change the current Ui used in game when a controller is used
    /// For the time being the main usage of this is for changing a boolean
    /// </summary>
    public void SwapInput()
    {
        _isUsingController = !_isUsingController;
        SaveManager.Instance.GetGameSaveData().IsUsingController = _isUsingController;
        _onSwapInput?.Invoke();
    }

    /// <summary>
    /// If the player still has back buttons, then this will remove the UIback listener
    /// </summary>
    private void OnDisable()
    {
        if (!_playerInput.Player.IsUnityNull() && !IsBackButtonStackEmpty())
        {
            _playerInput.Player.UIBack.performed -= ctx => ActivateBackButton();
        }
    }

    #region BaseManager
    /// <summary>
    /// Establishes the instance for the save manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    #endregion

    #region Getters

    public static bool IsUsingController => _isUsingController;

    public UnityEvent GetOnSwapInput => _onSwapInput;

    #endregion
}
