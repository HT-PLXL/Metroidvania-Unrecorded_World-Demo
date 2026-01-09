using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;

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
        UpdateIconColor(GetColor(lockedColor));//设置初始颜色为暗色调
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
    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);//恢复原色
        skillTree.RemoveSkillPoint(skillData.cost);
        LockConflictNodes();
    }
    private bool CanBeUnlocked()//判断节点能否被解锁
    {
        if (islocked || isUnlocked)
            return false;
        if(skillTree.haveEnoughSkillPoint(skillData.cost)==false)
            return false;
        foreach (var node in needNodes)
        {
            if (node.isUnlocked == false)//判断前置节点是否解锁
                return false;
        }
        foreach (var node in conflictNodes)
        {
            if (node.isUnlocked)//判断节点是否有冲突
                return false;
        }
        return true;
    }
    private void LockConflictNodes()//锁定冲突节点
    {
        foreach (var node in conflictNodes)
            node.islocked = true;
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
        else
            Debug.Log("无法解锁技能");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.showTooltip(true, rect, this);

        if (isUnlocked == false)
            UpdateIconColor(Color.white * .9f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.showTooltip(false, rect);

        if (isUnlocked == false)
            UpdateIconColor(lastColor);
    }
    private Color GetColor(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);//将16进制字符串转化为颜色
        return color;
    }
}
