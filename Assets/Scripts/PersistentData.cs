using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Persistent Data", menuName = "Persitent Data")]
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

    public void ClearData()
    {
        beaconsInLevels.Clear();
    }
}
