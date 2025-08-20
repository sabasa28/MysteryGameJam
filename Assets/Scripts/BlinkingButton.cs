using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingButton : MonoBehaviour
{
    Button button;
    Color baseColor;
    ColorBlock auxColorBlock;
    [SerializeField] float timeBetweenBlinks;
    float timer = 0.0f;
    bool blinked = false;
    private void Awake()
    {
        button = GetComponent<Button>();
        auxColorBlock = button.colors;
        baseColor = auxColorBlock.normalColor;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeBetweenBlinks)
        {
            auxColorBlock.normalColor = blinked? baseColor : Color.white;
            auxColorBlock.highlightedColor = blinked? baseColor : Color.white;
            auxColorBlock.pressedColor = blinked? baseColor : Color.white;
            button.colors = auxColorBlock;
            blinked = !blinked;
            timer = 0.0f;
        }
    }
}
