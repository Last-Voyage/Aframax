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
    private const string _DIDNT_SHOOT_ANIM = "didNotShoot";
    private const string _DID_SHOOT_ANIM = "didShoot";
    private const string _NOT_WALL_ANIM = "notAtWall";
    private const string _AT_WALL_ANIM = "atWall";

    private WaitForSeconds _sleep = new WaitForSeconds(1);

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
    }

    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().RemoveListener(StartFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFocusEndEvent().RemoveListener(StopFocusingAnimation);
        PlayerManager.Instance.GetOnHarpoonFiredEvent().RemoveListener(StartFiringAnimation);
        PlayerManager.Instance.GetOnHarpoonRestockCompleteEvent().RemoveListener(ReloadFromEmptyAnimation);
    }

    private void StartFocusingAnimation()
    {
        print("focusing");
        _animator.SetTrigger(_FOCUS_START_ANIM);
    }

    private void StopFocusingAnimation()
    {
        print("done focusing");
        _animator.SetTrigger(_FOCUS_END_ANIM);

        if (HarpoonGun.Instance.GetHarpoonFiringState() == HarpoonGun.EHarpoonFiringState.Ready)
        {
            _animator.SetTrigger(_DIDNT_SHOOT_ANIM);
            print("didnt shoot");
        }
        else
        {
            _animator.SetTrigger(_DID_SHOOT_ANIM);
            print("did shoot");

            if (HarpoonGun.Instance.GetReserveAmmo() > 0)
            {
                _animator.SetTrigger(_RELOAD_READY_ANIM);
                print("reloading");
            }
            else
            {
                _animator.SetTrigger(_AMMO_EMPTY_ANIM);
                print("out of ammo");
            }
        }
    }

    private void StartFiringAnimation()
    {
        _animator.SetTrigger(_FIRE_ANIM);
        print("firing");

        StopFocusingAnimation();
    }

    private void ReloadFromEmptyAnimation(int unusedEventVar)
    {
        if (_animator.GetBool(_AMMO_EMPTY_ANIM))
        {
            _animator.SetTrigger(_RELOAD_READY_ANIM);
            print("reloading from empty");
        }
    }
}
