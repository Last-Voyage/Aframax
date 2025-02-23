/**********************************************************************************************************************
// File Name :         AmmoRackInteractable.cs
// Author :            Andrew Stapay
// Contributors:       Jeremiah Peters, Ryan Swanson
// Creation Date :     11/13/2024
//
// Brief Description : Implements an ammo rack where the player can refill their ammo.
**********************************************************************************************************************/

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A class that implements the interaction with the ammo rack
/// </summary>
public class AmmoRackInteractable : TogglableInteractable, IPlayerInteractable
{
    // The nuumber of harpoons that are currently on the rack
    private int _currentHarpoons;

    [SerializeField] public UnityEvent OnAmmoDepletedEvent;

    /// <summary>
    /// Called when the game starts
    /// Sets the number of harpoons to the number of game objects in the ammo rack in-engine
    /// </summary>
    private void Awake()
    {
        _currentHarpoons = transform.GetChild(0).childCount;
    }

    /// <summary>
    /// Subscribes to events of when the ammo changes
    /// </summary>
    protected override void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFiredEvent().AddListener(UpdateInteractability);
        PlayerManager.Instance.GetOnHarpoonRestockCompleteEvent().AddListener(UpdateInteractability);
    }

    /// <summary>
    /// Unsubscribes to all subscribed events
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFiredEvent().RemoveListener(UpdateInteractability);
        PlayerManager.Instance.GetOnHarpoonRestockCompleteEvent().RemoveListener(UpdateInteractability);
    }

    /// <summary>
    /// Triggers when the ammo rack is interacted with
    /// </summary>
    public void OnInteractedByPlayer()
    {
        if(!_canInteract)
        {
            return;
        }

        PlayerManager.Instance.OnInvokeHarpoonRestockEvent(this);
        if (_currentHarpoons == 0)
        {
            //disable interact prompt
            OnAmmoDepletedEvent?.Invoke();
        }
    }

    /// <summary>
    /// Removes a number of harpoons from the ammo rack
    /// </summary>
    /// <param name="numHarpoons"> the number of harpoons to remove </param>
    public void RemoveHarpoons(int numHarpoons)
    {
        for (int i = 0; i < numHarpoons; i++)
        {
            _currentHarpoons--;
            DestroyImmediate(transform.GetChild(0).GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// Updates if the harpoon ammo rack is interactable
    /// </summary>
    private void UpdateInteractability()
    {
        _canInteract = !HarpoonGun.Instance.IsAtMaxAmmo();
        UpdateInteractablePopupToggle();
    }

    /// <summary>
    /// Second function for subscribing to the event for restocking ammo. It passes in an int so I needed 2 of these.
    /// </summary>
    /// <param name="ammo"> The number of harpoons restocked </param>
    private void UpdateInteractability(int ammo)
    {
        UpdateInteractability();
    }

    /// <summary>
    /// Gets the number of harpoons in this ammo rack
    /// </summary>
    /// <returns> The number of harpoons in the ammo rack </returns>
    public int GetNumHarpoons() => _currentHarpoons;
}
