/******************************************************************************
// File Name:       UiManager.cs
// Author:          Nick Rice
// Creation Date:   April 14, 2025
//
// Description:     Contains the functionality to set up and get access to Ui changes
******************************************************************************/

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [SerializeField] private bool _shouldMouseAppearUsingController;

    /// <summary>
    /// Makes the controller toggle save between game sessions
    /// </summary>
    private void Awake()
    {
        _isUsingController = SaveManager.Instance.GetGameSaveData().IsUsingController;
        _playerInput = new PlayerInputMap();
        _playerInput.Player.UIBack.Enable();
        _playerInput.Player.UIBack.performed += ActivateBackButton;
        _backButtons.Clear();
        _previousUiSelections.Clear();

        // Let's make sure that the mouse is hidden when you start the game again
        if (_isUsingController)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    #region Back Button

    /// <summary>
    /// This will add another button onto the stack, and add a listener if there are no other buttons on the stack
    /// The listener is for leaving the current UI screen
    /// </summary>
    /// <param name="button"></param>
    public void AddToBackStack(Button button)
    {
        _backButtons.Push(button);
    }

    /// <summary>
    /// This will activate the back button, and remove the listener if there are none left
    /// </summary>
    public void ActivateBackButton(InputAction.CallbackContext ctx)
    {
        if (IsBackButtonStackEmpty() && !PauseMenu.Instance.IsUnityNull() && TimeManager.Instance.GetIsGamePaused())
        {
            PauseMenu.Instance.PauseToggle();
        }
        
        if (_backButtons.TryPop(out Button button))
        {
            button.onClick.Invoke();
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
    private void SwapInput(InputDevice device)
    {
        if (device == Gamepad.current)
        {
            _isUsingController = true;
        }
        else if (device == Mouse.current || device == Keyboard.current)
        {
            _isUsingController = false;
        }
        
        SaveManager.Instance.GetGameSaveData().IsUsingController = _isUsingController;
        ToggleMouse();
        _onSwapInput?.Invoke();
    }

    /// <summary>
    /// Toggles mouse availability based on when a controller is used
    /// </summary>
    private void ToggleMouse()
    {
        if (_isUsingController)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// This only toggles the mouse if the player is playing the game and the game is paused
    /// </summary>
    /// <param name="isPaused">Checks if the game is paused</param>
    /// <param name="hasAudio">Useless</param>
    private void ToggleMouse(bool isPaused, bool hasAudio)
    {
        if (!_isUsingController)
        {
            if (isPaused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    
    /// <summary>
    /// If the newly loaded scene is the main menu, it will toggle the mouse
    /// </summary>
    /// <param name="newScene"></param>
    /// <param name="loadSceneMode"></param>
    private void MainSceneCheck(Scene newScene, LoadSceneMode loadSceneMode)
    {
        if (newScene != SceneManager.GetSceneAt(0))
        {
            return;
        }
        ToggleMouse();
    }

    /// <summary>
    /// Swapping input will auto change UI displayed controls
    /// </summary>
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        ControlsManager.Instance.GetOnChangeControls.AddListener(SwapInput);
        TimeManager.Instance.GetOnGamePauseToggleEvent().AddListener(ToggleMouse);
        SceneManager.sceneLoaded += MainSceneCheck;
    }

    /// <summary>
    /// If the player still has back buttons, then this will remove the UIback listener
    /// </summary>
    private void OnDisable()
    {
        if (!_playerInput.Player.IsUnityNull())
        {
            _playerInput.Player.UIBack.performed -= ActivateBackButton;
        }
        ControlsManager.Instance.GetOnChangeControls.RemoveListener(SwapInput);
        TimeManager.Instance.GetOnGamePauseToggleEvent().RemoveListener(ToggleMouse);
        SceneManager.sceneLoaded -= MainSceneCheck;
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

    public bool ShouldMouseAppearUsingController => _shouldMouseAppearUsingController;

    public UnityEvent GetOnSwapInput => _onSwapInput;

    #endregion
}
