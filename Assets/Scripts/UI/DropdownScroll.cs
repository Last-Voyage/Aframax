using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DropdownScroll : MonoBehaviour
{
    public float scrollSpeed = 10f;
    private bool mouseOver = false;

    private List<Selectable> m_Selectables = new List<Selectable>();
    private ScrollRect m_ScrollRect;

    private Vector2 m_NextScrollPosition = Vector2.up;
    void OnEnable()
    {
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
    }
    void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
    }
    void Start()
    {
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
        ScrollToSelected(true);
    }
    void Update()
    {
        // Scroll via input.
        InputScroll();
        if (!mouseOver)
        {
            // Lerp scrolling code.
            m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, scrollSpeed * Time.deltaTime);
        }
        else
        {
            m_NextScrollPosition = m_ScrollRect.normalizedPosition;
        }
        //m_NextScrollPosition = m_ScrollRect.normalizedPosition;
    }
    void InputScroll()
    {
        if (m_Selectables.Count > 0)
        {
            if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical") || Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                ScrollToSelected(false);
            }
        }
    }
    
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
        Debug.Log("Scroll to selected");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ScrollToSelected(false);
    }

    public void TESTING()
    {
        Debug.Log("TESTING");
        ScrollToSelected(false);
    }
    
    
    
    
    /*public TMP_Dropdown targetDropdown; // Assign your TMP_Dropdown here
    private ScrollRect scrollRect;
    private RectTransform contentRect;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            contentRect = scrollRect.content;
        }
    }

    void OnEnable()
    {
        if (targetDropdown != null)
        {
            targetDropdown.onValueChanged.AddListener(ScrollToSelectedItem);
        }
    }

    void OnDisable()
    {
        if (targetDropdown != null)
        {
            targetDropdown.onValueChanged.RemoveListener(ScrollToSelectedItem);
        }
    }

    public void ScrollToSelectedItem(int index)
    {
        if (scrollRect == null || contentRect == null || targetDropdown.options.Count == 0)
        {
            Debug.Log("RETURNING");
            return;
        }

        // Calculate the target position based on the selected item's index
        float itemHeight = contentRect.rect.height / targetDropdown.options.Count;
        float targetScrollPosition = 1f - (itemHeight * (index + 0.5f) / contentRect.rect.height); // Adjust 0.5f for centering
        Debug.Log("item height: " + itemHeight);

        // Clamp the value to ensure it's within 0 and 1
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(targetScrollPosition);
    }*/
}
