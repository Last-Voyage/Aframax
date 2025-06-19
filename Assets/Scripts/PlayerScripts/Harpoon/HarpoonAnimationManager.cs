/******************************************************************************
// File Name:       HarpoonAnimationManager.cs
// Author:          Andrew Stapay
// Creation Date:   November 18, 2024
//
// Description:     Assists with the logic of the harpoon's animations.
******************************************************************************/
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class that assists the harpoon's animator's logic
/// </summary>
public class HarpoonAnimationManager : MonoBehaviour
{
    private Animator _animator;

    // Names of states used in the Animator
    private const string _IDLE_ANIM = "harpoonIdle";

    // The names of the parameters used in the Animator
    private const string _FIRE_ANIM = "shoot";
    private const string _FOCUS_ANIM = "focusing";
    private const string _RELOAD_READY_ANIM = "reloadReady";
    private const string _RESTOCK_COMPLETE_ANIM = "restockComplete";
    private const string _AMMO_EMPTY_ANIM = "ammoEmpty";
    private const string _NOT_WALL_ANIM = "notAtWall";
    private const string _AT_WALL_ANIM = "atWall";
    private const string _SPRINT_ANIM = "sprint";
    private const string _PLAYER_MOVING_ANIM = "move";
    
    // Used to check to see if we are near a wall
    private const float _WALL_CHECK_DIST = 1;

    /// <summary>
    /// Called when the game starts
    /// Used to get the animator, subscribe to events, and start the coroutine for checking for walls.
    /// </summary>
    private void Awake()
    {
        GetAnimator();
        SubscribeToEvents();
        StartCoroutine(CheckForWall());
    }

    /// <summary>
    /// Called when the game ends
    /// Used to unsubscribe to events
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeToEvents();
        StopAllCoroutines();
    }

    /// <summary>
    /// Retrieves the Animator from the GameObject
    /// </summary>
    private void GetAnimator()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Subscribe to the needed events
    /// </summary>
    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().AddListener(StartFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFocusEndEvent().AddListener(StopFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFiredEvent().AddListener(StartFiringAnimation);
        PlayerManager.Instance.GetOnHarpoonRestockCompleteEvent().AddListener(RestockFromEmptyAnimation);
        PlayerManager.Instance.GetOnHarpoonStartReloadEvent().AddListener(ReloadHarpoonAnimation);
        PlayerManager.Instance.GetOnMovementStartEvent().AddListener(delegate { TogglePlayerMovingAnimation(true); });
        PlayerManager.Instance.GetOnMovementEndEvent().AddListener(delegate { TogglePlayerMovingAnimation(false); });
        EnemyManager.Instance.GetOnChaseSequenceBegin().AddListener(StartSprintAnimation);
        CameraManager.Instance.GetOnCinematicStartEvent().AddListener(ForceIdle);
    }

    /// <summary>
    /// Unsubscribe to the needed events
    /// </summary>
    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().RemoveListener(StartFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFocusEndEvent().RemoveListener(StopFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFiredEvent().RemoveListener(StartFiringAnimation);
        PlayerManager.Instance.GetOnHarpoonRestockCompleteEvent().RemoveListener(RestockFromEmptyAnimation);
        PlayerManager.Instance.GetOnHarpoonStartReloadEvent().RemoveListener(ReloadHarpoonAnimation);
        PlayerManager.Instance.GetOnMovementStartEvent().RemoveListener(delegate { TogglePlayerMovingAnimation(true); });
        PlayerManager.Instance.GetOnMovementEndEvent().RemoveListener(delegate { TogglePlayerMovingAnimation(false); });
        EnemyManager.Instance.GetOnChaseSequenceBegin().RemoveListener(StartSprintAnimation);
        CameraManager.Instance.GetOnCinematicStartEvent().RemoveListener(ForceIdle);
    }

    /// <summary>
    /// Changes the animation to the focusing animation
    /// </summary>
    private void StartFocusingAnimation()
    {
        
        _animator.SetBool(Animator.StringToHash(_FOCUS_ANIM), true);
    }

    /// <summary>
    /// Changes the animation from the focusing animation back to the idle animation
    /// </summary>
    private void StopFocusingAnimation()
    {
        _animator.SetBool(Animator.StringToHash(_FOCUS_ANIM), false);
    }

    /// <summary>
    /// Changes the animation from the focusing animation to the firing animation
    /// </summary>
    private void StartFiringAnimation()
    {
        _animator.SetTrigger(Animator.StringToHash(_FIRE_ANIM));
        _animator.SetBool(Animator.StringToHash(_FOCUS_ANIM), false);

        //Check to see if we are out of ammo
        //If we are, start the empty ammo animation
        if (HarpoonGun.Instance.GetHarpoonFiringState() != HarpoonGun.EHarpoonFiringState.Ready)
        {
            if (HarpoonGun.Instance.GetReserveAmmo() == 0)
            {
                _animator.SetTrigger(Animator.StringToHash(_AMMO_EMPTY_ANIM));
            }
        }
    }

    /// <summary>
    /// Changes the animation to the reloading animation
    /// </summary>
    private void ReloadHarpoonAnimation()
    {
        _animator.SetTrigger(Animator.StringToHash(_RELOAD_READY_ANIM));
    }
    
    /// <summary>
    /// Separate animation change after restocking on ammo
    /// If we went from completely empty to full, play the reloading animation
    /// </summary>
    /// <param name="ammoRestocked"> The amount of ammo that was restocked </param>
    private void RestockFromEmptyAnimation(int ammoRestocked)
    {
        if (ammoRestocked == HarpoonGun.Instance.GetReserveAmmo())
        {
            _animator.SetTrigger(Animator.StringToHash(_RESTOCK_COMPLETE_ANIM));
        }
    }

    /// <summary>
    /// Changes the animation boolean for sprinting to true
    /// </summary>
    private void StartSprintAnimation()
    {
        _animator.SetBool(Animator.StringToHash(_SPRINT_ANIM), true);
    }

    /// <summary>
    /// Toggles if the player is moving in the animations
    /// </summary>
    /// <param name="isMoving"> If the player is moving </param>
    private void TogglePlayerMovingAnimation(bool isMoving)
    {
        _animator.SetBool(Animator.StringToHash(_PLAYER_MOVING_ANIM), isMoving);
    }

    /// <summary>
    /// Coroutine that checks to see if the player is near a wall
    /// Used to start and stop the holstering animation
    /// </summary>
    private IEnumerator CheckForWall()
    {
        while(true)
        {
            if (Physics.Raycast(transform.position, transform.parent.forward, _WALL_CHECK_DIST))
            {
                _animator.SetTrigger(Animator.StringToHash(_AT_WALL_ANIM));
                _animator.ResetTrigger(Animator.StringToHash(_NOT_WALL_ANIM));
            }
            else
            {
                _animator.SetTrigger(Animator.StringToHash(_NOT_WALL_ANIM));
                _animator.ResetTrigger(Animator.StringToHash(_AT_WALL_ANIM));
            }

            yield return null;
        }
    }

    /// <summary>
    /// Automatically resets the animator to the idle state
    /// Used for cinematics
    /// </summary>
    private void ForceIdle()
    {
        // Cut back to the idle animation
        _animator.Play(Animator.StringToHash(_IDLE_ANIM));

        // Reset everything
        _animator.ResetTrigger(Animator.StringToHash(_FIRE_ANIM));
        _animator.SetBool(Animator.StringToHash(_FOCUS_ANIM), false);
        _animator.ResetTrigger(Animator.StringToHash(_RELOAD_READY_ANIM));
        _animator.ResetTrigger(Animator.StringToHash(_RESTOCK_COMPLETE_ANIM));
        _animator.ResetTrigger(Animator.StringToHash(_AMMO_EMPTY_ANIM));
        _animator.ResetTrigger(Animator.StringToHash(_NOT_WALL_ANIM));
        _animator.ResetTrigger(Animator.StringToHash(_AT_WALL_ANIM));
        _animator.SetBool(Animator.StringToHash(_SPRINT_ANIM), false);
        _animator.SetBool(Animator.StringToHash(_PLAYER_MOVING_ANIM), false);
    }
}
