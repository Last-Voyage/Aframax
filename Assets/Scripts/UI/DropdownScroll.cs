/******************************************************************************
// File Name:       DropdownScroll.cs
// Author:          Ryan Swanson
// Creation Date:   Aug 7, 2025
//
// Description:     Contains the functionality to scroll the dropdown
// Links :          Borrowed functionality from: https://gist.github.com/mandarinx/eae10c9e8d1a5534b7b19b74aeb2a665
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Serialization;

/// <summary>
/// Allows for scrolling of dropdown
/// </summary>
public class DropdownScroll : MonoBehaviour
{
    [SerializeField] private float _scrollSpeed = 5f;
    private bool _isMouseOver = false;

    private List<Selectable> m_Selectables = new List<Selectable>();
    private ScrollRect m_ScrollRect;

    private Vector2 m_NextScrollPosition = Vector2.up;
    
    /// <summary>
    /// Gets components on enable
    /// </summary>
    void OnEnable()
    {
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
    }
    /// <summary>
    /// Gets components on awake
    /// </summary>
    void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
    }
    /// <summary>
    /// Gets components on start
    /// </summary>
    void Start()
    {
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
        ScrollToSelected(true);
    }
    /// <summary>
    /// Scrolls the dropdown
    /// </summary>
    void Update()
    {
        if (!_isMouseOver)
        {
            // Lerp scrolling code.
            m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, _scrollSpeed * Time.unscaledDeltaTime);
        }
        else
        {
            m_NextScrollPosition = m_ScrollRect.normalizedPosition;
        }
    }
    
    
    /// <summary>
    /// Scrolls to the selected object
    /// </summary>
    /// <param name="quickScroll"></param>
    void ScrollToSelected(bool quickScroll)
    {
        int selectedIndex = -1;
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

        if (selectedElement)
        {
            selectedIndex = m_Selectables.IndexOf(selectedElement);
        }
        if (selectedIndex > -1)
        {
            if (quickScroll)
            {
                m_ScrollRect.normalizedPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
                m_NextScrollPosition = m_ScrollRect.normalizedPosition;
            }
            else
            {
                m_NextScrollPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
            }
        }
    }
    /// <summary>
    /// When the pointer overlaps the dropdown
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        _isMouseOver = true;
    }
    /// <summary>
    /// When the pointer leaves the dropdown
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        _isMouseOver = false;
        ScrollToSelected(false);
    }

    /// <summary>
    /// Scrolls to the currently hovered object
    /// </summary>
    public void ScrollToCurrentHovered()
    {
        ScrollToSelected(false);
    }
    
}
