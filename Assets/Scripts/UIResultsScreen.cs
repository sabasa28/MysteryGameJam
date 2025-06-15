using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIResultsScreen : MonoBehaviour
{
    float timer = 0.0f;
    bool takingInput = false;
    [SerializeField] float timeBeforeTakingInput;
    [SerializeField] TextMeshProUGUI docsText;
    [SerializeField] TextMeshProUGUI logsText;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PersistentData persistentData = LevelsManager.Get().persistentData;
        docsText.text = "You found " + persistentData.persistentDocsData.documentsRead.Count + " out of " + persistentData.persistentDocsData.existingDocuments.Count + " documents.";
        logsText.text = "You found " + persistentData.persistentDocsData.logsFound.Count + " out of " + persistentData.persistentDocsData.existingLogs.Count + " logs.";
    }

    // Update is called once per frame
    void Update()
    {
        if (!takingInput)
        {
            timer += Time.deltaTime;
            if (timer >= timeBeforeTakingInput)
            {
                takingInput = true;
            }
            return;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}
