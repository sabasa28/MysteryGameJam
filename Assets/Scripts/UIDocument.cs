using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDocument : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI placeFoundText;
    public Button button;
    public GameObject unreadIndicator;
    ColorBlock defaultColor;
    bool defaultColorSet = false;

    public void SetAsSelected()
    {
        ColorBlock aux = button.colors;
        aux.normalColor = defaultColor.pressedColor;
        aux.highlightedColor = defaultColor.pressedColor;
        button.colors = aux;
    }

    public void ResetToUnselected()
    {
        if (button.colors.normalColor != defaultColor.normalColor) //checkeo solo el normal porque es mas barato
        {
            button.colors = defaultColor;
        }
    }
    public void SetDefaultColor()
    {
        if (!defaultColorSet)
        {
            defaultColor = button.colors;
            defaultColorSet = true;
        }
    }
}
