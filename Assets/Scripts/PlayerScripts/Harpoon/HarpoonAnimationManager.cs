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

    // The names of the parameters used in the Animator
    private const string _FIRE_ANIM = "shoot";
    private const string _FOCUS_START_ANIM = "holdFocus";
    private const string _FOCUS_END_ANIM = "releaseFocus";
    private const string _RELOAD_READY_ANIM = "reloadReady";
    private const string _AMMO_EMPTY_ANIM = "ammoEmpty";
    private const string _NOT_WALL_ANIM = "notAtWall";
    private const string _AT_WALL_ANIM = "atWall";
    
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
        PlayerManager.Instance.GetOnHarpoonRestockCompleteEvent().AddListener(ReloadFromEmptyAnimation);
        PlayerManager.Instance.GetOnHarpoonStartReloadEvent().AddListener(ReloadHarpoonAnimation);
    }

    /// <summary>
    /// Unsubscribe to the needed events
    /// </summary>
    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().RemoveListener(StartFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFocusEndEvent().RemoveListener(StopFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFiredEvent().RemoveListener(StartFiringAnimation);
        PlayerManager.Instance.GetOnHarpoonRestockCompleteEvent().RemoveListener(ReloadFromEmptyAnimation);
        PlayerManager.Instance.GetOnHarpoonStartReloadEvent().RemoveListener(ReloadHarpoonAnimation);
    }

    /// <summary>
    /// Changes the animation to the focusing animation
    /// </summary>
    private void StartFocusingAnimation()
    {
        _animator.SetTrigger(_FOCUS_START_ANIM);
        _animator.ResetTrigger(_FOCUS_END_ANIM);
    }

    /// <summary>
    /// Changes the animation from the focusing animation back to the idle animation
    /// </summary>
    private void StopFocusingAnimation()
    {
        _animator.SetTrigger(_FOCUS_END_ANIM);
    }

    /// <summary>
    /// Changes the animation from the focusing animation to the firing animation
    /// </summary>
    private void StartFiringAnimation()
    {
        _animator.SetTrigger(_FIRE_ANIM);

        //Check to see if we are out of ammo
        //If we are, start the empty ammo animation
        if (HarpoonGun.Instance.GetHarpoonFiringState() != HarpoonGun.EHarpoonFiringState.Ready)
        {
            if (HarpoonGun.Instance.GetReserveAmmo() == 0)
            {
                _animator.SetTrigger(_AMMO_EMPTY_ANIM);
            }
        }
    }

    /// <summary>
    /// Changes the animation to the reloading animation
    /// </summary>
    private void ReloadHarpoonAnimation()
    {
        _animator.SetTrigger(_RELOAD_READY_ANIM);
    }
    
    /// <summary>
    /// Separate animation change after restocking on ammo
    /// If we went from completely empty to full, play the reloading animation
    /// </summary>
    /// <param name="ammoRestocked"> The amount of ammo that was restocked </param>
    private void ReloadFromEmptyAnimation(int ammoRestocked)
    {
        if (ammoRestocked == HarpoonGun.Instance.GetMaxAmmo())
        {
            _animator.SetTrigger(_RELOAD_READY_ANIM);
        }
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
                _animator.SetTrigger(_AT_WALL_ANIM);
                _animator.ResetTrigger(_NOT_WALL_ANIM);
            }
            else
            {
                _animator.SetTrigger(_NOT_WALL_ANIM);
                _animator.ResetTrigger(_AT_WALL_ANIM);
            }

            yield return null;
        }
    }
}
