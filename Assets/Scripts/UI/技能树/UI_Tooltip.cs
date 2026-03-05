using UnityEngine;

//该类旨在于实现信息介绍面板的显/隐以及调整显示的位置，隐藏通过移动到极远坐标实现
public class UI_Tooltip : MonoBehaviour
{
    private RectTransform rect;//Rectangle矩形
    [SerializeField] private Vector2 offset = new Vector2(200, 20);

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public virtual void ShowTooltip(bool show, RectTransform targetRect)
    {
        if (show == false)
        {
            rect.position = new Vector3(8888, 8888);
            return;
        }
        UpdatePosition(targetRect);
    }

    private void UpdatePosition(RectTransform targetRect)
    {

        // 获取目标位置（屏幕坐标）
        Vector3 targetPos = targetRect.position;
        // 获取 Tooltip 尺寸
        Vector2 tooltipSize = rect.rect.size;
        // 计算新位置
        Vector3 newPos = targetPos;
        // 水平方向：左右切换
        if (targetPos.x > Screen.width / 2f)
        {
            newPos.x = targetPos.x - tooltipSize.x / 2f - offset.x;
        }
        else
        {
            newPos.x = targetPos.x + tooltipSize.x / 2f + offset.x;
        }
        // 垂直方向：保持与目标对齐，边界检查
        float halfHeight = tooltipSize.y / 2f;
        newPos.y = targetPos.y + offset.y;
        newPos.y = Mathf.Clamp(newPos.y, halfHeight + offset.y,Screen.height - halfHeight - offset.y);
        // 应用位置
        rect.position = newPos;
    }
    protected string GetColorText(string color, string text)
    {
        return $"<color={color}>{text}</color>";
    }
}
