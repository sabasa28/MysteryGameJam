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
    [SerializeField] TextEntry notDoneWithZoneChat;
    [SerializeField] TextEntry doneWithZoneChat;
    [SerializeField] TextEntry wakeUpChat;
    public bool doneWithZone = false;
    [SerializeField] TextEntry firstDocChat;
    [SerializeField] TextEntry secondDocChat;
    [SerializeField] TextEntry newMessagesChat;
    [SerializeField] string astrounautName;
    [SerializeField] string unknownName;
    [SerializeField] string AIName;
    EventTriggerBase eventToTriggerAfterChat = null;

    #region singleton
    static ChatManager instance;
    public static ChatManager Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    #endregion singleton

    enum TextState
    {
        buildingText,
        textBuilt,
        notInTextMode
    }
    TextState textState = TextState.notInTextMode;

    private void Start()
    {
        //StartDisplayingTextEntry(testText);
    }

    public void StartDisplayingTextEntry(TextEntry textEntry, EventTriggerBase newEventToTriggerAfterChat = null)
    {
        eventToTriggerAfterChat = newEventToTriggerAfterChat;
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
        if (Input.anyKeyDown && !GameplayController.Get().IsOptionsMenuOpen())
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                return;
            }
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
                    if (eventToTriggerAfterChat)
                    {
                        eventToTriggerAfterChat.TriggerEvent();
                        eventToTriggerAfterChat = null;
                    }
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
            case Speaker.Unknown:
                textField.fontStyle = FontStyles.Normal;
                currentText = unknownName + ":\n" + currentText;
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

    public void PlayNotDoneWithZoneChat()
    {
        StartDisplayingTextEntry(notDoneWithZoneChat);
    }

    public void PlayDoneWithZoneChat()
    {
        if (!doneWithZone)
        {
            doneWithZone = true;
            if (GameplayController.Get().GetCurrentZone().zoneDoneChat != null)
            {
                StartDisplayingTextEntry(GameplayController.Get().GetCurrentZone().zoneDoneChat);
            }
        }
    }

    public void PlayFistDocChat()
    {
        StartDisplayingTextEntry(firstDocChat);
    }

    public void PlaySecondDocChat()
    {
        StartDisplayingTextEntry(secondDocChat);
    }

    public void PlayWakeUpChat()
    {
        StartDisplayingTextEntry(wakeUpChat);
    }
    public void PlayNewMessagesIgnoredChat()
    {
        StartDisplayingTextEntry(newMessagesChat);
    }

    public bool IsInTextMode()
    {
        return textState != TextState.notInTextMode;
    }

}
