using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Serializable]
    public class NodeData
    {
        public Vector2 Pos;
        public int ColorConfigID;
        public bool IsGhost;
    }

    public uint HighScore;
    public uint CurrentScore;
    public float PlayTime;
    public List<int> ColorIndexQueue = new List<int>();
    public List<NodeData> QueueNodes = new List<NodeData>();
    public List<NodeData> GrowUpNodes = new List<NodeData>();

    public string ToJSon()
    {
        return JsonUtility.ToJson(this, true);
    }

    public void FromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public void Clear()
    {
    }

    public bool IsNew()
    {
        return true;
    }
}

public interface ISaveable
{
    void PopulateSaveData(SaveData saveData);
    void LoadFromSaveData(SaveData saveData);
}