using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    private FileDataHandler dataHandler;
    private GameData gameData;
    private HashSet<string> deadEnemyIds = new HashSet<string>();
    private bool gameDataLoaded = false;

    [SerializeField] private string fileName = "类银河恶魔城游戏项目存档文件";
    [SerializeField][cn("是否加密数据")] private bool IsEncryptData = true;
    [Header("新存档初始设置")]
    [SerializeField][cn("新存档初始金币")] private int startingGold = 0;
    [SerializeField][cn("新存档初始技能点")] private int startingSkillPoints = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private IEnumerator Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, IsEncryptData);
        yield return null;
        LoadGame();
        gameDataLoaded = true;
    }

    public void SaveGame()
    {
        // 防御：确保 dataHandler 已初始化（防止 Start 协程未完成时调用）
        if (dataHandler == null)
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, IsEncryptData);

        if (gameData == null) gameData = new GameData();

        // 保存敌人死亡状态
        gameData.enemyStates ??= new SerializableDictionary<string, bool>();
        gameData.enemyStates.Clear();
        foreach (var id in deadEnemyIds) gameData.enemyStates[id] = true;

        var saveables = FindISaveables();
        foreach (var saveable in saveables)
            saveable.SavaData(ref gameData);

        dataHandler.SaveDataToJson(gameData);
    }

    private void LoadGame()
    {
        gameData = dataHandler.LoadDataFromJson();
        if (gameData == null)
        {
            gameData = new GameData();
            gameData.gold = startingGold;
            gameData.skillPoints = startingSkillPoints;
        }
        else
        {
            deadEnemyIds.Clear();
            if (gameData.enemyStates != null)
                foreach (var kv in gameData.enemyStates)
                    if (kv.Value) deadEnemyIds.Add(kv.Key);
        }

        var saveables = FindISaveables();
        foreach (var saveable in saveables) saveable.loadData(gameData);
        foreach (var completion in saveables.OfType<ILoadDataCompletion>()) completion.OnAllDataLoaded();

        ApplyEnemyDeathStates();
    }

    private void ApplyEnemyDeathStates()
    {
        if (deadEnemyIds.Count == 0) return;
        var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var enemy in enemies)
            if (!string.IsNullOrEmpty(enemy.GetEnemyId()) && deadEnemyIds.Contains(enemy.GetEnemyId()) && !enemy.isRebirthOnLoad)
                enemy.gameObject.SetActive(false);
    }

    public void MarkEnemyDead(string enemyId) => deadEnemyIds.Add(enemyId);
    public GameData GetGameData() => gameData;

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!gameDataLoaded) return;
        StartCoroutine(ReloadDataNextFrame());
    }

    private IEnumerator ReloadDataNextFrame()
    {
        yield return null;
        var saveables = FindISaveables();
        foreach (var saveable in saveables) saveable.loadData(gameData);
        foreach (var completion in saveables.OfType<ILoadDataCompletion>()) completion.OnAllDataLoaded();
        ApplyEnemyDeathStates();
    }

    private List<ISaveable> FindISaveables() => FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISaveable>().ToList();

    [ContextMenu("删除保存的存档")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, IsEncryptData);
        dataHandler.Delete();
    }

    private void OnApplicationQuit() => SaveGame();
}