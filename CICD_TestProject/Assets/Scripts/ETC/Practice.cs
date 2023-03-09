using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

/// <summary>
/// 인터넷 통신 실습용 클래스, 아직 실제 적용x
/// </summary>
public class Practice : MonoBehaviour
{
    //------------------------------------------------------
    int[] highScore;
    string[] highName;

    //------------------------------------------------------
    string url = "http://go2665.dothome.co.kr/HTTP_Data/TestData.txt";

    private void Start()
    {
        StartCoroutine(GetWebData());


    }

    IEnumerator GetWebData()
    {
        SaveData saveData = new SaveData();

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log($" Success :{www.downloadHandler.text}");
            TakeData takeData = JsonUtility.FromJson<TakeData>(www.downloadHandler.text);

            TakeToSave(takeData, saveData);
            SaveGameData(saveData);
            LoadGameData(saveData);
        }

    }

    private void TakeToSave(TakeData takeData, SaveData saveData)
    {
        highScore = takeData.highScore;
        highName = takeData.rankerName;

    }


    private void SaveGameData(SaveData saveData)
    {
        saveData = new SaveData();
        saveData.highScore = highScore;
        saveData.highName = highName;

        string json = JsonUtility.ToJson(saveData);
        string path = $"{Application.dataPath}/Save/";

        if (!Directory.Exists(path))   //System.IO에 존재
        {
            Directory.CreateDirectory(path);
        }

        string fullPath = $"{path}Save.json";
        File.WriteAllText(fullPath, json);
    }


    private void SaveGameData()
    {
        SaveData saveData = new SaveData();
        saveData.highScore = highScore;
        saveData.highName = highName;

        string json = JsonUtility.ToJson(saveData);
        string path = $"{Application.dataPath}/Save/";

        if(!Directory.Exists(path))   //System.IO에 존재
        {
            Directory.CreateDirectory(path);
        }

        string fullPath = $"{path}Save.json";
        File.WriteAllText(fullPath, json);
    }

    private void LoadGameData(SaveData saveData)
    {
        string path = $"{Application.dataPath}/Save/";
        string fullPath = $"{path}Save.json";

        if (Directory.Exists(path) && Directory.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            saveData = new SaveData();
            saveData = JsonUtility.FromJson<SaveData>(json);
            highName = saveData.highName;
            highScore = saveData.highScore;

        }

    }

    private void LoadGameData()
    {
        string path = $"{Application.dataPath}/Save/";
        string fullPath = $"{path}Save.json";

        if(Directory.Exists(path) && Directory.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            SaveData saveData = new SaveData();
            saveData = JsonUtility.FromJson<SaveData>(json);
            highName = saveData.highName;
            highScore = saveData.highScore;
  
        }
        
    }


}
