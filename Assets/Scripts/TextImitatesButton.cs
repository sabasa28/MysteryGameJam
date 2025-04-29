using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextImitatesButton : MonoBehaviour
{
    TextMeshProUGUI text;
    Button button;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        button = GetComponentInParent<Button>();
    }

    // Es una ui no me molestes
    void Update()
    {
        if (text.color != button.targetGraphic.canvasRenderer.GetColor())
        {
            text.color = button.targetGraphic.canvasRenderer.GetColor();
        }
    }
}
