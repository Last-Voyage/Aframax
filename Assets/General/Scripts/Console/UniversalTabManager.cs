/*
// Name: UniversalTabManager.CS
// Author: Nabil Tagba
// Overview: Seamlessly handles and manages the creation and
//transitions of tabs and their corresponding pages
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// Allows for seamless transitions between
/// tabs
/// </summary>
public class UniversalTabManager : MonoBehaviour
{   
    //the list of the bachground of the tabs and the pages the tab
    //enables. the indexes should correlate
    [SerializeField] private List<GameObject> _tabsBackgrounds = new List<GameObject>();
    [SerializeField] private List<GameObject> _tabsPages = new List<GameObject>();

    private Color _selectedColor;
    private Color _unSelectedColor;


    void Start()
    {

        _selectedColor = Color.white;
        _unSelectedColor = new Color(0.4037736f, 0.383204f, 0.383204f, 1);

        ChangeTab(0);

    }



    /// <summary>
    /// when a tab is pressed, this method is called 
    /// the tab index should correlate to the index
    /// of the tab in the list as well as the page
    /// </summary>
    /// <param name="tabIndex"></param>
    public void ChangeTab(int tabIndex)
    {
        for (int i = 0; i < _tabsBackgrounds.Count; i++)
        {
            if (i == tabIndex)
            {
                //enable the tab
                GameObject _currentSelectedTab = _tabsPages[i];
                GameObject _currentSelectedTabBK = _tabsBackgrounds[i];

                _currentSelectedTab.SetActive(true);

                _currentSelectedTabBK.GetComponent<Image>().color = _selectedColor;
                //turn off the other tab pages except for the select tab's page
                foreach (GameObject tab in _tabsPages)
                {
                    if (tab != _currentSelectedTab)
                    {
                        tab.SetActive(false);
                    }
                }


                //change the unselected tabs backgrounds to the unselect tab bk color

                foreach (GameObject tabBK in _tabsBackgrounds) 
                {
                    if (tabBK != _currentSelectedTabBK)
                    {
                        tabBK.GetComponent<Image>().color = _unSelectedColor;
                    }
                }

            }
        }
    }

   
}