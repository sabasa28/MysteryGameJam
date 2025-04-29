using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Log Entry", menuName = "Log Entry")]
public class LogEntry : ScriptableObject
{
    public Sprite attachedPhoto;
    public string logText;
    public int orderFound;
}
