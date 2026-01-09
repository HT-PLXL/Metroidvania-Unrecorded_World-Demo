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

    public virtual void showTooltip(bool show, RectTransform targetRect)
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
        float screenCenterX = Screen.width / 2f;
        float screenTop = Screen.height;
        float screenBottom = 0;

        Vector2 targetPosition = targetRect.position;
        targetPosition.x = targetPosition.x > screenCenterX ? targetPosition.x - offset.x : targetPosition.x + offset.x;

        float halfHeight = rect.sizeDelta.y / 2f;//当锚点居中时：sizeDelta = 实际尺寸
        float topY = targetPosition.y + halfHeight;
        float bottomY = targetPosition.y - halfHeight;

        if (topY > screenTop)
            targetPosition.y = screenTop - halfHeight - offset.y;//保证面板最少距离屏幕上方 offset.y 远
        else if (bottomY < screenBottom)
            targetPosition.y = screenBottom + halfHeight + offset.y;

        rect.position = targetPosition;

    }
}
