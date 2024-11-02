/***********************************************************************************************************************
// File Name:       WeakPoint.cs
// Author:          Ryan Swanson
// Contributors:    Andrea Swihart-DeCoster
// Creation Date:   September 22, 2024
//
// Description:     Provides the weak point with its needed functionality
***********************************************************************************************************************/

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides the weak point with its needed functionality
/// </summary>
public class WeakPoint : MonoBehaviour
{
    public WeakPointHealth HealthComponent {  get; private set; }
    
    private void Awake()
    {
        InitializeHealthComponent();
    }

    /// <summary>
    /// Initializes the health component
    /// </summary>
    private void InitializeHealthComponent()
    {
        if (!TryGetComponent(out WeakPointHealth weakPointHealth)) return;
        HealthComponent = weakPointHealth;
        HealthComponent.DeathEvent().AddListener(PlayDeathSfx);
    }

    /// <summary>
    /// Access the death event from the health component
    /// </summary>
    /// <returns></returns>
    public UnityEvent GetWeakPointDeathEvent()
    {
        return HealthComponent.GetDeathEvent();
    }

    /// <summary>
    /// Plays the Death SFX
    /// </summary>
    private void PlayDeathSfx()
    {
        RuntimeSfxManager.APlayOneShotSFX?.Invoke(FmodSfxEvents.Instance.WeakPointDestroyed, transform.position);
    }
}
