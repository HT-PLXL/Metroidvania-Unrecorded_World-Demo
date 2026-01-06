using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private Image skillIcon;
    [SerializeField] private string lockedColor = "#B0B0B0";
    private Color lastColor;
    public bool isUnlocked;
    public bool islocked;//锁定支线

    private void Awake()
    {
        UpdateIconColor(GetColor(lockedColor));//设置初始颜色为暗色调
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
        if (isUnlocked == false)
            UpdateIconColor(Color.white * .9f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isUnlocked == false)
            UpdateIconColor(lastColor);
    }
    private Color GetColor(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);//将16进制字符串转化为颜色
        return color;
    }
}
