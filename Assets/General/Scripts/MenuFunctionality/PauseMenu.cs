/*****************************************************************************
// File Name :         PauseMenu.cs
// Author :            Jeremiah Peters
//                     Ryan Swanson
// Creation Date :     9/28/24
//
// Brief Description : operates pausing the game and the pause menu buttons
*****************************************************************************/

using UnityEngine;

/// <summary>
/// functionality for pausing the game and the pause menu buttons
/// </summary>
public class PauseMenu : MonoBehaviour
{
    //Contains all ui to toggle on and off
    //Personally I prefer to make this serialized rather than getting the child in awake
    //You can get children by order, but that order can change if messed with
    [SerializeField] private GameObject _pauseMenuContent;

    private PlayerInputMap _playerInputControls;

    private void Awake()
    {
        //initialize input
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.Pause.performed += ctx => PauseToggle();
    }

    /// <summary>
    /// toggles the pause state so you can press escape again to close the pause menu
    /// Public so that it can be accessed by button
    /// </summary>
    public void PauseToggle()
    {
        TimeManager.Instance.PauseGameToggle();
    }

    /// <summary>
    /// Enables and disables the pause menu ui
    /// </summary>
    /// <param name="isVisible"></param>
    private void PauseUIVisibility(bool isVisible)
    {
        _pauseMenuContent.SetActive(isVisible);
    }

    /// <summary>
    /// it quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        _playerInputControls.Enable();

        TimeManager.Instance.GetOnGamePauseToggleEvent().AddListener(PauseUIVisibility);
    }

    private void OnDisable()
    {
        _playerInputControls.Disable();

        TimeManager.Instance.GetOnGamePauseToggleEvent().RemoveListener(PauseUIVisibility);
    }
}
