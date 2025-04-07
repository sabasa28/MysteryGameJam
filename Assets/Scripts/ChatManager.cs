using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatManager : MonoBehaviour
{
    string currentText;
    TextLine currentTextLine;
    TextEntry currentTextEntry;
    bool skipTextBuilding = false;
    [SerializeField] float timeBetweenCharacter;
    int currentCharacter = 0;
    int currentLineIndex = 0;
    [SerializeField] TextMeshProUGUI textField;
    [SerializeField] GameObject textPanel;
    [SerializeField] TextEntry testText;
    [SerializeField] string astrounautName;
    [SerializeField] string AIName;
    bool inputEnabled;
    enum TextState
    {
        buildingText,
        textBuilt,
        notInTextMode
    }
    TextState textState = TextState.notInTextMode;

    private void Start()
    {
        StartDisplayingTextEntry(testText);
    }

    public void StartDisplayingTextEntry(TextEntry textEntry)
    {
        GameplayController.Get().ChangeInputState(GameplayController.InputState.Chat);
        textPanel.SetActive(true);
        currentTextEntry = textEntry;
        currentLineIndex = 0;
        DisplayTextLine(currentTextEntry.textLine[currentLineIndex]);
    }

    void DisplayTextLine(TextLine textlineToDisplay)
    {
        currentTextLine = textlineToDisplay;
        StartCoroutine(GraduallyDisplayText());
    }

    public void Update()
    {
        if (textState == TextState.notInTextMode)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (textState == TextState.buildingText)
            {
                skipTextBuilding = true;
            }
            else
            {
                if (currentTextEntry.textLine.Length > currentLineIndex + 1)
                {
                    currentLineIndex++;
                    DisplayTextLine(currentTextEntry.textLine[currentLineIndex]);
                }
                else
                {
                    GameplayController.Get().ChangeInputState(GameplayController.InputState.Movement);
                }
            }
        }
    }

    IEnumerator GraduallyDisplayText()
    {
        currentCharacter = 0;
        currentText = currentTextLine.text;
        textState = TextState.buildingText;
        textField.text = "";
        switch (currentTextLine.speaker)
        {
            case Speaker.AstronautSpeak:
                textField.fontStyle = FontStyles.Normal;
                currentText = astrounautName + ":\n" + currentText;
                break;
            case Speaker.AI:
                textField.fontStyle = FontStyles.Normal;
                currentText = AIName + ":\n" + currentText;
                break;
            case Speaker.AstronautThought:
                textField.fontStyle = FontStyles.Italic;
                break;
            default:
                break;
        }
        while (currentCharacter < currentText.Length)
        {
            if (skipTextBuilding)
            {
                textField.text = currentText;
                break;
            }
            textField.text += currentText[currentCharacter];
            yield return new WaitForSeconds(timeBetweenCharacter);
            currentCharacter++;
            yield return null;
        }
        currentText = null;
        skipTextBuilding = false;
        textState = TextState.textBuilt;
    }

    public void TurnOffTextMode()
    {
        textPanel.SetActive(false);
        textState = TextState.notInTextMode;
    }
}
