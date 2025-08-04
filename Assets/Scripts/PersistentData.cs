using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Persistent Data", menuName = "Persistent Data")]
public class PersistentData : ScriptableObject
{
    [Serializable]
    public struct BeaconsInLevel
    {
        public BeaconsInLevel(Vector3 firstBeaconPos, string newSceneName)
        {
            BeaconsPos = new();
            BeaconsPos.Add(firstBeaconPos);
            SceneName = newSceneName;
        }
        public List<Vector3> BeaconsPos;
        public string SceneName;
    }
    public List<BeaconsInLevel> beaconsInLevels = new();

    [Serializable]
    public struct PersistentDocsData
    {
        public List<string> learnableWords;
        public List<string> wordsLearned;
        public List<Document> documentsFound;
        public List<Document> existingDocuments;
        public List<LogEntry> logsFound;
        public List<LogEntry> existingLogs;
        public bool docsSortedByDate;
        public int knowledgeLevel;
        public void InitializeData()
        {
            if (learnableWords != null)
            {
                learnableWords.Clear();
            }
            else
            {
                learnableWords = new();
            }
            if (wordsLearned != null)
            {
                wordsLearned.Clear();
            }
            else
            {
                wordsLearned = new();
            }
            if (documentsFound != null)
            {
                documentsFound.Clear();
            }
            else
            {
                documentsFound = new();
            }
            if (existingDocuments != null)
            {
                existingDocuments.Clear();
            }
            else
            {
                existingDocuments = new();
            }
            if (logsFound != null)
            {
                logsFound.Clear();
            }
            else
            {
                logsFound = new();
            }
            if (existingLogs != null)
            {
                existingLogs.Clear();
            }
            else
            {
                existingLogs = new();
            }
            knowledgeLevel = 0;
            docsSortedByDate = false;
        }

        public bool HasData()
        {
            if (learnableWords != null && wordsLearned != null && documentsFound != null && logsFound != null)
            {
                return (learnableWords.Count > 0 || wordsLearned.Count > 0 || documentsFound.Count > 0 || logsFound.Count > 0);
            }
            else
            {
                return false;
            }
        }
    }
    public PersistentDocsData persistentDocsData = new();

    public bool flashlightOn;
    public bool helmetOn;
    public bool hookDiscovered = false;
    public bool beaconsDiscovered = false;
    public bool sonarDiscovered = false;
    public bool calendarDiscovered = false;
    public bool isReturning = false;
    public bool canEndGame = false;

    public List<TextEntry> chatsAlreadyPlayed = new();
    public void AddBeacon(Vector3 pos, string scene)
    {
        bool found = false;
        foreach (BeaconsInLevel level in beaconsInLevels)
        {
            if (level.SceneName == scene)
            {
                found = true;
                level.BeaconsPos.Add(pos);
            }
        }
        if (!found)
        {
            beaconsInLevels.Add(new BeaconsInLevel(pos, scene));
        }
    }

    public int GetBeaconsUsed()
    {
        int beacons = 0;
        foreach (BeaconsInLevel level in beaconsInLevels)
        {
            beacons += level.BeaconsPos.Count;
        }
        return beacons;
    }

    public List<Vector3> GetLevelBeaconsPos(string targetScene)
    {
        foreach (BeaconsInLevel level in beaconsInLevels)
        {
            if (level.SceneName == targetScene)
            {
                return level.BeaconsPos;
            }
        }
        return null;
    }

    public void UpdatePersistentDocsData(List<string> wordsLearned, List<string> learnableWords, List<Document> documentsRead, int knowledgeLevel)
    {
        persistentDocsData.wordsLearned = wordsLearned;
        persistentDocsData.learnableWords = learnableWords;
        persistentDocsData.documentsFound = documentsRead;
        persistentDocsData.knowledgeLevel = knowledgeLevel;
    }

    public void AddDocToExisting(Document doc)
    {
        if (!persistentDocsData.existingDocuments.Contains(doc))
        {
            persistentDocsData.existingDocuments.Add(doc);
        }
    }

    public void UpdatePersistentLogsData(List<LogEntry> logsFound)
    {
        persistentDocsData.logsFound = logsFound;
    }

    public void AddLogToExisting(LogEntry log)
    {
        if (!persistentDocsData.existingLogs.Contains(log))
        {
            persistentDocsData.existingLogs.Add(log);
        }
    }

    public void UpdateFlashlightState(bool newState)
    {
        flashlightOn = newState;
    }

    public void UpdateHelmetState(bool newState)
    {
        helmetOn = newState;
    }

    public void InitializeData()
    {
        beaconsInLevels.Clear();
        persistentDocsData.InitializeData();
        hookDiscovered = false;
        sonarDiscovered = false;
        calendarDiscovered = false;
        beaconsDiscovered = false;
        flashlightOn = false;
        helmetOn = false;
        isReturning = false;
        canEndGame = false;
        if (chatsAlreadyPlayed != null)
        {
            chatsAlreadyPlayed.Clear();
        }
        else
        {
            chatsAlreadyPlayed = new(); 
        }
    }

    public bool WasChatPlayed(TextEntry chat)
    {
        return chatsAlreadyPlayed.Contains(chat);
    }

    public bool WasLogTriggered(LogEntry log)
    {
        return persistentDocsData.logsFound.Contains(log);
    }

    public bool WasDocFound(Document doc)
    {
        return persistentDocsData.documentsFound.Contains(doc);
    }

    public void AddChatToPlayedChats(TextEntry chat)
    {
        if (!chatsAlreadyPlayed.Contains(chat))
        {
            chatsAlreadyPlayed.Add(chat);
        }
    }

    public bool PlayerReadAnyLog()
    {
        foreach (LogEntry log in persistentDocsData.logsFound)
        {
            if (log.read)
            {
                return true;
            }
        }
        return false;
    }

    public bool PlayerFoundAnyDoc()
    {
        return persistentDocsData.documentsFound.Count > 0;
    }
}
