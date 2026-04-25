using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 currentTarget;
    private float currentSpeed;
    private bool isMoving;

    /// <summary> 平台起点（运行时的初始位置） </summary>
    public Vector2 StartPos { get; private set; }

    /// <summary> 平台当前速度（基于帧位移，用于玩家跟随） </summary>
    public Vector2 Velocity { get; private set; }

    private Rigidbody2D rb;
    private Vector2 previousPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        StartPos = rb.position;
        startPos = StartPos;
        previousPosition = rb.position;
    }

    /// <summary>
    /// 由外部（拉杆）调用，命令平台移动到指定目标点并设定速度。
    /// 新指令会打断当前移动，立即转向新目标。
    /// </summary>
    public void MoveToTarget(Vector2 target, float speed)
    {
        currentTarget = target;
        currentSpeed = speed;
        isMoving = true;
    }

    /// <summary>
    /// 判断平台是否在某个点附近（用于决定往返方向）。
    /// </summary>
    public bool IsAtPosition(Vector2 point, float tolerance = 0.05f)
    {
        return Vector2.Distance(rb.position, point) <= tolerance;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, currentTarget, currentSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            if (Vector2.Distance(rb.position, currentTarget) < 0.01f)
            {
                rb.MovePosition(currentTarget);
                isMoving = false;
            }
        }
        Velocity = (rb.position - previousPosition) / Time.fixedDeltaTime;
        previousPosition = rb.position;
    }

#if UNITY_EDITOR
    // 编辑器下供拉杆 Gizmos 使用（未运行时获取场景初始位置）
    public Vector2 GetEditorStartPos()
    {
        return Application.isPlaying ? StartPos : (Vector2)transform.position;
    }
#endif
}