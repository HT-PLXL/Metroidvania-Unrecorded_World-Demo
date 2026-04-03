using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;//静态实例
    private FileDataHandler dataHandler;//数据处理器实例
    private GameData gameData;//需要保存/加载的游戏数据
    private List<ISaveable> allSaveables;//所有需要保存的类
    [SerializeField] private string fileName = "类银河恶魔城游戏项目存档文件";
    [SerializeField] private bool IsEncryptData = true;

    private void Awake()
    {
        instance = this;
    }
    private IEnumerator Start()//协程start，给其他start一点启动时间
    {
        //Application.persistentDataPath是Unity提供的特殊路径，用于存储游戏需要永久保存的数据
        Debug.Log(Application.persistentDataPath);//显示路径
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName,IsEncryptData);//创建新数据处理器（路径，名，是否加密）
        allSaveables = FindISaveables();//查找得到包含“需要存档”接口的对象列表
        yield return null;
        LoadGame();
    }
    public void SaveGame()//保存数据至存档
    {
        foreach (var saveable in allSaveables)
            saveable.SavaData(ref gameData);//执行接口方法修改唯一总数据
        dataHandler.SaveDataToJson(gameData);//将总数据转化为json文件存档
    }
    private void LoadGame()//读取存档数据并覆盖至原数据
    {
        gameData = dataHandler.LoadDataFromJson();
        if (gameData == null)
        {
            Debug.Log("未找到存档，正在创建新存档数据");
            gameData = new GameData();
        }
        //用存档数据覆盖各处数据
        foreach (var saveable in allSaveables)
            saveable.loadData(gameData);
    }
    public GameData GetGameData() => gameData;

    [ContextMenu("删除保存的存档")]
    public void DeleteSaveData()//删除存档
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, IsEncryptData);//创建携带最新文件信息的处理器
        dataHandler.Delete();
        Debug.Log("存档删除");
    }
    private void OnApplicationQuit()//在关闭游戏时
    {
        SaveGame();
    }
    private List<ISaveable> FindISaveables()//查找得到包含“需要存档”接口的对象列表
    {
        //查找包含MonoBehaviour基类的对象（包含未激活，不排序）.OfType保留可以转换为<ISaveable>的元素,再制成列表
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISaveable>().ToList();
    }
}
