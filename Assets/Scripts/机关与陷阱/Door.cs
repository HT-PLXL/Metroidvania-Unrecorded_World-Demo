using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public enum MoveDirection { Up, Down, Left, Right }

    [Header("方向设置（世界坐标）")]
    public MoveDirection direction = MoveDirection.Down;

    [Header("移动参数")]
    public float hiddenOffset = 2.1f;
    public float moveSpeed = 3f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Coroutine moveCoroutine;

    void Awake()
    {
        closedPosition = transform.position;
        UpdateOpenPosition();
    }

    // 根据世界方向计算打开位置
    private void UpdateOpenPosition()
    {
        Vector3 dir = GetWorldDirectionVector();
        openPosition = closedPosition + dir * hiddenOffset;
    }

    // 获取世界方向向量（不随旋转改变）
    private Vector3 GetWorldDirectionVector()
    {
        switch (direction)
        {
            case MoveDirection.Up: return Vector3.up;
            case MoveDirection.Down: return Vector3.down;
            case MoveDirection.Left: return Vector3.left;
            case MoveDirection.Right: return Vector3.right;
            default: return Vector3.zero;
        }
    }

    public void Toggle()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        UpdateOpenPosition(); // 支持运行时改变方向或位置

        Vector3 target = Vector3.Distance(transform.position, closedPosition) < 0.1f
            ? openPosition
            : closedPosition;

        moveCoroutine = StartCoroutine(MoveToTarget(target));
    }

    IEnumerator MoveToTarget(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }

    // 编辑器可视化：绘制两个带旋转的线框（当前位置和隐藏位置）
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;

        // 保存当前 Gizmos 矩阵
        Matrix4x4 originalMatrix = Gizmos.matrix;

        // 1. 绘制当前位置的线框（跟随门的旋转和缩放）
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        // 2. 绘制隐藏位置的线框（同样跟随门的旋转和缩放）
        Vector3 hidePos = transform.position + GetWorldDirectionVector() * hiddenOffset;
        Gizmos.matrix = Matrix4x4.TRS(hidePos, transform.rotation, transform.lossyScale);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        // 3. 绘制连线（从当前位置到隐藏位置）
        Gizmos.matrix = originalMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, hidePos);

        // 恢复矩阵
        Gizmos.matrix = originalMatrix;
    }
}