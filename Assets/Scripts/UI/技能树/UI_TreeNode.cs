using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    private UI_TreeConnectHandle connectHandle;

    [Header("解锁功能详情")]
    public UI_TreeNode[] needNodes;//需要的前置节点
    public UI_TreeNode[] conflictNodes;//冲突的节点
    public bool isUnlocked;//已被解锁
    public bool islocked;//已被锁定

    [Header("技能详情")]
    public Skill_DataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private int skillCost;
    [SerializeField] private string lockedColor = "#B0B0B0";
    private Color lastColor;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();
        connectHandle = GetComponent<UI_TreeConnectHandle>();

        UpdateIconColor(GetColor(lockedColor));//设置初始颜色为暗色调
    }
    private void Start()
    {
        if (skillData.unlockedByDefault)
            Unlock();
    }
    private void OnValidate()//用于编辑器模式下的数据验证和初始化的特殊生命周期方法
    {
        if (skillData == null)
            return;
        skillName = skillData.skillName;
        skillIcon.sprite = skillData.icon;
        skillCost = skillData.cost;
        gameObject.name = "UI树节点 - " + skillName;
    }
    private void OnEnable()
    {
        //用于修复开启技能树后出现的异常问题
        if (isUnlocked)
            UpdateIconColor(Color.white);
        else
            UpdateIconColor(GetColor(lockedColor));
    }
    public void RefundSkillPoint()
    {
        if (skillData.unlockedByDefault)
            return;
        islocked = false;
        if (isUnlocked)
        {
            isUnlocked = false;
            skillTree.AddSkillPoint(skillData.cost);
        }
        UpdateIconColor(GetColor(lockedColor));//恢复图标颜色
        connectHandle?.UnlockConnectionLineImage(false);//恢复连接线颜色
    }
    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);//恢复原色
        skillTree.RemoveSkillPoint(skillData.cost);
        LockConflictNodes();
        connectHandle.UnlockConnectionLineImage(true);//改变连接线颜色

        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData.upgradeData);
    }
    public void UnlockByUsingSaveData()//用于读档
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);//恢复原色
        LockConflictNodes();

        connectHandle.UnlockConnectionLineImage(true);//改变连接线颜色

    }
    private bool CanBeUnlocked()//判断节点能否被解锁
    {
        if (islocked || isUnlocked)
            return false;
        if (skillTree.HaveEnoughSkillPoint(skillData.cost) == false)
            return false;
        foreach (var node in needNodes)
        {
            if (node.isUnlocked == false)//判断前置节点是否解锁
                return false;
        }
        foreach (var node in conflictNodes)
        {
            if (node == null) continue;
            if (node.isUnlocked)//判断节点是否有冲突
                return false;
        }
        return true;
    }
    private void LockConflictNodes()//锁定冲突节点
    {
        foreach (var node in conflictNodes)
            node.LockChildNodes();
    }
    public void LockChildNodes()
    {
        islocked = true;
        foreach (var node in connectHandle.GetChildNodes())
            node.LockChildNodes();
    }
    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;
        lastColor = skillIcon.color;
        skillIcon.color = color;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
        {
            Unlock();
            Debug.Log("解锁技能");
        }
        else if (islocked)
            ui.skillTooltip.lockedSkillEffect();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.showTooltip(true, rect, this);

        if (isUnlocked == false && islocked == false)
        {
            Color color = Color.white * 0.9f;
            color.a = 1;//颜色rgba的a：透明度
            UpdateIconColor(color);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.ShowTooltip(false, rect);

        if (isUnlocked == false && islocked == false)
            UpdateIconColor(lastColor);

        ui.skillTooltip.StopCo(ui.skillTooltip.textEffectCo);//停止锁定提示的变色协程

    }
    private Color GetColor(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);//将16进制字符串转化为颜色
        return color;
    }
}
