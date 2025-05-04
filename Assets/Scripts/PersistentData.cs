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
        public List<Document> documentsRead;
        public List<LogEntry> logsFound;
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
            if (documentsRead != null)
            {
                documentsRead.Clear();
            }
            else
            {
                documentsRead = new();
            }
            if (logsFound != null)
            {
                logsFound.Clear();
            }
            else
            {
                logsFound = new();
            }
        }

        public bool HasData()
        {
            if (learnableWords != null && wordsLearned != null && documentsRead != null && logsFound != null)
            {
                return (learnableWords.Count > 0 || wordsLearned.Count > 0 || documentsRead.Count > 0 || logsFound.Count > 0);
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
        persistentDocsData.documentsRead = documentsRead;
        persistentDocsData.knowledgeLevel = knowledgeLevel;
    }

    public void UpdatePersistentLogsData(List<LogEntry> logsFound)
    {
        persistentDocsData.logsFound = logsFound;
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
        beaconsDiscovered = false;
        flashlightOn = false;
        helmetOn = false;
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

    public void AddChatToPlayedChats(TextEntry chat)
    {
        if (!chatsAlreadyPlayed.Contains(chat))
        {
            chatsAlreadyPlayed.Add(chat);
        }
    }
}
