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
    public Image HealthBar;

    private void Awake()
    {
        HealthBar = GameObject.FindObjectOfType<Image>();
        if (HealthBar == null)
        {
            Debug.Log("Couldn't find health bar. Make sure there's one in the scene!");
        }
    }
    public void UpdateHealthBar(float CurrentHealth, float MaxHealth)
    {
        if (HealthBar != null)
        {
            HealthBar.fillAmount = CurrentHealth / MaxHealth;
        }
    }
}
