/******************************************************************************
// File Name:       WaterDrip.cs
// Author:          Ryan Swanson
// Creation Date:   March 3rd, 2025
//
// Description:     Plays the vfx and sfx of the water drip
******************************************************************************/

using PrimeTween;
using UnityEngine;

/// <summary>
/// Overrides the base class to also play audio with a delay
/// </summary>
public class WaterDrip : RandomizedVisualEffects
{
    [SerializeField] private float _audioDelay;
    private Tween _audioDelayTween;

    /// <summary>
    /// Overrides the play visual effect to delay the audio
    /// </summary>
    protected override void PlayVisualEffect()
    {
        base.PlayVisualEffect();
        _audioDelayTween = Tween.Delay(this, _audioDelay, PlayAssociatedAudio);
    }

    /// <summary>
    /// Plays the water drip audio
    /// </summary>
    private void PlayAssociatedAudio()
    {
        RuntimeSfxManager.APlayOneShotSfx(FmodSfxEvents.Instance.WaterDrip, transform.position);
    }

    /// <summary>
    /// Overrides the OnDestroy
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_audioDelayTween.isAlive)
        {
            _audioDelayTween.Stop();
        }
    }
}
