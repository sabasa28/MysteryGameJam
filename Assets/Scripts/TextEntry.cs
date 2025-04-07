using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Speaker
{
    AstronautSpeak,
    AI,
    AstronautThought

}
[Serializable]
public struct TextLine
{
    public string text;
    public Speaker speaker;
}
[CreateAssetMenu(fileName = "New Text Entry", menuName = "TextEntry")]
public class TextEntry : ScriptableObject
{
    public TextLine[] textLine;
}
