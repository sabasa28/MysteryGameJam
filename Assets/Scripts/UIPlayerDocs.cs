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
    List<UIDocument> uIDocuments = new();
    List<Document> documents;
    List<Document> sortedDocs = new();
    [SerializeField] Transform docsParent;
    [SerializeField] GameObject readingDoc;
    [SerializeField] TextMeshProUGUI readingDocTitle;
    [SerializeField] TextMeshProUGUI readingDocFound;
    [SerializeField] TextMeshProUGUI readingDocContent;

    private void OnEnable()
    {
        SetReadingDocActiveState(false);
        SetLogActiveState(false);
        SetDocumentListActiveState(false);
    }

    public void SetDocumentListActiveState(bool state)
    {
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
                uIDocument.GetComponent<Button>().onClick.AddListener(delegate { InitializeReadingDocument(uIDocument); });
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
        }
    }

    public void SetLogActiveState(bool state)
    {
        log.SetActive(state);
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
        SetReadingDocActiveState(true);
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

}
