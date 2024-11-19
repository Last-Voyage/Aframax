using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonAnimationManager : MonoBehaviour
{
    private Animator _animator;

    private const string _FIRE_ANIM = "shoot";
    private const string _FOCUS_START_ANIM = "holdFocus";
    private const string _FOCUS_END_ANIM = "releaseFocus";
    private const string _RELOAD_READY_ANIM = "reloadReady";
    private const string _AMMO_EMPTY_ANIM = "ammoEmpty";
    private const string _NOT_WALL_ANIM = "notAtWall";
    private const string _AT_WALL_ANIM = "atWall";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().AddListener(StartFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFocusEndEvent().AddListener(StopFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFiredEvent().AddListener(StartFiringAnimation);
        PlayerManager.Instance.GetOnHarpoonRestockCompleteEvent().AddListener(ReloadFromEmptyAnimation);
        PlayerManager.Instance.GetOnHarpoonReloadedEvent().AddListener(ReloadHarpoonAnimation);
    }

    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().RemoveListener(StartFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFocusEndEvent().RemoveListener(StopFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFiredEvent().RemoveListener(StartFiringAnimation);
        PlayerManager.Instance.GetOnHarpoonRestockCompleteEvent().RemoveListener(ReloadFromEmptyAnimation);
        PlayerManager.Instance.GetOnHarpoonStartReloadEvent().RemoveListener(ReloadHarpoonAnimation);
    }

    private void StartFocusingAnimation()
    {
        _animator.SetTrigger(_FOCUS_START_ANIM);
        _animator.ResetTrigger(_FOCUS_END_ANIM);
    }

    private void StopFocusingAnimation()
    {
        _animator.SetTrigger(_FOCUS_END_ANIM);
        _animator.ResetTrigger(_FOCUS_START_ANIM);

        if (HarpoonGun.Instance.GetHarpoonFiringState() != HarpoonGun.EHarpoonFiringState.Ready)
        {
            if (HarpoonGun.Instance.GetReserveAmmo() > 0)
            {
                _animator.SetTrigger(_RELOAD_READY_ANIM);
                //_animator.ResetTrigger(_RELOAD_READY_ANIM);
            }
            else
            {
                _animator.SetTrigger(_AMMO_EMPTY_ANIM);
                //_animator.ResetTrigger(_AMMO_EMPTY_ANIM);
            }
        }
    }

    private void StartFiringAnimation()
    {
        _animator.SetTrigger(_FIRE_ANIM);
        //_animator.ResetTrigger(_FIRE_ANIM);

        StopFocusingAnimation();
    }

    private void ReloadHarpoonAnimation()
    {
        _animator.SetTrigger(_RELOAD_READY_ANIM);
    }
    
    private void ReloadFromEmptyAnimation(int unusedEventVar)
    {
        _animator.SetTrigger(_RELOAD_READY_ANIM);
        //_animator.ResetTrigger(_RELOAD_READY_ANIM);
    }
}
