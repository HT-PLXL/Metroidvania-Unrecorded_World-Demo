using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField][cn("关联的平台")] private MovingPlatform platform;
    [SerializeField][cn("交互按键")] private KeyCode interactKey = KeyCode.E;
    [SerializeField][cn("即时响应")] private bool moveAgain;

    [Header("移动参数")]
    [SerializeField][cn("移动角度(0=右, 90=上)")] private float moveAngle = 0f;      // 角度（0=右, 90=上）
    [SerializeField][cn("移动距离")] private float moveDistance = 3f;
    [SerializeField][cn("移动速度")] private float moveSpeed = 2f;

    [Header("调试")]
    [SerializeField][cn("展示辅助线")] private bool showGizmos = true;
    [SerializeField][cn("初始朝左")] private bool turnLeft;

    private bool playerInRange;
    private Animator anim;
    private Vector2 direction;
    private Vector2 targetPos;
    private Vector2 nextPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        // 计算目标点（基于平台的初始起点 + 偏移）
        direction = Quaternion.Euler(0, 0, moveAngle) * Vector2.right;
        targetPos = platform.StartPos + direction * moveDistance;
        if (moveAgain)
            nextPoint = targetPos;
        
    }
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Activate();
        }
    }

    private void Activate()
    {
        if (platform == null)
        {
            Debug.LogWarning("拉杆未关联平台！");
            return;
        }

        if (moveAgain)
        {
            if (nextPoint == targetPos)
            {
                platform.MoveToTarget(targetPos, moveSpeed);
                nextPoint = platform.StartPos;
            }
            else if (nextPoint == platform.StartPos)
            {
                platform.MoveToTarget(platform.StartPos, moveSpeed);
                nextPoint = targetPos;
            }

            turnLeft = !turnLeft;
            anim.SetBool("turnLeft", turnLeft);
            return;
        }
        // 判断平台当前位置：若已在目标点，则返回起点；否则移向目标点
        if (platform.IsAtPosition(targetPos))
            platform.MoveToTarget(platform.StartPos, moveSpeed);
        else
            platform.MoveToTarget(targetPos, moveSpeed);

        turnLeft = !turnLeft;
        anim.SetBool("turnLeft", turnLeft);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || platform == null) return;

        // 获取平台起点（编辑器未运行时取当前位置）
        Vector2 startPos;
#if UNITY_EDITOR
        startPos = platform.GetEditorStartPos();
#else
        startPos = platform.StartPos;
#endif

        Vector2 dir = Quaternion.Euler(0, 0, moveAngle) * Vector2.right;
        Vector3 targetPoint = (Vector3)(startPos + dir * moveDistance);
        Vector3 origin = startPos;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, targetPoint);
        Gizmos.DrawSphere(targetPoint, 0.15f);

        // 箭头
        Vector3 arrowDir = (targetPoint - origin).normalized;
        Vector3 right = Quaternion.Euler(0, 0, 30) * -arrowDir;
        Vector3 left = Quaternion.Euler(0, 0, -30) * -arrowDir;
        Gizmos.DrawRay(targetPoint, right * 0.3f);
        Gizmos.DrawRay(targetPoint, left * 0.3f);
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        if (anim != null)
            anim.SetBool("turnLeft", turnLeft);
    }
#endif
}