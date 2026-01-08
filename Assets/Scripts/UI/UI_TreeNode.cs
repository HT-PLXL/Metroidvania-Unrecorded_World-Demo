using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;

    [SerializeField] private Skill_DataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private string lockedColor = "#B0B0B0";
    private Color lastColor;
    public bool isUnlocked;
    public bool islocked;//锁定支线

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        UpdateIconColor(GetColor(lockedColor));//设置初始颜色为暗色调
    }
    private void OnValidate()//用于编辑器模式下的数据验证和初始化的特殊生命周期方法
    {
        if (skillData == null)
            return;
        skillName = skillData.skillName;
        skillIcon.sprite = skillData.icon;
        gameObject.name = "UI树节点 - " + skillName;
    }
    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);//恢复原色
    }
    private bool CanBelocked()
    {
        if (islocked || isUnlocked)
            return false;
        return true;
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
        if (CanBelocked())
        {
            Unlock();
            Debug.Log("解锁技能");
        }
        else
            Debug.Log("无法解锁技能");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.showTooltip(true, rect,skillData);

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
