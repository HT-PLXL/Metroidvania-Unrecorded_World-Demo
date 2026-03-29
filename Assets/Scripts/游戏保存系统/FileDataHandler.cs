using System;
using System.IO;
using System.Text;
using UnityEngine;
//**********此类是文件数据处理器，管理存储+加载+删除的底层方法**********

#region 此类中使用的各种类与知识介绍
//FileStream 负责打开文件、字节传输、文件锁定——————————————水管
//StreamReader 负责：字节→字符转换、编码处理、文本解析——————水龙头 + 过滤器
/*
public FileStream(
    string path,           // 文件路径
    FileMode mode,         // 打开模式（创建、打开等）
    FileAccess access,     // 访问权限（读、写、读写）
    FileShare share        // 共享模式（其他进程如何访问）
                 )
*/

//代码块内部的using：用于自动管理资源释放，确保实现了 IDisposable 接口的对象在使用完毕后自动调用 Dispose() 方法。
//Stream这个基类包含IDisposable接口

// using 语句：确保 FileStream 在使用完毕后自动释放资源（关闭文件句柄）
// FileMode.Create 表示创建新文件，如果文件已存在则覆盖
#endregion

public class FileDataHandler
{
    private string fullPath;//完整路径
    private bool isEncryptData;//加密数据
    private string codeWord = "hjh1214179741";
    public FileDataHandler(string dataPath, string dataFileName, bool encryptData)//（路径，名，是否加密）
    {
        //拼接地址+文件名
        fullPath = Path.Combine(dataPath, dataFileName);//生成路径，dataFileName即是目录最后的文件名
        this.isEncryptData = encryptData;
    }
    public void SaveDataToJson(GameData gameData)//存储数据
    {
        try
        {
            //// 获取文件所在的目录路径（如 "C:\Game\Data\save.dat" 会返回 "C:\Game\Data"）
            //Directory.CreateDirectory(Path.GetDirectoryName(fullPath)); //如果目录不存在则创建目录
            //// 将 GameData 对象转换为 JSON 格式的字符串
            //// 参数 true 表示生成带缩进的格式化 JSON（便于阅读），false 则生成紧凑的 JSON
            //string dataNeedSave = JsonUtility.ToJson(gameData, true);//转换游戏数据为json文件

            //if (isEncryptData)
            //    dataNeedSave = EncryptOrDecrypt(dataNeedSave);//加密
            ////使用字节写入，避免文本编码破坏原始数据
            ////将加密后的字符串（可能含不可打印字符）转为 UTF-8 字节数组
            ////直接写入二进制文件，保证字节原样保存
            //byte[] bytes = Encoding.UTF8.GetBytes(dataNeedSave);
            //File.WriteAllBytes(fullPath, bytes);
            #region 废案：会出现编码问题导致报错
            /*
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))//创建或覆盖文件
            {
                using (StreamWriter write = new StreamWriter(stream))//将json信息写入文件
                {
                    write.Write(dataNeedSave);//JSON字符串写入文件
                }
            }
            */
            #endregion

            // 获取文件所在的目录路径（如 "C:\Game\Data\save.dat" 会返回 "C:\Game\Data"）
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));//如果目录不存在则创建目录
            // 将 GameData 对象转换为 JSON 格式的字符串
            // 参数 true 表示生成带缩进的格式化 JSON（便于阅读），false 则生成紧凑的 JSON
            string json = JsonUtility.ToJson(gameData, true);

            if (isEncryptData)
            {
                // 1. 将 JSON 字符串转为UTF-8字节数组
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                // 2. 对字节数组进行异或加密
                byte[] encryptedBytes = EncryptDecryptBytes(jsonBytes);
                // 3. 将加密后的字节数组转为 Base64 字符串（可打印字符）
                string base64 = Convert.ToBase64String(encryptedBytes);// Base64 是一种编码方式，将任意二进制数据转换为纯文本
                // 4. 将 Base64 字符串写入文件（可以使用文本方式写入，因为它是纯文本）
                File.WriteAllText(fullPath, base64, Encoding.UTF8);// 指定 UTF-8 编码确保跨平台兼容
            }
            else
                File.WriteAllText(fullPath, json, Encoding.UTF8);
        }
        catch (Exception e)// 捕获 try 块中可能发生的任何异常
        {
            Debug.LogError("保存数据至文件时出错：" + fullPath + "\n" + e);
        }
    }

    public GameData LoadDataFromJson()//加载数据
    {
        GameData loadData = null;
        //如果 存在fullPath
        if (File.Exists(fullPath))//查找文件
        {
            try
            {
                #region 废案：会出现编码问题导致报错
                /*
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))//文件模式：打开
                {
                    using (StreamReader reader = new StreamReader(stream))
                        dataToLoad = reader.ReadToEnd();//ReadToEnd()读取整个文件的全部内容到字符串
                }
                */
                #endregion
                ////直接读取文件的原始字节
                ////再通过 UTF-8 解码还原字符串，保证加密数据完整
                //byte[] bytes = File.ReadAllBytes(fullPath);
                //string dataToLoad = Encoding.UTF8.GetString(bytes);

                //if (isEncryptData)//如果是加密数据
                //    dataToLoad = EncryptOrDecrypt(dataToLoad);//解密
                //// 将 JSON 字符串反序列化为 GameData 对象
                //// FromJson<GameData> 是泛型方法，指定要转换的目标类型
                //loadData = JsonUtility.FromJson<GameData>(dataToLoad);//转化回游戏数据


                // 因为保存的是 Base64 字符串（纯文本），所以可以用 ReadAllText
                // 读取到的 content 是 Base64 字符串或 JSON 字符串
                string content = File.ReadAllText(fullPath, Encoding.UTF8);
                string json;// 用于存储解密后的 JSON 字符串

                if (isEncryptData)
                {
                    // 1. 将 Base64 字符串 解码为加密字节数组
                    byte[] encryptedBytes = Convert.FromBase64String(content);
                    // 2. 解密得到原始 JSON 的字节数组
                    byte[] jsonBytes = EncryptDecryptBytes(encryptedBytes);
                    // 3. 将字节数组转为 JSON 字符串
                    json = Encoding.UTF8.GetString(jsonBytes);
                }
                else
                    json = content;
                //将 JSON 字符串反序列化为 GameData 对象
                loadData = JsonUtility.FromJson<GameData>(json);
            }   
            catch (Exception e)// 捕获 try 块中可能发生的任何异常
            {
                Debug.LogError("尝试从文件加载数据时出错: " + fullPath + "\n" + e);
            }
        }
        return loadData;
    }
    public void Delete()//删除存档文件
    {
        if (File.Exists(fullPath))//如果存在
            File.Delete(fullPath);//删除
    }

    //private string EncryptOrDecrypt(string data)
    //{
    //    string modifedData = "";
    //    for (int i = 0; i < data.Length; i++)
    //    {
    //        modifedData += (char)(data[i]) ^ codeWord[i % codeWord.Length];
    //    }
    //    return modifedData;
    //}

    /// <summary>
    /// 异或加密/解密（操作字节数组）
    /// </summary>
    private byte[] EncryptDecryptBytes(byte[] data)
    {
        // 将密钥字符串转为字节数组（UTF-8 编码）
        // 密钥长度会影响加密强度，这里使用固定的密钥
        byte[] key = Encoding.UTF8.GetBytes(codeWord);
        // 创建结果数组，长度与输入数据相同
        byte[] result = new byte[data.Length];
        // 对每个字节进行异或运算
        // 异或运算的特点：
        //   - 对称性：A ^ B ^ B = A，所以加密和解密是同一操作
        //   - 快速：位运算，性能好
        //   - 安全性较低：主要用于简单的混淆，不适合高安全场景
        for (int i = 0; i < data.Length; i++)
        {
            // data[i]：当前要处理的字节
            // key[i % key.Length]：循环使用密钥中的字节
            // 异或运算：相同为0，不同为1
            result[i] = (byte)(data[i] ^ key[i % key.Length]);
        }
        return result;
    }
}

