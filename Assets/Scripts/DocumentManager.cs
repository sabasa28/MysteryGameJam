using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DocumentManager : MonoBehaviour
{
    
    [SerializeField] List<string> learnableWords = new();
    [SerializeField] List<string> wordsLearned = new();
    [SerializeField] string censoredText;
    string uncensoredText;
    int knowledgeLevel = 0;
    int testDocsIndex = 0;
    [SerializeField] Document[] testDocs;
    [SerializeField] List<Document> documentsRead = new();
    string[] separators = new string[] { ",", ".", "!", " ", "?", "\'s", "-", "\n" };

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
        foreach (string separator in separators)
        {
            if (separator != " " && separator != "\n")
            {
                AddToLearnableWords(separator);
            }
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (testDocsIndex < testDocs.Length)
            {
                AddDocumentWordsToLearnable(testDocs[testDocsIndex]);
                testDocsIndex++;
            }
        }
    }

    void SetKnowledgeLevel(int knowledge)
    {
        knowledgeLevel = knowledge;
        int totalWords = learnableWords.Count + wordsLearned.Count;
        int wordsPercentage = (totalWords * knowledgeLevel / 100) - wordsLearned.Count;
        List<int> randoms = new();
        List<int> posibleRandoms = new();
        for (int i = 0; i < learnableWords.Count; i++)
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
            wordsLearned.Add(learnableWords[randoms[i]]);
            learnableWords.RemoveAt(randoms[i]);
        }
    }

    void AddDocumentWordsToLearnable(Document document)
    {
        documentsRead.Add(document);
        foreach (string word in document.fullText.Split(separators, StringSplitOptions.RemoveEmptyEntries))
        {
            AddToLearnableWords(word);
        }
        uncensoredText = document.fullText;
        SetKnowledgeLevel(knowledgeLevel + 10);

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
            int indexFound = textToCensor.IndexOf(wordReplacement.word);
            while (indexFound != -1)
            {
                allIndexFound.Add(indexFound);
                indexFound++;
                indexFound = textToCensor.IndexOf(wordReplacement.word, indexFound);
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

}
