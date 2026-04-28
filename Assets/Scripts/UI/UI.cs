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

    private GameObject currentUI;
    private bool skillTreeEnable;
    private bool bagEnable;
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

        skillTreeEnable = skillTreeUI.gameObject.activeSelf;//同步激活情况，防止出现初始激活，然后需要两次才能关掉
        bagEnable = bagUI.gameObject.activeSelf;
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
        skillTreeEnable = false;
        bagEnable = false;

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
    //public void ToggleSkillTreeUI()
    //{
    //    if (gameSetUI.gameObject.activeSelf)
    //        return;
    //    skillTreeEnable = !skillTreeEnable;

    //    if (skillTreeEnable && bagEnable)//先关闭其他UI
    //        ToggleBagUI();

    //    skillTreeUI.gameObject.SetActive(skillTreeEnable);
    //    skillTooltip.ShowTooltip(false, null);

    //    StopPlayerControl(skillTreeEnable);
    //}
    //public void ToggleBagUI()
    //{
    //    if (gameSetUI.gameObject.activeSelf)
    //        return;
    //    bagEnable = !bagEnable;

    //    if (skillTreeEnable && bagEnable)//先关闭其他UI
    //        ToggleSkillTreeUI();

    //    bagUI.gameObject.SetActive(bagEnable);
    //    statsTooltip.ShowTooltip(false, null);
    //    itemTooltip.ShowTooltip(false, null);

    //    StopPlayerControl(bagEnable);

    //}
    //public void CloseUI()
    //{
    //    skillTreeEnable = true;//先设为真，方法内会反转，从而关闭
    //    ToggleSkillTreeUI();

    //    bagEnable = true;//先设为真，方法内会反转，从而关闭
    //    ToggleBagUI();
    //}
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
    public void ToggleShopUI(bool shopUIEnable)
    {
        shopUI.gameObject.SetActive(shopUIEnable);
        StopPlayerControl(shopUIEnable);
        if (shopUIEnable == false)
            CloseAllTooltips();
    }

    public void CloseAllTooltips()
    {
        itemTooltip.ShowTooltip(false, null);
        skillTooltip.ShowTooltip(false, null);
        statsTooltip.ShowTooltip(false, null);
    }
    public void SwitchTo(GameObject objectNeedSwitch, bool exit = false)
    {
        Debug.Log("1111111");
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
