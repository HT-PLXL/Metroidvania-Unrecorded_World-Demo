using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string fullPath;
    private bool encryptData;//加密数据
    private string codeWord = "hjh1214179741";
    public FileDataHandler(string dataPath, string dataFileName, bool encryptData)
    {
        fullPath = Path.Combine(dataPath, dataFileName);//生成路径
        this.encryptData = encryptData;
    }
    public void SaveData(GameData gameData)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)); //如果目录不存在则创建目录
            string dataNeedSava = JsonUtility.ToJson(gameData, true);//转换游戏数据为json文件

            if(encryptData)
                dataNeedSava = EncryptDecrypt(dataNeedSava);//加密

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))//打开或创建文件
            {
                using (StreamWriter write = new StreamWriter(stream))//将json信息写入文件
                {
                    write.Write(dataNeedSava);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存数据至文件时出错" + fullPath + "\n" + e);
        }
    }

    public GameData LoadData()
    {
        GameData loadData = null;

        if (File.Exists(fullPath))//查找文件
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))//打开它
                {
                    using (StreamReader reader = new StreamReader(stream))
                        dataToLoad = reader.ReadToEnd();//读取信息
                }

                if (encryptData)
                    dataToLoad = EncryptDecrypt(dataToLoad);//解密

                loadData = JsonUtility.FromJson<GameData>(dataToLoad);//转化回游戏数据
            }
            catch (Exception e)
            {
                Debug.LogError("Error on trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadData;
    }
    public void Delete()
    {
        if (File.Exists(fullPath))//如果存在
            File.Delete(fullPath);
    }

    private string EncryptDecrypt(string data)
    {
        string modifedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifedData += (char)(data[i]) ^ codeWord[i % codeWord.Length];
        }
        return modifedData;
    }
}

