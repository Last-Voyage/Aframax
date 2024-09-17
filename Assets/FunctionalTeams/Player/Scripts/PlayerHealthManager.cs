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
    [SerializeField] private float _playerHealth;

    //used for calculating the health bar size
    [SerializeField] private float _maxPlayerHealth;

    //this is a game object in the scene that runs the health bar
    private GameObject HealthBarManager;

    public void Awake()
    {
        //this way it doesn't waste time doing find if it's already connected
        if (HealthBarManager == null)
        {
            HealthBarManager = GameObject.Find("HealthBarManager");
            if (HealthBarManager == null)
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
            //Debug.Log(other.gameObject.GetComponent<EnemyDamageTemp>().AttackPower);

            TakeDamage(other.gameObject.GetComponent<EnemyDamageTemp>().AttackPower);
        }
    }

    public void TakeDamage(float AttackPower)
    {
        _playerHealth = _playerHealth - AttackPower;

        //calls update health bar function from healthbar manager
        if (HealthBarManager != null)
        {
            HealthBarManager.GetComponent<PlayerHealthBar>().UpdateHealthBar(_playerHealth, _maxPlayerHealth);
        }

        if (_playerHealth <= 0)
        {
            //Debug.Log("man im dead");

            //when you die it reloads the current scene
            //this is what i was told to do on the trello so i did it
            //trello also mentions hooking it up with ryan's scene loader system, which probably wouldn't be too hard
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
