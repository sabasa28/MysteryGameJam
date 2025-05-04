using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerDocs : MonoBehaviour
{
    [SerializeField] GameObject documentList;
    [SerializeField] GameObject log;
    [SerializeField] UIDocument uiDocumentPrefab;
    [SerializeField] UILogEntry uiLogEntryPrefab;
    List<UIDocument> uIDocuments = new();
    List<UILogEntry> uiLogEntries = new();
    List<Document> documents;
    List<LogEntry> logs;
    List<Document> sortedDocs = new();
    [SerializeField] Transform docsParent;
    [SerializeField] Transform logsParent;
    [SerializeField] GameObject readingDoc;
    [SerializeField] GameObject initialOptions;
    [SerializeField] TextMeshProUGUI readingDocTitle;
    [SerializeField] TextMeshProUGUI readingDocFound;
    [SerializeField] TextMeshProUGUI readingDocContent;
    [SerializeField] GameObject logImageDisplay;

    private void OnEnable()
    {
        initialOptions.SetActive(true);
        SetReadingDocActiveState(false);
        SetLogActiveState(false);
        SetDocumentListActiveState(false);
    }

    public void SetDocumentListActiveState(bool state)
    {
        initialOptions.SetActive(!state);
        documentList.SetActive(state);
        if (!state)
        {
            return;
        }

        documents = DocumentManager.Get().GetReadDocuments();
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
            uIDocuments[i].titleText.text = "Document " + documents.FindIndex(a => a == sortedDocs[i]);
            uIDocuments[i].placeFoundText.text = sortedDocs[i].placeFoundText;
            uIDocuments[i].unreadIndicator.SetActive(!sortedDocs[i].read);
        }
    }

    public void SetLogActiveState(bool state)
    {
        initialOptions.SetActive(!state);
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
            return;
        }

        logs = DocumentManager.Get().GetFoundLogs();
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
            }
        }
        InitializeUILogEntries();
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
        sortedDocs.Sort((x,y) => x.orderToDisplay - y.orderToDisplay);
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

}
