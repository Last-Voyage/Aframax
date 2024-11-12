using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DialogueLine", menuName ="Dialogue Line")]
public class DialogueLine : ScriptableObject
{
    [SerializeField] private string _text;
    [SerializeField] private float _textDisplayTime;
    [SerializeField] private float _textHoldTime;

    public string GetText() => _text;
    public float GetTextDisplayTime() => _textDisplayTime;
    public float GetTextHoldTime() => _textHoldTime;
}
