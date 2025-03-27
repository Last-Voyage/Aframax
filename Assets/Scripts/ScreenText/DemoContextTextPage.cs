using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DemoContextTextPage : MonoBehaviour
{
    [Serializable]
    private struct TextPage
    {
        public string TextToDisplay;
        public float ActiveTime;
    }

    [SerializeField] private TextPage[] _textPages;
    private TMP_Text _currentText;

    private void Awake()
    {
        GetTextComponent();

        SetFirstPage();

        StartCoroutine(ProgressPages());
    }

    private void GetTextComponent()
    {
        _currentText = GetComponent<TMP_Text>();
    }

    private void SetFirstPage()
    {
        if (!_textPages[0].IsUnityNull())
        {
            _currentText.text = _textPages[0].TextToDisplay;
        }
    }

    private IEnumerator ProgressPages()
    {
        int index = 0;

        while (index < _textPages.Length)
        {
            yield return new WaitForSeconds(_textPages[index].ActiveTime);

            index++;

            if (index < _textPages.Length)
            {
                _currentText.text = _textPages[index].TextToDisplay;
            }
        }
    }
}
