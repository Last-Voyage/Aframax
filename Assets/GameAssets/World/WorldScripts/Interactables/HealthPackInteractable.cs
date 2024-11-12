using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackInteractable : MonoBehaviour, IPlayerInteractable
{
    [SerializeField] private int _healthRestored = 20;
    [SerializeField] private int _numUses = 3;

    public void OnInteractedByPlayer()
    {
        PlayerManager.Instance.InvokePlayerHealEvent(_healthRestored);
        _numUses--;

        if (_numUses == 0)
        {
            OutOfUses();
        }
    }

    private void OutOfUses()
    {
        Destroy(gameObject);
    }
}
