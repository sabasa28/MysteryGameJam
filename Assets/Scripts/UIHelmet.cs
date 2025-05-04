using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHelmet : MonoBehaviour
{
    [SerializeField] Image newLogPanel;
    [SerializeField] TextMeshProUGUI newLogText;
    [SerializeField] Image newDocPanel;
    [SerializeField] TextMeshProUGUI newDocText;
    [SerializeField] float movementTime;
    [SerializeField] float stillTime;
    [SerializeField] float xStartOffset;
    Coroutine logCoroutine;
    Coroutine docCoroutine;

    public void DisplayNewLogNotif()
    {
        if (logCoroutine != null)
        {
            StopCoroutine(logCoroutine);
        }
        logCoroutine = StartCoroutine(DisplayNotif(newLogPanel, newLogText));
    }

    public void DisplayNewDocNotif()
    {
        if (docCoroutine != null)
        {
            StopCoroutine(docCoroutine);
        }
        docCoroutine = StartCoroutine(DisplayNotif(newDocPanel, newDocText));
    }
    IEnumerator DisplayNotif(Image image, TextMeshProUGUI text)
    {
        float timer = 0.0f;
        Color imageColor = image.color;
        Color textColor = text.color;
        Color imageColorNoAlpha = new Color(imageColor.r, imageColor.g, imageColor.b, 0.0f);
        Color textColorNoAlpha = new Color(textColor.r, textColor.g, textColor.b, 0.0f);
        image.gameObject.SetActive(true);
        Transform panelTransform = image.transform;
        Vector3 endPos = panelTransform.localPosition;
        Vector3 initialPos = panelTransform.localPosition + new Vector3(xStartOffset, 0.0f, 0.0f);
        panelTransform.localPosition = initialPos;
        float t = 0.0f;
        while (timer < movementTime)
        {
            timer += Time.deltaTime;
            t = timer / movementTime;
            image.color = Color.Lerp(imageColorNoAlpha, imageColor, t);
            text.color = Color.Lerp(textColorNoAlpha, textColor, t);
            panelTransform.localPosition = Vector3.Lerp(initialPos, endPos, t);
            yield return null;
        }
        image.color = imageColor;
        text.color = textColor;
        panelTransform.localPosition = endPos;
        yield return new WaitForSeconds(stillTime);
        timer = 0.0f;
        while (timer < movementTime)
        {
            timer += Time.deltaTime;
            t = timer / movementTime;
            image.color = Color.Lerp(imageColor, imageColorNoAlpha, t);
            text.color = Color.Lerp(textColor, textColorNoAlpha, t);
            panelTransform.localPosition = Vector3.Lerp(endPos, initialPos, t);
            yield return null;
        }
        image.color = imageColor;
        text.color = textColor;
        panelTransform.localPosition = endPos;
        image.gameObject.SetActive(false);
    }
}
