using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class DocumentManager : MonoBehaviour
{
    #region singletonStuff
    static DocumentManager instance;
    public static DocumentManager Get()
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
    #endregion

    [SerializeField] List<string> learnableWords = new();
    [SerializeField] List<string> wordsLearned = new();
    [SerializeField] List<string> redList = new();
    [SerializeField] List<string> greenListNAMES = new();
    [SerializeField] List<string> greenListSECRETS = new();
    [SerializeField] string censoredText; //debug
    int knowledgeLevel = 0;
    [SerializeField] List<Document> documentsRead = new();
    [SerializeField] List<LogEntry> logsFound = new();
    string[] separators = new string[] { ",", ".", "!", " ", "?", "\'s", "-", "\n" };
    [SerializeField] TextMeshProUGUI testText;
    [SerializeField] PersistentData persistentData;
    [SerializeField] UIHelmet uiHelmet;

    [Serializable]
    struct WordsReplacement : IComparable<WordsReplacement>
    {
        public string word;
        public bool censor;

        public WordsReplacement(string inWord, bool inCensor)
        {
            word = inWord;
            censor = inCensor;
        }
        public int CompareTo(WordsReplacement other)
        {
            return word.Length > other.word.Length? -1 : 1;
        }
    }
    [SerializeField]
    List<WordsReplacement> wordReplacements = new();

    private void Start()
    {
        if (persistentData.persistentDocsData.HasData())
        {
            wordsLearned = persistentData.persistentDocsData.wordsLearned;
            learnableWords = persistentData.persistentDocsData.learnableWords;
            documentsRead = persistentData.persistentDocsData.documentsRead;
            knowledgeLevel = persistentData.persistentDocsData.knowledgeLevel;
            logsFound = persistentData.persistentDocsData.logsFound;
        }
        else
        {
            foreach (string separator in separators)
            {
                if (separator != " " && separator != "\n")
                {
                    AddToLearnableWords(separator);
                }
            }
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (knowledgeLevel<100)
            {
                SetKnowledgeLevel(knowledgeLevel + 10);
                if (documentsRead.Count > 0)
                {
                    testText.SetText(documentsRead[0].currentText);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            LearnNames();
            if (documentsRead.Count > 0)
            {
                testText.SetText(documentsRead[0].currentText);
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            LearnSecrets();
            if (documentsRead.Count > 0)
            {
                testText.SetText(documentsRead[0].currentText);
            }
        }
    }

    void SetKnowledgeLevel(int knowledge)
    {
        knowledgeLevel = knowledge;

        List<string> actualLernableWords = new();
        actualLernableWords.AddRange(learnableWords);
        List<string> actualWordsLearned = new();
        actualWordsLearned.AddRange(wordsLearned);

        List<string> forbidenWords = new();
        forbidenWords.AddRange(redList);
        forbidenWords.AddRange(greenListNAMES);
        forbidenWords.AddRange(greenListSECRETS);

        foreach (string word in forbidenWords)
        {
            actualLernableWords.Remove(word.ToLower());
            actualWordsLearned.Remove(word.ToLower());
        }

        int totalWords = actualLernableWords.Count + actualWordsLearned.Count;
        int wordsPercentage = (totalWords * knowledgeLevel / 100) - actualWordsLearned.Count;
        List<int> randoms = new();
        List<int> posibleRandoms = new();

        for (int i = 0; i < actualLernableWords.Count; i++)
        {
            posibleRandoms.Add(i);
        }
        int wordsToLearn = Mathf.Min(wordsPercentage, posibleRandoms.Count);
        for (int i = 0; i < wordsToLearn; i++)
        {
            int rand = UnityEngine.Random.Range(0, posibleRandoms.Count - 1);
            randoms.Add(posibleRandoms[rand]);
            posibleRandoms.RemoveAt(rand);
        }
        randoms.Sort();
        for (int i = randoms.Count -1; i >= 0; i--)
        {
            wordsLearned.Add(actualLernableWords[randoms[i]]);
            learnableWords.Remove(actualLernableWords[randoms[i]]);
            actualLernableWords.RemoveAt(randoms[i]);
        }
        foreach (Document doc in documentsRead)
        {
            CensorDocument(doc);
        }
    }

    public void AddDocumentWordsToLearnable(Document document)
    {
        if (documentsRead.Contains(document))
        {
            return;
        }
        document.fullText = document.fullText.Replace("\\n", "\n");
        document.read = false;
        documentsRead.Add(document);
        foreach (string word in document.fullText.Split(separators, StringSplitOptions.RemoveEmptyEntries))
        {
            AddToLearnableWords(word.ToLower());
        }
        SetKnowledgeLevel(knowledgeLevel + 10);
        persistentData.UpdatePersistentDocsData(wordsLearned, learnableWords, documentsRead, knowledgeLevel);
        uiHelmet.DisplayNewDocNotif();
    }

    public void LearnNames()
    {
        foreach (string word in greenListNAMES)
        {
            wordsLearned.Add(word.ToLower());
            learnableWords.Remove(word.ToLower());
        }
        foreach (Document doc in documentsRead)
        {
            CensorDocument(doc);
        }
    }

    public void LearnSecrets()
    { 
        foreach (string word in greenListSECRETS)
        {
            wordsLearned.Add(word.ToLower());
            learnableWords.Remove(word.ToLower());
        }
        foreach (Document doc in documentsRead)
        {
            CensorDocument(doc);
        }
    }

    void AddToLearnableWords(string stringToAdd)
    {
        if (!learnableWords.Contains(stringToAdd) && !wordsLearned.Contains(stringToAdd))
        {
            learnableWords.Add(stringToAdd);
        }
    }

    List<string> SortByLength(List<string> stringList) //lo descubri como Colón //maybe remover + linq
    {
        var sortedStringList = stringList
            .OrderByDescending(n => n.Length)
            .ToList();

        return sortedStringList;
    }

    void CensorDocument(Document document)
    {
        string textToCensor = document.fullText;
        foreach (string word in learnableWords)
        {
            wordReplacements.Add(new WordsReplacement(word, true));
        }
        foreach (string word in wordsLearned)
        {
            wordReplacements.Add(new WordsReplacement(word, false));
        }

        wordReplacements.Sort();


        List<int> processedIndex = new();
        List<int> allIndexFound = new();
        foreach (WordsReplacement wordReplacement in wordReplacements)
        {
            int wordLength = wordReplacement.word.Length;
            int indexFound = textToCensor.IndexOf(wordReplacement.word, StringComparison.InvariantCultureIgnoreCase);
            while (indexFound != -1)
            {
                allIndexFound.Add(indexFound);
                indexFound++;
                indexFound = textToCensor.IndexOf(wordReplacement.word, indexFound, StringComparison.InvariantCultureIgnoreCase);
            }

            //puede ser que una de las instancias de la palabra no deberia ser censurada y la otra si, porque tal vez una esta incluida dentro de otra palabra que ya se sabe que no deber ser censurada
            //y otra no.

            for (int j = allIndexFound.Count-1; j >= 0; j--)
            {
                bool allIndexAreUnprocessed = true;
                for (int i = 0; i < wordLength; i++)
                {
                    if (processedIndex.Contains(allIndexFound[j] + i))
                    {
                        allIndexAreUnprocessed = false;
                        break;
                    }
                }
                if (!allIndexAreUnprocessed)
                {
                    allIndexFound.Remove(allIndexFound[j]);
                }
            }

            foreach (int index in allIndexFound)
            {
                for (int i = 0; i < wordLength; i++)
                {
                    processedIndex.Add(index + i);
                }
            }

            if (wordReplacement.censor)
            {
                string replacement = "";
                for (int i = 0; i < wordLength; i++)
                {
                    replacement += "*";
                }
                foreach (int index in allIndexFound)
                {
                    textToCensor = textToCensor.Remove(index, wordLength);
                    textToCensor = textToCensor.Insert(index, replacement);
                }
            }
            
            allIndexFound.Clear();
        }
        wordReplacements.Clear();
        document.currentText = textToCensor;
        censoredText = textToCensor;
    }

    public List<Document> GetReadDocuments()
    {
        return documentsRead;
    }

    public void AddLogToFoundLogs(LogEntry newLog)
    {
        if (!logsFound.Contains(newLog))
        {
            newLog.read = false;
            logsFound.Add(newLog);
            persistentData.UpdatePersistentLogsData(logsFound);
            uiHelmet.DisplayNewLogNotif();
        }
    }

    public List<LogEntry> GetFoundLogs()
    {
        return logsFound;
    }

}
