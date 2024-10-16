/*****************************************************************************
// File Name :         PlayerHeathManager.cs
// Author :            Jeremiah Peters
// Creation Date :     9/16/24
//
// Brief Description : Controls the player's health, makes them take damage
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthManager : MonoBehaviour
{
    //this is the health for the player and when it hits 0 you die
    private float _playerHealth;

    //used for calculating the health bar size
    [SerializeField] private float _maxPlayerHealth;

    //this is a game object in the scene that runs the health bar
    private PlayerHealthUI _healthUI;

    private void Awake()
    {
        //this way it doesn't waste time doing find if it's already connected
        if (_healthUI == null)
        {
            _healthUI = Object.FindObjectOfType<PlayerHealthUI>();

            if (_healthUI == null)
            {
                Debug.Log("Couldn't find health bar manager.");
            }
        }

        _playerHealth = _maxPlayerHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        //checks to see if the collider is the thing that kills you via tag
        //idk if this is a good way to check for that, but it's the first thing that came to mind
        if (other.gameObject.CompareTag("Hitbox"))
        {
            TakeDamage(other.gameObject.GetComponent<EnemyDamageTemp>().AttackPower);
        }
    }

    /// <summary>
    /// Contains functionality for the player to take damage on the old health system
    /// </summary>
    /// <param name="attackPower"></param>
    public void TakeDamage(float attackPower)
    {
        _playerHealth -= attackPower;

        //calls update health bar function from healthbar manager
        if (_healthUI != null)
        {
            _healthUI.UpdateHealthBar(_playerHealth/_maxPlayerHealth);
            _healthUI.PlayerDamagedUI();
        }

        if (_playerHealth <= 0)
        {
            PlayerDied();
        }
    }

    /// <summary>
    /// Called to kill the player
    /// Public to allow for the player to be forcibly killed from external sources
    /// </summary>
    public void PlayerDied()
    {
        //when you die it reloads the current scene through the scene loading manager
        //Will be changed later to have ui pop up to do this
        SceneLoadingManager.Instance.DeathReloadCurrentScene();
    }
}
