/*****************************************************************************
// File Name :         DevUi.cs
// Author :            Nabil Tagba
// Creation Date :     2/23/2025
//
// Brief Description : sets the health, focus level, and speed for the dev ui
*****************************************************************************/
using TMPro;
using UnityEngine;

public class DevUI : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text focusText;
    /// <summary>
    /// happens every frame
    /// </summary>
    private void Update()
    {

        healthText.text = "HP: " + FindObjectOfType<PlayerHealth>().GetCurrentHealth().ToString();
        speedText.text = "Speed: " + FindObjectOfType<PlayerMovementController>().GetPlayerSpeed().ToString();
        focusText.text = "Focus: " + FindObjectOfType<HarpoonGun>().GetFocusAccuracy();
    }
}
