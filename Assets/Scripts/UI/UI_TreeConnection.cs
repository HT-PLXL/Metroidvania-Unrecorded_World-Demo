using UnityEngine;

//该类旨在给出树节点的连接方法，用于被调用
public class UI_TreeConnection : MonoBehaviour
{
    [SerializeField] private RectTransform connectPoint;
    [SerializeField] private RectTransform connectionLength;
    [SerializeField] private RectTransform childConnectPoint;

    public void DirectConnection(NodeDirectionType direction,float length)//调节连接方向和长度
    {
        bool active = direction != NodeDirectionType.None;
        float finalLength = active ? length : 0;
        float angle = GetDirectionAngle(direction);

        connectPoint.localRotation = Quaternion.Euler(0,0,angle);
        connectionLength.sizeDelta = new Vector2(finalLength,connectionLength.sizeDelta.y);
    }
    public Vector2 GetConnectionPoint(RectTransform rect)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect.parent as RectTransform, childConnectPoint.position, null, out var localPosition);
        //RectTransformUtility是静态工具类，专门用于处理 RectTransform 的复杂计算和操作，特别是在屏幕空间、世界空间和本地空间之间的转换。
        //ScreenPointToLocalPointInRectangle（目标对象，屏幕空间坐标（世界坐标凑合用），相机（UI元素使用空），输出本地坐标）
        //rectTransform是Transform的子类，rect.parent返回的是Transform，使用as转化为RectTransform类型
        return localPosition;
    }
    private float GetDirectionAngle(NodeDirectionType type)
    {
        switch (type)
        {
            case NodeDirectionType.Up:return 90f;
            case NodeDirectionType.Down: return -90f;
            case NodeDirectionType.Left: return 180f;
            case NodeDirectionType.Right: return 0f;
            case NodeDirectionType.UpLeft: return 135f;
            case NodeDirectionType.UpRight: return 45f;
            case NodeDirectionType.DownLeft: return -135f;
            case NodeDirectionType.DownRight: return -45f;
            default: return 0f;
        }
    } 
}
public enum NodeDirectionType
{
    None, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight
}