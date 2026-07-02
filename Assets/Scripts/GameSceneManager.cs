using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameSceneManager : MonoBehaviour, ISaveable
{
    public static GameSceneManager instance;
    private Vector3 lastPosition;
    private string lastScenePlayed;
    private bool dataLoaded;
    private UI_FadeScreen fadeScreenUI;
    // 区域传送临时数据
    private static string targetExitId;
    private static bool useAreaTransition;
    private void Awake()
    {
        //因为该类包含一个加载场景的协程ChangeSceneCo，为了确保它在下一个场景依旧继续运行
        //因此需要确保GameSceneManager的单例instance从上一个场景到下一个场景依旧存在
        //因此使用DontDestroyOnLoad保留上一个场景的管理器，并销毁下一个场景的管理器
        if (instance != null && instance != this)//如果已有且不是自己，就销毁自己并退出，保证只有一个实例
        {
            Destroy(gameObject);
            return;
        }
        instance = this;//为空时初始化
        DontDestroyOnLoad(gameObject);//跨场景而不想销毁的内容（xxx）
    }
    private void Start()
    {
        StartCoroutine(ActivateTempSlotDelayed());//首次运行时尝试启用临时存档
    }

    private IEnumerator ActivateTempSlotDelayed()//首次运行时没有存档却不处于主菜单则激活测试存档
    {
        yield return null; // 等待一帧
        if (SaveManager.instance != null && SaveManager.instance.HasSlotSelected == false && SceneManager.GetActiveScene().name != "主菜单")
        {
            SaveManager.instance.ActivateTempSlot();
        }
    }
    /// <summary>通过区域传送切换场景，目标出口标识用于定位</summary>
    public void ChangeSceneWithAreaTransition(string sceneName, string exitId)
    {
        targetExitId = exitId;
        useAreaTransition = true;
        ChangeScene(sceneName, RespawnType.AreaTransition);
    }
    public void ChangeScene(string sceneName, RespawnType respawnType)//切换场景
    {
        // 防止上次异常中断后残留的区域传送标志干扰本次切换
        if (respawnType != RespawnType.AreaTransition)
        {
            targetExitId = null;
            useAreaTransition = false;
        }

        SaveManager.instance.SaveGame();
        Time.timeScale = 1;//设置界面的时间缩放是0，会影响返回主菜单
        StartCoroutine(ChangeSceneCo(sceneName, respawnType));
    }
    private IEnumerator ChangeSceneCo(string sceneName, RespawnType respawnType)//切换场景协程
    {
        fadeScreenUI = FindFadeScreenUI();
        if (fadeScreenUI == null)
        {
            Debug.LogError("找不到 UI_FadeScreen，无法执行场景切换");
            yield break;
        }

        fadeScreenUI.DoFadeOut();// 透明=>黑
        yield return fadeScreenUI.fadeEffectCo;//延迟到渐变效果结束

        SceneManager.LoadScene(sceneName);//加载场景
        yield return null;//延迟一帧等待加载完成

        fadeScreenUI = FindFadeScreenUI();//切换场景后，引用可能丢失，需要重新获取
        Image image = fadeScreenUI.GetComponent<Image>();
        image.color = new Color(0, 0, 0, 1);

        dataLoaded = false;//重置标志符，重要
        //while (dataLoaded == false)//循环，直到数据加载完成
        //    yield return null;

        // 超时保护：最多等待2秒，防止因 slotSelected 未就绪而永久卡死
        float timeout = 2f;
        float elapsed = 0f;
        while (dataLoaded == false && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!dataLoaded)
        {
            Debug.LogError($"[GameSceneManager] 等待 dataLoaded 超时（{timeout}秒），强制继续。场景：{sceneName}");
            dataLoaded = true;   // 强制标记，避免卡死
        }


        Player player = Player.instance;
        if (player == null && sceneName != "主菜单")
        {
            Debug.LogWarning("Player.instance 为空，跳过玩家位置设置");
            yield break;
        }

        Vector3 position = Vector3.zero;
        bool isArea = (respawnType == RespawnType.AreaTransition) && useAreaTransition;

        if (isArea)
        {
            position = GetExitPosition(targetExitId);
            if (position == Vector3.zero)
            {
                Debug.LogWarning($"未找到出口 '{targetExitId}'，使用默认重生点");
                position = GetNewPlayerPosition(RespawnType.None);
            }
        }
        else
            position = GetNewPlayerPosition(respawnType);//找新场景的可重生位置

        if (position != Vector3.zero)//如果查找到了非默认位置
        {
            //position.y -= 0.64178f;//重生点偏差
            Player.instance.TeleportPlayer(position);
            yield return StartCoroutine(Player.instance.TeleportPlayerCoroutine(position));
        }

        if (isArea && position != Vector3.zero)
        {
            StartCoroutine(AutoWalk(targetExitId));
        }
        fadeScreenUI.DoFadeIn();// 黑=>透明
        //yield return fadeScreenUI.fadeEffectCo;  // 等待淡入动画结束

        useAreaTransition = false;
        targetExitId = null;
    }
    private UI_FadeScreen FindFadeScreenUI()
    {
        if (UI.instance != null && UI.instance.fadeScreenUI != null)
            return UI.instance.fadeScreenUI;
        else
            return FindFirstObjectByType<UI_FadeScreen>();
    }
    public void ContinuePlay()
    {
        ChangeScene(lastScenePlayed, RespawnType.None);
    }
    /// <summary>
    /// 自定义重置场景方法
    /// </summary>
    public void RestartScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        ChangeScene(sceneName, RespawnType.None);
    }
    private Vector3 GetNewPlayerPosition(RespawnType type)//选择重生点坐标
    {
        if (type == RespawnType.None)//不指定重生点，即可以是入口也可以是重生信标，看哪个近
        {
            //获取解锁的重生点坐标列表
            var data = SaveManager.instance.GetGameData();
            // 确保 unlockedCheckPoints 不为 null
            if (data.unlockedCheckPoints == null)
                data.unlockedCheckPoints = new SerializableDictionary<string, bool>();
            var checkPoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None);//查找所有重生点且不排序
            var unlocklist = checkPoints.Where(checkPoint => data.unlockedCheckPoints.TryGetValue(checkPoint.GetCheckPointId(), out bool unlock) && unlock)//过滤解锁的重生点
                .Select(checkPoint => checkPoint.getPosition())//转而获取解锁的重生点的坐标
                .ToList();
            //获取入口坐标列表
            var enterlist = FindObjectsByType<Object_Signpost>(FindObjectsSortMode.None)
                .Where(signpost => signpost.GetRespawnType() == RespawnType.Enter)
                .Select(signpost => signpost.GetPositionAndSetTriggerFalse())
                .ToList();
            var positions = unlocklist.Concat(enterlist).ToList();
            if (positions.Count == 0)
                return Vector3.zero;
            return positions.OrderBy(position => Vector3.Distance(position, lastPosition))//距离从小到大排序
                .First();//获取第一个值（最小值）
        }
        return GetSignpostPosition(type);//指定重生点，如入口出口
    }
    private Vector3 GetSignpostPosition(RespawnType type)//寻找位置或者默认位置
    {
        var waypoints = FindObjectsByType<Object_Signpost>(FindObjectsSortMode.None);
        foreach (var point in waypoints)
        {
            if (point.GetRespawnType() == type)
            {
                return point.GetPositionAndSetTriggerFalse();
            }
        }
        return Vector3.zero;
    }
    /// <summary>根据出口标识查找场景中的出口位置</summary>
    private Vector3 GetExitPosition(string exitId)
    {
        foreach (var exit in FindObjectsByType<AreaTransitionExit>(FindObjectsSortMode.None))
            if (exit.exitId == exitId)
                return exit.transform.position;
        return Vector3.zero;
    }
    /// <summary>自动走出协程：禁用输入，按出口配置的方向速度移动一段时间</summary>
    private IEnumerator AutoWalk(string exitId)
    {
        Player player = Player.instance;
        if (player == null) yield break;

        AreaTransitionExit exit = null;
        foreach (var e in FindObjectsByType<AreaTransitionExit>(FindObjectsSortMode.None))
            if (e.exitId == exitId) { exit = e; break; }

        // 根据出口速度方向翻转角色
        player.HandleFlip(exit.moveDirectionAndSpeed.x);

        if (exit == null || exit.moveDuration <= 0f || exit.moveDirectionAndSpeed == Vector2.zero) yield break;

        // 设置自动行走标志，阻止玩家输入和状态机覆盖速度
        player.isAutoWalking = true;

        Rigidbody2D rb = player.rb;
        float elapsed = 0f;
        while (elapsed < exit.moveDuration)
        {
            rb.linearVelocity = exit.moveDirectionAndSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.isAutoWalking = false;
    }
    public void SavaData(ref GameData data)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "主菜单")
            return;
        data.lastPlayerPosition = Player.instance.transform.position;
        data.lastScenePlayed = currentScene;
        dataLoaded = false;
    }

    public void loadData(GameData data)
    {
        lastScenePlayed = data.lastScenePlayed;
        lastPosition = data.lastPlayerPosition;

        if (string.IsNullOrEmpty(lastScenePlayed))
            lastScenePlayed = "安全区";
        dataLoaded = true;
    }
}