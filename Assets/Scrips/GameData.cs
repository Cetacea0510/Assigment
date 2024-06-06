using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GameData
{

    public int score;// diem so lon nhat da tung dat duoc
    public float time;// thoi gian ngan nhat da tung choi
}
//class nay dung xu li doc va ghi file
public class DataManager
{
    const string FILE_NAME = "data.txt";
    public static bool SaveData(GameData data)
    {
        try
        {
            var json = JsonUtility.ToJson(data);// chuyển dữ liệu sang dạng text
            var fileStream = new FileStream(FILE_NAME, FileMode.Create);//tạo file mới 
            using (var writer = new StreamWriter(fileStream))// mở file 
            {
                writer.Write(json);//ghi dữ liệu vào file  
            }
            return true;
        }
        catch (Exception e) 
        {
            Debug.Log($"Save data error: {e.Message}");
        }
        return false;
    }
    public static GameData ReadData()
    {
        try
        {
            if(File.Exists(FILE_NAME))// ktra file có tồn tại không
            {
                var fileStream = new FileStream(FILE_NAME, FileMode.Open);
                using (var reader = new StreamReader(fileStream))
                {
                    var json = reader.ReadToEnd();// đọc dữ liệu trong file
                    var data = JsonUtility.FromJson<GameData>(json);
                    return data;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Error loading file: " + e.Message);
        }
        return null;
    }
}