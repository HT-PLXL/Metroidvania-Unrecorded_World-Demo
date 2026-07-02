using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [Header("存档设置")]
    [SerializeField][cn("存档槽位数量")] private int slotCount = 6;// 存档槽位数量
    [SerializeField][cn("存档文件名前缀")] private string slotFileNamePrefix = "Save_";// 文件名前缀
    [SerializeField][cn("是否加密数据")] private bool isEncryptData = true;

    [Header("新存档初始设置")]
    [SerializeField][cn("新存档初始金币")] private int startingGold = 0;
    [SerializeField][cn("新存档初始技能点")] private int startingSkillPoints = 0;

    [Header("直接从关卡运行时使用测试存档")]
    [SerializeField][cn("测试存档文件名")] private string tempSlotFileName = "TempSave";
    [SerializeField][cn("测试存档初始场景")] private string tempStartScene = "安全区";
    private FileDataHandler[] slotHandlers;     // 每个槽位独立的数据处理器
    private GameData gameData;                  // 当前加载的游戏数据
    private int currentSlot = -1;               // -1 表示未选择任何槽位
    private HashSet<string> deadEnemyIds = new HashSet<string>();
    private HashSet<string> openedChestIds = new HashSet<string>();// 记录已打开的不可重生宝箱
    private bool slotSelected = false;

    // 测试存档独立的处理器和标志
    private FileDataHandler tempHandler;
    public bool isUsingTempData = false;
    // 公共属性
    public int CurrentSlot => currentSlot;
    public int SlotCount => slotCount;
    public bool HasSlotSelected => slotSelected;
    public GameData GetGameData() => gameData;

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

        // 确保对象激活，避免协程无法启动
        gameObject.SetActive(true);

        //初始化所有槽位的处理器（不加载数据）
        InitializeSlotHandlers();
        //初始化临时存档
        InitializeTempHandler();

    }

    [ContextMenu("删除测试存档")]
    private void DeleteTempSaveFile()
    {
        string fullPath = System.IO.Path.Combine(Application.persistentDataPath, tempSlotFileName);
        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
            Debug.Log($"已删除测试存档文件: {fullPath}");
        }
        else
        {
            Debug.Log("测试存档文件不存在，无需删除");
        }
    }
    private void InitializeSlotHandlers()
    {
        slotHandlers = new FileDataHandler[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            string slotFileName = $"{slotFileNamePrefix}{i}";
            slotHandlers[i] = new FileDataHandler(Application.persistentDataPath, slotFileName, isEncryptData);
        }
    }
    private void InitializeTempHandler()
    {
        tempHandler = new FileDataHandler(Application.persistentDataPath, tempSlotFileName, isEncryptData);
    }
    /// <summary> 检查指定槽位是否有存档文件 </summary>
    public bool GetSlotSaveExists(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotCount) return false;
        return System.IO.File.Exists(slotHandlers[slotIndex].FullPath);
    }

    /// <summary> 获取槽位的预览数据（金币和最后场景）用于UI显示 </summary>
    public (int gold, string lastScene) GetSlotPreview(int slotIndex)
    {
        if (!GetSlotSaveExists(slotIndex)) return (0, "");
        try
        {
            GameData previewData = slotHandlers[slotIndex].LoadDataFromJson();
            if (previewData != null)
                return (previewData.gold, previewData.lastScenePlayed);
        }
        catch { }
        return (0, "");
    }

    /// <summary> 选择并加载指定槽位（如果已有存档）</summary>
    public bool SelectSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotCount) return false;
        currentSlot = slotIndex;
        gameData = slotHandlers[currentSlot].LoadDataFromJson();

        if (gameData == null)
        {
            Debug.LogWarning($"槽位 {slotIndex} 没有存档文件，请先创建新存档。");
            slotSelected = false;
            return false;
        }

        LoadEnemyStatesFromData();
        LoadChestStatesFromData();

        slotSelected = true;


        isUsingTempData = false;


        ReloadAllSaveables();
        return true;
    }

    /// <summary> 创建新存档（覆盖）并立即选中 </summary>
    public bool CreateNewSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotCount) return false;

        // 创建全新的 GameData
        GameData newGameData = new GameData();
        newGameData.gold = startingGold;
        newGameData.skillPoints = startingSkillPoints;
        newGameData.lastScenePlayed = "安全区";

        // 保存到文件
        slotHandlers[slotIndex].SaveDataToJson(newGameData);

        // 选中该槽位
        return SelectSlot(slotIndex);
    }
    // --- 测试存档相关方法 ---
    public bool ActivateTempSlot()
    {
        gameData = tempHandler.LoadDataFromJson();
        if (gameData == null)
        {
            // 创建全新的测试存档
            gameData = new GameData();
            gameData.gold = startingGold;
            gameData.skillPoints = startingSkillPoints;
            gameData.lastScenePlayed = tempStartScene;
            SaveTempData(); // 立即保存一次，确保文件存在
        }
        LoadEnemyStatesFromData();
        LoadChestStatesFromData();

        slotSelected = true;      // 视为已选择存档（以便保存逻辑正常工作）
        isUsingTempData = true;
        currentSlot = -1;         // 标记为临时槽位
        ReloadAllSaveables();
        Debug.Log("已激活测试存档");
        return true;
    }
    private void SaveTempData()
    {
        if (gameData == null) return;
        // 同步敌人状态
        gameData.enemyStates ??= new SerializableDictionary<string, bool>();
        gameData.enemyStates.Clear();
        foreach (var id in deadEnemyIds)
            gameData.enemyStates[id] = true;

        tempHandler.SaveDataToJson(gameData);
    }
    /// <summary> 保存当前游戏数据到选中的槽位 </summary>
    public void SaveGame()
    {
        if (!slotSelected)
        {
            Debug.LogWarning("没有选择存档，自动激活测试存档");
            ActivateTempSlot();  // 自动激活测试存档
        }

        if (gameData == null) gameData = new GameData();

        // 保存敌人死亡状态
        gameData.enemyStates ??= new SerializableDictionary<string, bool>();
        gameData.enemyStates.Clear();
        foreach (var id in deadEnemyIds)
            gameData.enemyStates[id] = true;

        // 保存宝箱状态
        gameData.chestOpenedStates ??= new SerializableDictionary<string, bool>();
        gameData.chestOpenedStates.Clear();
        foreach (var id in openedChestIds)
            gameData.chestOpenedStates[id] = true;

        // 收集所有需要保存的组件
        var saveables = FindISaveables();
        foreach (var saveable in saveables)
            saveable.SavaData(ref gameData);

        if (isUsingTempData)
            SaveTempData();
        else if (currentSlot >= 0)
            slotHandlers[currentSlot].SaveDataToJson(gameData);

        Debug.Log($"游戏已保存 ({(isUsingTempData ? "测试存档" : $"槽位 {currentSlot}")})");
    }
    private void LoadChestStatesFromData()
    {
        openedChestIds.Clear();
        if (gameData.chestOpenedStates != null)
        {
            foreach (var kv in gameData.chestOpenedStates)
                if (kv.Value) openedChestIds.Add(kv.Key);
        }
    }

    private void LoadEnemyStatesFromData()
    {
        deadEnemyIds.Clear();
        if (gameData.enemyStates != null)
        {
            foreach (var kv in gameData.enemyStates)
                if (kv.Value) deadEnemyIds.Add(kv.Key);
        }
    }
    private void ApplyChestOpenedStates()
    {
        if (openedChestIds.Count == 0) return;
        var chests = FindObjectsByType<Chest>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var chest in chests)
        {
            if (!chest.canRebornOnLoad && openedChestIds.Contains(chest.GetChestId()))
            {
                Destroy(chest.gameObject);
            }
        }
    }
    private void ApplyEnemyDeathStates()
    {
        if (deadEnemyIds.Count == 0) return;
        var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            if (!string.IsNullOrEmpty(enemy.GetEnemyId()) && deadEnemyIds.Contains(enemy.GetEnemyId()) && !enemy.isRebirthOnLoad)
                enemy.gameObject.SetActive(false);
        }
    }

    public void MarkEnemyDead(string enemyId) => deadEnemyIds.Add(enemyId);//标记敌人死亡
    public void MarkChestOpened(string chestId)
    {
        openedChestIds.Add(chestId);
    }
    private List<ISaveable> FindISaveables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISaveable>().ToList();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!slotSelected) return;
        StartCoroutine(ReloadDataNextFrame());
    }
    /// <summary> 强制所有可保存组件重新加载当前游戏数据 </summary>
    private IEnumerator ReloadDataNextFrame()
    {
        yield return null; // 等待一帧，确保场景所有对象的 Start 执行完毕
        if (!slotSelected || gameData == null) yield break;

        yield return null;

        var saveables = FindISaveables();
        foreach (var saveable in saveables)
        {
            try
            {
                saveable.loadData(gameData);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"加载数据时出错: {saveable.GetType().Name} - {e}");
            }
        }

        foreach (var completion in saveables.OfType<ILoadDataCompletion>())
        {
            try
            {
                completion.OnAllDataLoaded();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"完成回调出错: {completion.GetType().Name} - {e}");
            }
        }

        ApplyEnemyDeathStates();
        ApplyChestOpenedStates();
    }
    public void ReloadAllSaveables()
    {
        if (!slotSelected || gameData == null) return;

        var saveables = FindISaveables();
        foreach (var saveable in saveables)
        {
            try
            {
                saveable.loadData(gameData);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ReloadAllSaveables 加载失败: {saveable.GetType().Name} - {e}");
            }
        }

        foreach (var completion in saveables.OfType<ILoadDataCompletion>())
        {
            try
            {
                completion.OnAllDataLoaded();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"OnAllDataLoaded 失败: {completion.GetType().Name} - {e}");
            }
        }

        ApplyEnemyDeathStates();
        ApplyChestOpenedStates();
    }
    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if (!slotSelected) return;
    //    StartCoroutine(ReloadDataNextFrame());
    //}

    //private IEnumerator ReloadDataNextFrame()
    //{
    //    yield return null;
    //    if (!slotSelected) yield break;
    //    var saveables = FindISaveables();
    //    foreach (var saveable in saveables)
    //        saveable.loadData(gameData);
    //    foreach (var completion in saveables.OfType<ILoadDataCompletion>())
    //        completion.OnAllDataLoaded();
    //    ApplyEnemyDeathStates();
    //    ApplyChestOpenedStates();
    //}

    /// <summary> 强制所有可保存组件重新加载当前游戏数据 </summary>
    //public void ReloadAllSaveables()
    //{
    //    if (!slotSelected || gameData == null) return;
    //    var saveables = FindISaveables();
    //    foreach (var saveable in saveables)
    //        saveable.loadData(gameData);
    //    foreach (var completion in saveables.OfType<ILoadDataCompletion>())
    //        completion.OnAllDataLoaded();
    //    ApplyEnemyDeathStates();  // 重新应用敌人死亡状态
    //    ApplyChestOpenedStates();
    //}

    /// <summary> 删除指定槽位的存档文件 </summary>
    public void DeleteSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotCount) return;
        slotHandlers[slotIndex].Delete();
        Debug.Log($"已删除槽位 {slotIndex} 的存档");

        // 如果删除的是当前正在使用的槽位（理论上主菜单不会，但防御）
        if (slotSelected && currentSlot == slotIndex)
        {
            slotSelected = false;
            currentSlot = -1;
            gameData = null;
            deadEnemyIds.Clear();
        }
    }

    private void OnApplicationQuit()
    {
        if (slotSelected)
            SaveGame();
    }

    // 辅助方法：获取完整路径（供UI预览使用）
    public string GetSlotFilePath(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotCount) return "";
        return slotHandlers[slotIndex].FullPath;
    }
}