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
    [SerializeField] List<Document> documentsFound = new();
    [SerializeField] List<LogEntry> logsFound = new();
    string[] separators = new string[] { ",", ".", "!", " ", "?", "\'s", "-", "\n", "\"" };
    [SerializeField] PersistentData persistentData;
    [SerializeField] UIHelmet uiHelmet;
    [SerializeField] List<LogEntry> preexistentLogs = new();
    [SerializeField] int knowledgePerDoc;

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
            wordsLearned = new List<string>(persistentData.persistentDocsData.wordsLearned);
            learnableWords = new List<string>(persistentData.persistentDocsData.learnableWords);
            documentsFound = new List<Document>(persistentData.persistentDocsData.documentsFound);
            knowledgeLevel = persistentData.persistentDocsData.knowledgeLevel;
            logsFound = new List<LogEntry>(persistentData.persistentDocsData.logsFound);
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
        foreach (LogEntry log in preexistentLogs)
        {
            AddLogToFoundLogs(log, true);
            persistentData.AddLogToExisting(log);
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
        foreach (Document doc in documentsFound)
        {
            CensorDocument(doc);
        }
    }

    public void AddDocumentWordsToLearnable(Document document)
    {
        LevelsManager.Get().persistentData.PlayerFoundAnyDoc();
        if (documentsFound.Contains(document))
        {
            return;
        }
        if (documentsFound.Count == 0)
        {
            ChatManager.Get().PlayFistDocChat();
        }
        if (documentsFound.Count == 1)
        {
            ChatManager.Get().PlaySecondDocChat();
        }
        document.fullText = document.fullText.Replace("\\n", "\n");
        document.read = false;
        documentsFound.Add(document);
        foreach (string word in document.fullText.Split(separators, StringSplitOptions.RemoveEmptyEntries))
        {
            AddToLearnableWords(word.ToLower());
        }
        SetKnowledgeLevel(LevelsManager.Get().persistentData.PlayerFoundAnyDoc()? knowledgeLevel + knowledgePerDoc : knowledgeLevel);
        persistentData.UpdatePersistentDocsData(wordsLearned, learnableWords, documentsFound, knowledgeLevel);
        uiHelmet.DisplayNewDocNotif();
    }

    public void LearnNames()
    {
        foreach (string word in greenListNAMES)
        {
            wordsLearned.Add(word.ToLower());
            learnableWords.Remove(word.ToLower());
        }
        persistentData.UpdatePersistentDocsData(wordsLearned, learnableWords, documentsFound, knowledgeLevel);
        foreach (Document doc in documentsFound)
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
        persistentData.UpdatePersistentDocsData(wordsLearned, learnableWords, documentsFound, knowledgeLevel);
        foreach (Document doc in documentsFound)
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
        return documentsFound;
    }

    public void AddLogToFoundLogs(LogEntry newLog, bool isPreexisting = false)
    {
        if (!logsFound.Contains(newLog))
        {
            newLog.logText = newLog.logText.Replace("\\n", "\n");
            newLog.read = false;
            logsFound.Add(newLog);
            persistentData.UpdatePersistentLogsData(logsFound);
            if (!isPreexisting)
            {
                uiHelmet.DisplayNewLogNotif();
            }
        }
    }

    public List<LogEntry> GetFoundLogs()
    {
        return logsFound;
    }

    public bool AreAllFoundDocumentsRead()
    {
        for (int i = documentsFound.Count-1; i >= 0; i--)
        {
            if (documentsFound[i].read == false)
            {
                return false;
            }
        }
        return true;
    }
    public bool AreAllFoundLogsRead()
    {
        return logsFound[logsFound.Count - 1].read; //instead of checking all we just check the last one since logs are all read automatically when the travelers log is opened
    }
}
