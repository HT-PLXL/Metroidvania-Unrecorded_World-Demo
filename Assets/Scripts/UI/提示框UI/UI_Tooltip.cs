using UnityEngine;

//该类旨在于实现信息介绍面板的显/隐以及调整显示的位置，隐藏通过移动到极远坐标实现
public class UI_Tooltip : MonoBehaviour
{
    private RectTransform rect;//Rectangle矩形
    [SerializeField] private Vector2 offset = new Vector2(60, 20);
    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public virtual void ShowTooltip(bool show, RectTransform targetRect)
    {
        if (rect == null)//防止停止运行时报错
            return;
        if (show == false)
        {
            rect.position = new Vector3(8888, 8888);
            return;
        }
        UpdatePosition(targetRect);
    }

    private void UpdatePosition(RectTransform targetRect)
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 targetPos = targetRect.position;
        Vector2 tooltipSize = rect.rect.size;
        Vector3 halfTooltip = tooltipSize / 2f;
        if (mouseScreenPosition.y + halfTooltip.y > Screen.height)
            targetPos.y = Screen.height - halfTooltip.y - offset.y;
        else if (mouseScreenPosition.y < halfTooltip.y)
            targetPos.y = halfTooltip.y + offset.y;
        if (targetPos.x > Screen.width / 2f)
        {
            targetPos.x = targetPos.x - tooltipSize.x / 2f - offset.x;
        }
        else
        {
            targetPos.x = targetPos.x + tooltipSize.x / 2f + offset.x;
        }
        rect.position = targetPos;

    }

    protected string GetColorText(string color, string text)
    {
        return $"<color={color}>{text}</color>";
    }
}
