/******************************************************************************
// File Name:       HorrorCameraEffects.cs
// Author:          Ryan Swanson
// Creation Date:   February 19th, 2025
//
// Description:     Design friendly way to activate camera horror effects
******************************************************************************/

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides the design team with several options to play the camera horror effects
/// </summary>
public class HorrorCameraEffects : MonoBehaviour
{
    [SerializeField] private bool _doesPlayOnEnable;
    [SerializeField] private bool _doesPlayOnDisable;
    [SerializeField] private bool _doesPlayOnTriggerCollision;
    [SerializeField] private bool _doesPlayOnCollision;

    [Space]
    [SerializeField] private UnityEvent _onHorrorEffectPlayed;

    /// <summary>
    /// Checks to play the horror event on enable
    /// </summary>
    private void OnEnable()
    {
        if(_doesPlayOnEnable)
        {
            PlayCameraHorrorEffects();
        }
    }

    /// <summary>
    /// Checks to play the horror event on disable
    /// </summary>
    private void OnDisable()
    {
        if (_doesPlayOnDisable)
        {
            PlayCameraHorrorEffects();
        }

        _onHorrorEffectPlayed.RemoveAllListeners();
    }

    /// <summary>
    /// Checks to play the horror event on trigger enter
    /// </summary>
    /// <param name="other"> The object the trigger collided with </param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && _doesPlayOnTriggerCollision)
        {
            PlayCameraHorrorEffects();
        }
    }

    /// <summary>
    /// Checks to play the horror event on collision
    /// </summary>
    /// <param name="collision"> The object we are colliding with </param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && _doesPlayOnCollision)
        {
            PlayCameraHorrorEffects();
        }
    }

    /// <summary>
    /// Plays the horror effects and invokes the event
    /// Is called
    /// </summary>
    public void PlayCameraHorrorEffects()
    {
        CameraManager.Instance.InvokeOnJumpscare();
        _onHorrorEffectPlayed?.Invoke();
    }
}
