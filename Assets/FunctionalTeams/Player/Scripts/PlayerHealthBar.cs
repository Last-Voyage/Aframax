/*****************************************************************************
// File Name :         PlayerHealthBar.cs
// Author :            Jeremiah Peters
// Creation Date :     9/16/24
//
// Brief Description : operates the health bar for the player
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image HealthBar;

    private void Awake()
    {
        //this way it doesn't waste time doing find if it's already connected
        if (HealthBar == null)
        {
            HealthBar = GameObject.FindObjectOfType<Image>();
            if (HealthBar == null)
            {
                Debug.Log("Couldn't find health bar. Make sure there's one in the scene!");
            }
        }
    }
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (HealthBar != null)
        {
            HealthBar.fillAmount = currentHealth / maxHealth;
        }
    }
}
