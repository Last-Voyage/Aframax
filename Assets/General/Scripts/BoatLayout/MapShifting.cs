using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapShifting : MonoBehaviour
{
    [SerializeField]private protected GameObject[] _listOfLayouts;
    private protected int _currentLayout = 0;
    // Start is called before the first frame update
    void Start()
    {
        _listOfLayouts[_currentLayout].SetActive(true);
    }

    private protected void LayoutSwapForward()
    {
        _listOfLayouts[_currentLayout].SetActive(false);
        ++_currentLayout;
        _listOfLayouts[_currentLayout].SetActive(true);
    }

    private protected void LayoutSwapBackward()
    {
        _listOfLayouts[_currentLayout].SetActive(false);
        --_currentLayout;
        _listOfLayouts[_currentLayout].SetActive(true);
    }
}
