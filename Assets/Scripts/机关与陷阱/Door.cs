using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public enum MoveDirection { Up, Down }

    [Header("方向设置")]
    public MoveDirection direction = MoveDirection.Down;   // 向下收进地面，或向上收进天花板

    [Header("移动参数")]
    public float hiddenOffset = 2.1f;   // 完全隐藏所需的偏移量
    public float moveSpeed = 3f;

    private Vector3 closedPosition;   // 关闭（阻挡）位置
    private Vector3 openPosition;     // 打开（隐藏）位置
    private Coroutine moveCoroutine;
    private BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        closedPosition = transform.position;

        float dir = (direction == MoveDirection.Down) ? -1f : 1f;
        openPosition = closedPosition + Vector3.up * dir * hiddenOffset;
    }

    /// <summary>
    /// 外部调用（拉杆）切换门位置
    /// </summary>
    public void Toggle()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        // 如果当前在关闭位置 → 移到打开位置；反之亦然
        Vector3 target = (Vector3.Distance(transform.position, closedPosition) < 0.1f) ? openPosition : closedPosition;
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

    // 编辑器可视化隐藏位置
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        Vector3 start = transform.position;
        float dir = (direction == MoveDirection.Down) ? -1f : 1f;
        Vector3 hidePos = start + Vector3.up * dir * hiddenOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(hidePos, transform.localScale);
        Gizmos.DrawLine(start, hidePos);
    }
}