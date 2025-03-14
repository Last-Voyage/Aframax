/*****************************************************************************
// File Name :         DevUi.cs
// Author :            Nabil Tagba
// Creation Date :     2/23/2025
//
// Brief Description : sets the health, focus level, and speed for the dev ui
*****************************************************************************/
using TMPro;
using UnityEngine;

/// <summary>
/// sets the health, focus level, and speed for the dev ui
/// </summary>
public class DevUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private TMP_Text _focusText;
    /// <summary>
    /// happens every frame
    /// </summary>
    private void FixedUpdate()
    {

        _healthText.text = "HP: " + FindObjectOfType<PlayerHealth>().GetCurrentHealth().ToString();
        _speedText.text = "Speed: " + PlayerMovementController.Instance.GetPlayerSpeed().ToString();
        _focusText.text = "Focus: " + HarpoonGun.Instance.GetFocusAccuracy();
    }
}
