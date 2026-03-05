using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;//静态实例
    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> allSaveables;
    [SerializeField] private string fileName = "类银河恶魔城游戏项目";
    [SerializeField] private bool encryptData = true;

    private void Awake()
    {
        instance = this;
    }
    private IEnumerator Start()//协程start，给其他start一点启动时间
    {
        Debug.Log(Application.persistentDataPath);//显示路径
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName,encryptData);
        allSaveables = FindISaveables();
        yield return new WaitForSeconds(0.01f);
        LoadGame();
    }
    private void LoadGame()
    {
        gameData = dataHandler.LoadData();
        if (gameData == null)
        {
            Debug.Log("未找到存档，正在创建新存档");
            gameData = new GameData();
            return;
        }
        foreach (var saveable in allSaveables)
            saveable.loadData(gameData);
    }
    public void SavaGame()
    {
        foreach (var saveable in allSaveables)
            saveable.SavaData(ref gameData);
        dataHandler.SaveData(gameData);
    }
    public GameData GetGameData() => gameData;
    [ContextMenu("删除保存的存档")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }
    private void OnApplicationQuit()
    {
        SavaGame();
    }
    private List<ISaveable> FindISaveables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISaveable>().ToList();
    }
}
