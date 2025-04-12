using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Document", menuName = "Document")]
public class Document : ScriptableObject
{
    public string fullText;
    public string currentText;

}
