using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour, ISaveable
{
    public static GameSceneManager instance;
    private Vector3 lastPosition;
    private string lastScenePlayed;
    private bool dataLoaded;
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
    public void ChangeScene(string sceneName, RespawnType respawnType)//切换场景
    {
        SaveManager.instance.SaveGame();
        Time.timeScale = 1;//设置界面的时间缩放是0，会影响返回主菜单
        StartCoroutine(ChangeSceneCo(sceneName, respawnType));
    }
    private IEnumerator ChangeSceneCo(string sceneName, RespawnType respawnType)//切换场景协程
    {
        UI_FadeScreen fadeScreenUI = FindFadeScreenUI();
        fadeScreenUI.DoFadeOut();// 透明=>黑
        yield return fadeScreenUI.fadeEffectCo;//延迟到渐变效果结束

        SceneManager.LoadScene(sceneName);//加载场景

        dataLoaded = false;//重置标志符，重要
        //yield return null;//延迟一帧给UI准备

        while (dataLoaded == false)//循环，直到数据加载完成
            yield return null;

        Player player = Player.instance;
        if (player == null)
            yield break;//终止协程
        Vector3 position = GetNewPlayerPosition(respawnType);//找新场景的可重生位置
        if (position != Vector3.zero)//如果查找到了非默认位置
            Player.instance.TeleportPlayer(position);

        fadeScreenUI = FindFadeScreenUI();//切换场景后，引用可能丢失，需要重新获取
        fadeScreenUI.DoFadeIn();// 黑=>透明
    }
    private UI_FadeScreen FindFadeScreenUI()
    {
        if (UI.instance != null)
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
            lastScenePlayed = "关卡0";
        dataLoaded = true;
    }
}
