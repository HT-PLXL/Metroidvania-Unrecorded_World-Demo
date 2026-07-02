using UnityEngine;

public class UI : MonoBehaviour
{
    private PlayerInputSet input;
    public static UI instance;
    public bool alternativeInput { get; private set; }//替代输入
    [Header("各系统UI——不包括游戏UI")]
    [SerializeField] private GameObject[] allUI;//不包括gameUI
    //在这里声明需要的各种UI类，并初始化实例
    //方便其他类通过UI的实例ui访问各种UI类
    public UI_ItemTooltip itemTooltip { get; private set; }
    public UI_SkillTooltip skillTooltip { get; private set; }
    public UI_StatTooltip statsTooltip { get; private set; }
    public UI_SkillTree skillTreeUI { get; private set; }
    public UI_Bag bagUI { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_Craft craftUI { get; private set; }
    public UI_Shop shopUI { get; private set; }
    public UI_Game gameUI { get; private set; }
    public UI_Set gameSetUI { get; private set; }
    public UI_DeathScreen deathScreenUI { get; private set; }
    public UI_FadeScreen fadeScreenUI { get; private set; }
    public UI_AttributeUpgrade AttributeUpgradeUI { get; private set; }
    public UI_Info infoUI {  get; private set; }

    private GameObject currentUI;
    private void Awake()
    {
        instance = this;
        itemTooltip = GetComponentInChildren<UI_ItemTooltip>();
        skillTooltip = GetComponentInChildren<UI_SkillTooltip>();
        statsTooltip = GetComponentInChildren<UI_StatTooltip>();
        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);//true会查找所有对象，包括被禁用的
        bagUI = GetComponentInChildren<UI_Bag>(true);
        storageUI = GetComponentInChildren<UI_Storage>(true);
        craftUI = GetComponentInChildren<UI_Craft>(true);
        shopUI = GetComponentInChildren<UI_Shop>(true);
        gameUI = GetComponentInChildren<UI_Game>(true);
        gameSetUI = GetComponentInChildren<UI_Set>(true);
        deathScreenUI = GetComponentInChildren<UI_DeathScreen>(true);
        fadeScreenUI = GetComponentInChildren<UI_FadeScreen>(true);
        AttributeUpgradeUI = GetComponentInChildren<UI_AttributeUpgrade>(true);
        infoUI = GetComponentInChildren<UI_Info>(true);

    }
    private void Start()
    {
        skillTreeUI.UnlockedDefaultSkills();
    }
    public void SetupControlUI(PlayerInputSet input, Player player)
    {
        this.input = input;
        this.input.UI.交互.performed += ctx => player.TryInteract();
        this.input.UI.开关技能树UI.performed += ctx => SwitchTo(skillTreeUI.gameObject);
        this.input.UI.开关角色与背包UI.performed += ctx => SwitchTo(bagUI.gameObject);
        this.input.UI.代替输入.performed += ctx => alternativeInput = true;
        this.input.UI.代替输入.canceled += ctx => alternativeInput = false;
        this.input.UI.开关设置UI.performed += ctx =>
        {
            //匿名函数
            foreach (var ui in allUI)
            {
                if (ui.activeSelf)//存在任意非gameUI激活则执行
                {
                    Time.timeScale = 1;
                    OpenGameUI();
                    return;
                }
            }
            Time.timeScale = 0;//时间暂停
            OpenOptionsUI();//打开设置菜单
        };
    }
    public void OpenOptionsUI()
    {
        StopPlayerControl(true);
        SwitchTo(gameSetUI.gameObject);
    }
    public void OpenGameUI()
    {
        StopPlayerControl(false);
        SwitchTo(gameUI.gameObject);
    }
    public void OpenDeathScreenUI()
    {
        if (deathScreenUI.gameObject.activeSelf == false)
            SwitchTo(deathScreenUI.gameObject);
        input.Disable();
    }
    private void StopPlayerControl(bool needStop)//开关玩家控制
    {
        if (needStop)
            input.Player.Disable();
        else
            input.Player.Enable();
    }
    public void ToggleStorageUI(bool strageUIEnable)//开关铁匠铺UI
    {
        storageUI.gameObject.SetActive(strageUIEnable);
        StopPlayerControl(strageUIEnable);
        if (strageUIEnable == false)
        {
            craftUI.gameObject.SetActive(false);
            CloseAllTooltips();
        }
    }
    public void ToggleShopUI()
    {
        bool shouldClose = shopUI.gameObject.activeSelf || craftUI.gameObject.activeSelf || storageUI.gameObject.activeSelf;
        StopPlayerControl(!shouldClose);
        shopUI.gameObject.SetActive(!shouldClose);
        if (shouldClose)
            CloseAllTooltips();
        craftUI.gameObject.SetActive(false);
        storageUI.gameObject.SetActive(false);
    }

    public void CloseAllTooltips()
    {
        itemTooltip.ShowTooltip(false, null);
        skillTooltip.ShowTooltip(false, null);
        statsTooltip.ShowTooltip(false, null);
    }
    public void SwitchTo(GameObject objectNeedSwitch, bool exit = false)
    {
        bool shouldClose = objectNeedSwitch.gameObject.activeSelf;
        currentUI = null;
        foreach (var ui in allUI)
        {
            if (ui.activeSelf)//存在任意非gameUI激活则执行
            {
                currentUI = ui;
            }
        }

        CloseAllTooltips();
        foreach (var ui in allUI)
            ui.gameObject.SetActive(false);
        StopPlayerControl(false);

        if (shouldClose || exit || currentUI == AttributeUpgradeUI.gameObject || currentUI == craftUI.gameObject)//如果开关的是当前激活的或离开检测范围true或二级UI则不执行打开(等于关闭UI)
            return;
        objectNeedSwitch.SetActive(true);
        StopPlayerControl(true);
    }
}
