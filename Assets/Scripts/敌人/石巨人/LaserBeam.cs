using System.Collections.Generic;
using UnityEngine;

public class LaserBeam2D : MonoBehaviour
{
    private Stone stone;
    private int stoneEnterStage;
    [Header("基础设置")]
    public Transform startPoint;                        // 激光发射点参考 (Reference start point)
    public Transform player;                            // 追踪目标 (Tracking target)
    public float existenceTime = 5;

    [Header("目标偏移")]
    public Vector2 targetOffset = new Vector2(0f, -0.5f); // 目标点偏移，可调整命中脚部区域

    [Header("起点偏移")]
    public bool applyStartOffset = false;               // 是否启用起点偏移
    public Vector2 startOffset = Vector2.zero;          // 相对于 startPoint 的偏移量

    [Header("地面图层")]
    public LayerMask groundLayer;                       // 用于射线检测的地面层

    [Header("动画资源")]
    public SpriteRenderer spriteRenderer;               // 精灵渲染器
    public List<Sprite> dotFrames;                      // 圆点动画帧
    public List<Sprite> beamFrames;                     // 激光束动画帧
    public float frameRate = 30f;                       // 动画帧率

    [Header("行为参数")]
    public float extensionSpeed = 10f;                  // 伸长速度 (单位/秒)
    public float tipMoveSpeed = 5f;                     // 末端线速度 (单位/秒)
    public float rotationSpeed = 2f;                    // 备用角速度 (当线速度不可用时)

    public enum LaserState { Extending, Tracking }
    public LaserState currentState = LaserState.Extending;

    [Header("旋转开关")]
    public bool enableRotation = true;                  // 是否允许方向旋转 (关闭则方向固定)

    [Header("固定长度设置")]
    public bool useFixedLength = false;                 // 是否使用固定长度 (忽略地面检测)
    public float fixedLength = 5f;                      // 固定长度值

    [Header("固定方向设置")]
    public bool useFixedDirection = false;              // 是否使用固定方向
    public Vector2 fixedDirection = Vector2.down;       // 固定方向向量 (会被归一化)
    [Tooltip("固定角度 (覆盖 fixedDirection)")]
    public float fixedAngle = 0f;                       // 固定角度 (度)，优先级高于 fixedDirection
    public bool useAngleInstead = false;                // 是否使用角度而非向量

    [Header("独立起点")]
    public bool useCustomStartPoint = false;            // 是否使用另一个 Transform 作为起点
    public Transform customStartPoint;                  // 自定义起点

    // 私有变量
    private Vector2 currentDirection;                   // 当前激光方向
    private float currentLength = 0f;                   // 当前激光长度
    private float frameTimer = 0f;
    private int currentFrameIndex = 0;
    private float originalScaleY = 1f;                  // 精灵原始 Y 轴缩放 (宽度)
    private float spriteNativeWidth = 1f;               // 精灵原生宽度 (世界单位)
    private float scaleCorrectionFactor = 1f;           // 缩放修正系数

    // 计算实际发射点位置 (考虑偏移和自定义起点)
    private Vector2 ActualStartPoint
    {
        get
        {
            Transform basePoint = (useCustomStartPoint && customStartPoint != null) ? customStartPoint : startPoint;
            if (basePoint == null)
                return Vector2.zero;
            Vector2 pos = basePoint.position;
            if (applyStartOffset)
                pos += startOffset;
            return pos;
        }
    }

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalScaleY = transform.localScale.y;
        CalculateSpriteNativeWidth();

        // 初始化方向
        UpdateFixedDirectionIfNeeded();
        if (useFixedDirection)
        {
            currentDirection = GetFixedDirection();
        }
        else
        {
            currentDirection = Vector2.down;
        }

        currentLength = 0f;
        PlayDotAnimation();

        // 预检测起点是否在地面内（可选）
        RaycastHit2D initHit = Physics2D.Raycast(ActualStartPoint, currentDirection, 0.1f, groundLayer);
        if (initHit.collider != null)
        {
            // 若起点已在地面内，可自行决定处理方式
        }
    }

    void UpdateFixedDirectionIfNeeded()
    {
        if (useAngleInstead)
        {
            float rad = fixedAngle * Mathf.Deg2Rad;
            fixedDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }
        else
        {
            fixedDirection.Normalize();
        }
    }

    Vector2 GetFixedDirection()
    {
        if (useAngleInstead)
        {
            float rad = fixedAngle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }
        return fixedDirection.normalized;
    }

    void CalculateSpriteNativeWidth()
    {
        if (spriteRenderer.sprite == null)
        {
            spriteNativeWidth = 1f;
            scaleCorrectionFactor = 1f;
            return;
        }
        Sprite sprite = spriteRenderer.sprite;
        float pixelWidth = sprite.rect.width;
        float pixelsPerUnit = sprite.pixelsPerUnit;
        spriteNativeWidth = pixelWidth / pixelsPerUnit;
        scaleCorrectionFactor = 1f / spriteNativeWidth;
    }

    void Update()
    {
        // 绘制调试射线
        Debug.DrawRay(ActualStartPoint, currentDirection * currentLength, Color.red);
        UpdateLaserDirectionAndLength();
        UpdateSequenceFrame();

        //Debug.Log($"{stoneEnterStage}+{stone.stage}");

        if (stoneEnterStage != stone.stage)
        {
            Destroy(gameObject);
        }
    }

    void UpdateLaserDirectionAndLength()
    {
        Vector2 startPos = ActualStartPoint;

        // 计算目标点 (玩家位置 + 偏移)
        Vector2 targetPoint = (Vector2)player.position + targetOffset;

        // 决定目标方向
        Vector2 targetDirection;
        if (useFixedDirection)
        {
            targetDirection = GetFixedDirection();
        }
        else
        {
            targetDirection = (targetPoint - startPos).normalized;
        }

        // 更新方向
        if (enableRotation)
        {
            if (useFixedDirection)
            {
                // 固定方向，但平滑旋转到固定方向 (如果需要平滑)
                currentDirection = Vector3.RotateTowards(currentDirection, targetDirection,
                                                         rotationSpeed * Time.deltaTime, 0.0f);
            }
            else
            {
                // 根据线速度计算角速度
                float angularSpeed = (currentLength > 0.01f) ? tipMoveSpeed / currentLength : rotationSpeed;
                currentDirection = Vector3.RotateTowards(currentDirection, targetDirection,
                                                         angularSpeed * Time.deltaTime, 0.0f);
            }
        }
        else
        {
            // 不旋转，方向保持不变 (若未初始化则用目标方向)
            if (currentDirection == Vector2.zero)
                currentDirection = targetDirection;
        }

        // 根据状态处理长度
        if (currentState == LaserState.Extending)
        {
            // 圆点状态：只旋转，不伸长，长度保持0
            // 检查是否应切换到追踪状态 (例如方向对准)
            if (!useFixedDirection)
            {
                float angleDiff = Vector2.Angle(currentDirection, targetDirection);
                if (angleDiff < 5f)
                {
                    currentState = LaserState.Tracking;
                    PlayBeamAnimation();
                    Debug.Log("方向已对准，进入追踪伸长状态");
                }
            }
            else
            {
                // 固定方向时，可直接进入追踪
                currentState = LaserState.Tracking;
                PlayBeamAnimation();
            }
        }
        else if (currentState == LaserState.Tracking)
        {
            if (useFixedLength)
            {
                // 使用固定长度
                currentLength = Mathf.MoveTowards(currentLength, fixedLength, extensionSpeed * Time.deltaTime);
            }
            else
            {
                // 基于地面检测的长度
                RaycastHit2D hit = Physics2D.Raycast(startPos, currentDirection, Mathf.Infinity, groundLayer);
                if (hit.collider != null)
                {
                    float groundDist = hit.distance;
                    if (currentLength < groundDist)
                    {
                        currentLength += extensionSpeed * Time.deltaTime;
                        if (currentLength > groundDist)
                            currentLength = groundDist;
                    }
                    else
                    {
                        currentLength = groundDist;
                    }
                }
                else
                {
                    // 未击中地面，限制最大长度
                    float maxLen = 100f;
                    currentLength = Mathf.Min(currentLength + extensionSpeed * Time.deltaTime, maxLen);
                }
            }
        }

        UpdateLaserVisual();
    }

    void UpdateLaserVisual()
    {
        Vector2 startPos = ActualStartPoint;
        transform.position = startPos;

        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (currentState == LaserState.Extending)
        {
            // 圆点状态：保持原始大小
            Vector3 newScale = transform.localScale;
            newScale.x = originalScaleY;   // 假设精灵是方形或使用相同缩放
            newScale.y = originalScaleY;
            transform.localScale = newScale;
        }
        else
        {
            // 激光束状态：沿 X 轴拉伸
            Vector3 newScale = transform.localScale;
            newScale.x = currentLength * scaleCorrectionFactor;
            newScale.y = originalScaleY;
            transform.localScale = newScale;
        }
    }

    void UpdateSequenceFrame()
    {
        List<Sprite> currentFrames = (currentState == LaserState.Extending) ? dotFrames : beamFrames;
        if (currentFrames == null || currentFrames.Count == 0) return;

        frameTimer += Time.deltaTime;
        float frameInterval = 1.0f / frameRate;

        if (frameTimer >= frameInterval)
        {
            frameTimer -= frameInterval;
            currentFrameIndex = (currentFrameIndex + 1) % currentFrames.Count;
            Sprite newSprite = currentFrames[currentFrameIndex];

            if (spriteRenderer.sprite != newSprite)
            {
                spriteRenderer.sprite = newSprite;
                CalculateSpriteNativeWidth();
            }
            else
            {
                spriteRenderer.sprite = newSprite;
            }
        }
    }

    void PlayDotAnimation()
    {
        currentFrameIndex = 0;
        frameTimer = 0f;
        if (dotFrames.Count > 0)
        {
            spriteRenderer.sprite = dotFrames[0];
            CalculateSpriteNativeWidth();
        }
    }

    void PlayBeamAnimation()
    {
        currentFrameIndex = 0;
        frameTimer = 0f;
        if (beamFrames.Count > 0)
        {
            spriteRenderer.sprite = beamFrames[0];
            CalculateSpriteNativeWidth();
        }
    }

    // 外部调用设置目标
    public void SetLaser(Stone stone)
    {
        player = stone.player;
        this.stone = stone;
        stoneEnterStage = stone.stage;
        Destroy(gameObject, existenceTime);
    }

    // 在 Inspector 中修改固定角度时自动更新方向向量
    private void OnValidate()
    {
        UpdateFixedDirectionIfNeeded();
    }
    private void OnDestroy()
    {
        stone.skillOver = true;
    }
}