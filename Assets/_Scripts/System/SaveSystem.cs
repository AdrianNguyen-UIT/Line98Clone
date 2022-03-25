using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public static class SaveSystem
{
    private readonly static string saveFileName = "SaveData00.dat";

    #region Public Methods
    public static bool SaveJSonData()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        SaveData saveData = new SaveData();
        GameManager.Instance.PopulateSaveData(saveData);

        if (FileManager.WriteToFile(saveFileName, saveData.ToJSon()))
        {
            stopwatch.Stop();
            UnityEngine.Debug.Log("Save successful. Time elapsed: " + stopwatch.ElapsedMilliseconds + "ms");
            return true;
        }
        stopwatch.Stop();
        return false;
    }

    public static bool LoadJSonData()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        if (FileManager.LoadFromFile(saveFileName, out var json))
        {
            SaveData saveData = new SaveData();
            saveData.FromJson(json);
            GameManager.Instance.LoadFromSaveData(saveData);

            stopwatch.Stop();
            UnityEngine.Debug.Log("Load complete. Time elapsed: " + stopwatch.ElapsedMilliseconds + "ms");
            return true;
        }
        return false;
    }
    #endregion

}