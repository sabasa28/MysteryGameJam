using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHelmet : MonoBehaviour
{
    [SerializeField] Image newLogPanel;
    Vector3 initialNewLogPanelPos;
    Color initialNewLogColor;
    [SerializeField] TextMeshProUGUI newLogText;
    [SerializeField] Image newDocPanel;
    Vector3 initialNewDocPanelPos;
    Color initialNewDocColor;
    [SerializeField] TextMeshProUGUI newDocText;
    [SerializeField] float movementTime;
    [SerializeField] float stillTime;
    [SerializeField] float xStartOffset;
    Coroutine logCoroutine;
    Coroutine docCoroutine;

    private void Awake()
    {
        initialNewLogPanelPos = newLogPanel.transform.localPosition;
        initialNewLogColor = newLogPanel.color;
        initialNewDocPanelPos = newDocPanel.transform.localPosition;
        initialNewDocColor = newDocPanel.color;
    }

    public void DisplayNewLogNotif()
    {
        if (logCoroutine != null)
        {
            StopCoroutine(logCoroutine);
        }
        logCoroutine = StartCoroutine(DisplayNotif(newLogPanel, newLogText, initialNewLogPanelPos, initialNewLogColor));
    }

    public void DisplayNewDocNotif()
    {
        if (docCoroutine != null)
        {
            StopCoroutine(docCoroutine);
        }
        docCoroutine = StartCoroutine(DisplayNotif(newDocPanel, newDocText, initialNewDocPanelPos, initialNewDocColor));
    }
    IEnumerator DisplayNotif(Image image, TextMeshProUGUI text, Vector3 endPos, Color targetImageColor)
    {
        float timer = 0.0f;
        Color targetTextColor = text.color;
        targetTextColor.a = 1;
        Color imageColorNoAlpha = new Color(targetImageColor.r, targetImageColor.g, targetImageColor.b, 0.0f);
        Color textColorNoAlpha = new Color(targetTextColor.r, targetTextColor.g, targetTextColor.b, 0.0f);
        image.gameObject.SetActive(true);
        Transform panelTransform = image.transform;
        Vector3 initialPos = endPos + new Vector3(xStartOffset, 0.0f, 0.0f);
        panelTransform.localPosition = initialPos;
        float t = 0.0f;
        while (timer < movementTime)
        {
            timer += Time.deltaTime;
            t = timer / movementTime;
            image.color = Color.Lerp(imageColorNoAlpha, targetImageColor, t);
            text.color = Color.Lerp(textColorNoAlpha, targetTextColor, t);
            panelTransform.localPosition = Vector3.Lerp(initialPos, endPos, t);
            yield return null;
        }
        image.color = targetImageColor;
        text.color = targetTextColor;
        panelTransform.localPosition = endPos;
        yield return new WaitForSeconds(stillTime);
        timer = 0.0f;
        while (timer < movementTime)
        {
            timer += Time.deltaTime;
            t = timer / movementTime;
            image.color = Color.Lerp(targetImageColor, imageColorNoAlpha, t);
            text.color = Color.Lerp(targetTextColor, textColorNoAlpha, t);
            panelTransform.localPosition = Vector3.Lerp(endPos, initialPos, t);
            yield return null;
        }
        image.color = targetImageColor;
        text.color = targetTextColor;
        panelTransform.localPosition = endPos;
        image.gameObject.SetActive(false);
    }
}
