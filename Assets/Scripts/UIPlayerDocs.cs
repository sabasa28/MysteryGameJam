using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerDocs : MonoBehaviour
{
    [SerializeField] GameObject documentList;
    [SerializeField] GameObject log;
    [SerializeField] ScrollRect logScroll;
    [SerializeField] UIDocument uiDocumentPrefab;
    [SerializeField] UILogEntry uiLogEntryPrefab;
    List<UIDocument> uIDocuments = new();
    List<UILogEntry> uiLogEntries = new();
    List<Document> documents;
    List<LogEntry> logs;
    List<Document> sortedDocs = new();
    [SerializeField] Transform docsParent;
    [SerializeField] RectTransform logsParent;
    [SerializeField] GameObject logsSpacer;
    [SerializeField] GameObject readingDoc;
    [SerializeField] GameObject initialOptions;
    [SerializeField] TextMeshProUGUI readingDocTitle;
    [SerializeField] TextMeshProUGUI readingDocFound;
    [SerializeField] TextMeshProUGUI readingDocContent;
    [SerializeField] GameObject logImageDisplay;
    bool firstDisplay = true;
    [SerializeField] GameObject tutorialScreen;
    [SerializeField] GameObject sortingButton;
    [SerializeField] TextMeshProUGUI sortingButtonText;
    [SerializeField] GameObject anyDocUnreadIndicator;
    [SerializeField] GameObject anyLogUnreadIndicator;
    UIDocument lastSelectedDoc = null;

    private void OnEnable()
    {
        DocumentManager documentManager = DocumentManager.Get();
        anyDocUnreadIndicator.SetActive(!documentManager.AreAllFoundDocumentsRead());
        anyLogUnreadIndicator.SetActive(!documentManager.AreAllFoundLogsRead());
        SetReadingDocActiveState(false);
        SetLogActiveState(false);
        SetDocumentListActiveState(false);

        if (tutorialScreen && tutorialScreen.activeInHierarchy)
        {
            bool showTutorial = firstDisplay && !LevelsManager.Get().GoingUp;
            ChangeTutorialScreenState(showTutorial);
            firstDisplay = false;
        }
    }

    private void OnDisable()
    {
        SetLogActiveState(false);
    }

    public void SetDocumentListActiveState(bool state)
    {
        initialOptions.SetActive(!state);
        SetReadingDocActiveState(false);
        DocumentManager documentManager = DocumentManager.Get();
        documentList.SetActive(state);
        sortingButton.SetActive(LevelsManager.Get().persistentData.calendarDiscovered);
        if (!state)
        {
            anyDocUnreadIndicator.SetActive(!documentManager.AreAllFoundDocumentsRead());
            return;
        }

        documents = documentManager.GetReadDocuments();
        if (uIDocuments.Count > documents.Count) //no se si esto va a pasar pero por si acaso
        {
            for (int i = uIDocuments.Count - 1; i > documents.Count - 1; i--)
            {
                Destroy(uIDocuments[i].gameObject);
                uIDocuments.RemoveAt(i);
            }
        }
        else if (uIDocuments.Count < documents.Count)
        {
            for (int i = uIDocuments.Count; i < documents.Count; i++)
            {
                UIDocument uIDocument = Instantiate(uiDocumentPrefab, docsParent);
                uIDocument.SetDefaultColor();
                uIDocument.button.onClick.AddListener(delegate { InitializeReadingDocument(uIDocument); });
                uIDocuments.Add(uIDocument);
            }
        }
        InitializeDocsUI();
    }

    void InitializeDocsUI()
    {
        UpdateDocsSortedByDate();
        for (int i = 0; i < uIDocuments.Count; i++)
        {
            uIDocuments[i].ResetToUnselected();
            uIDocuments[i].titleText.text = "Document " + documents.FindIndex(a => a == sortedDocs[i]);
            uIDocuments[i].placeFoundText.text = sortedDocs[i].placeFoundText;
            uIDocuments[i].unreadIndicator.SetActive(!sortedDocs[i].read);
        }
        if (lastSelectedDoc)
        {
            lastSelectedDoc.ResetToUnselected();
        }
    }

    public void SetLogActiveState(bool state)
    {
        initialOptions.SetActive(!state);
        DocumentManager documentManager = DocumentManager.Get();
        log.SetActive(state);
        logImageDisplay.SetActive(!state);
        if (!state)
        {
            for (int i = 0; i < uiLogEntries.Count; i++)
            {
                if (logs.Count > i)
                {
                    if (!logs[i].read)
                    {
                        logs[i].read = true;
                    }
                }
            }
            anyLogUnreadIndicator.SetActive(!documentManager.AreAllFoundLogsRead()); //tecnicamente siempre false pero igual es barato, mas legible y menos propenso a bugs en el futuro
            return;
        }

        logs = documentManager.GetFoundLogs();
        if (uiLogEntries.Count > logs.Count) //no se si esto va a pasar pero por si acaso
        {
            for (int i = uiLogEntries.Count - 1; i > logs.Count - 1; i--)
            {
                Destroy(uiLogEntries[i].gameObject);
                uiLogEntries.RemoveAt(i);
            }
        }
        else if (uiLogEntries.Count < logs.Count)
        {
            for (int i = uiLogEntries.Count; i < logs.Count; i++)
            {
                UILogEntry uilogEntry = Instantiate(uiLogEntryPrefab, logsParent);
                uilogEntry.image.onClick.AddListener(delegate { SetLogImageAndDisplay(uilogEntry.image.image); });
                uiLogEntries.Add(uilogEntry);
                Instantiate(logsSpacer, logsParent);
            }
        }
        InitializeUILogEntries();

        LayoutRebuilder.ForceRebuildLayoutImmediate(logsParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(logsParent); // esta es la cosa mas estupida que hice pero 2 funcionan y 1 no
        if (LevelsManager.Get().persistentData.PlayerReadAnyLog())
        {
            logScroll.verticalNormalizedPosition = 0.0f;
        }
        else
        {
            logScroll.verticalNormalizedPosition = 1.0f;
        }
    }

    public void InitializeReadingDocument(UIDocument document)
    {
        int foundIndex = uIDocuments.FindIndex(a => a == document);
        if (uIDocuments.Count < foundIndex || sortedDocs.Count < foundIndex)
        {
            //algo anda mal aca
            return;
        }
        readingDocTitle.text = uIDocuments[foundIndex].titleText.text;
        readingDocFound.text = uIDocuments[foundIndex].placeFoundText.text;
        readingDocContent.text = sortedDocs[foundIndex].currentText;
        if (!sortedDocs[foundIndex].read)
        {
            sortedDocs[foundIndex].read = true;
        }

        if (lastSelectedDoc)
        {
            lastSelectedDoc.ResetToUnselected();
        }
        lastSelectedDoc = document;
        document.SetAsSelected();

        SetReadingDocActiveState(true);
    }

    void InitializeUILogEntries()
    {
        for (int i = 0; i < uiLogEntries.Count; i++)
        {
            uiLogEntries[i].text.text = logs[i].logText;
            uiLogEntries[i].image.image.sprite = logs[i].attachedPhoto;
            if (logs.Count > i)
            {
                uiLogEntries[i].unreadIndicator.SetActive(!logs[i].read);
            }
        }
    }

    public void SetReadingDocActiveState(bool state)
    {
        readingDoc.SetActive(state);
    }

    public void UpdateDocsSortedByDate()
    {
        sortedDocs = new List<Document>(documents);
        bool sortByDate = LevelsManager.Get().persistentData.persistentDocsData.docsSortedByDate;
        sortingButtonText.text = sortByDate ? "Date" : "Found";
        if (sortByDate)
        {
            sortedDocs.Sort((x,y) => x.orderToDisplay - y.orderToDisplay);
        }
    }

    public void SetLogImageAndDisplay(Image newImage)
    {
        Sprite sprite = newImage.sprite;
        Image image = logImageDisplay.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = sprite;
        }
        StartCoroutine(ScaleLogImage());
    }

    IEnumerator ScaleLogImage()
    {
        Button button = logImageDisplay.GetComponent<Button>();
        if (button == null)
        {
            yield break;
        }
        logImageDisplay.SetActive(true);
        button.interactable = false;
        float timer = 0.0f;
        float timeToScale = 0.4f;
        while (timer < timeToScale)
        {
            timer += Time.deltaTime;
            logImageDisplay.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timer / timeToScale);
            yield return null;
        }
        button.interactable = true;
        logImageDisplay.transform.localScale = Vector3.one;
    }

    public void SwitchSortingCriteria()
    {
        LevelsManager levelManager = LevelsManager.Get();
        bool previousValue = levelManager.persistentData.persistentDocsData.docsSortedByDate;
        levelManager.persistentData.persistentDocsData.docsSortedByDate = !previousValue;
        InitializeDocsUI();
    }

    public void ChangeTutorialScreenState(bool newState)
    {
        tutorialScreen.SetActive(newState);
        initialOptions.SetActive(!newState);
    }
}
