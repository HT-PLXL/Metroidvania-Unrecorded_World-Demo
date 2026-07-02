using UnityEngine;

public class Arrow : MonoBehaviour, ICounter
{
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private LayerMask whatIsGround;
    private Collider2D col;
    private Rigidbody2D rb;
    private Entity_Combat combat;
    private Animator anim;

    //击退详情，由弓箭手传参
    private Vector2 Knockback;
    private float KnockbackTime;

    // 跟踪相关
    private bool trackingEnabled = false;
    private TrackingMode trackingMode;
    private float trackingDelayOrDistance;  // 延迟时间(秒) 或 距离(米)
    private float trackingSpeed;             // 跟踪时的速度，-1表示保持原速
    private float originalSpeed;
    private Vector2 startPosition;
    private float spawnTime;
    private bool isTracking = false;
    private bool isHit = false;              // 是否已命中
    public enum TrackingMode
    {
        None,//不跟踪
        TimeDelay,//射出后延迟一段时间开始跟踪
        DistanceThreshold//飞行超过一定距离后开始跟踪
    }
    public bool CanBeCountered => true;

    public void SetupArrow(float xVelocity, Entity_Combat combat, Vector2 Knockback, float KnockbackTime, bool enableTracking, TrackingMode mode, float delayOrDistance, float trackSpeed = -1f)
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
        this.combat = combat;

        this.Knockback = Knockback;
        this.KnockbackTime = KnockbackTime;

        rb.linearVelocity = new Vector2(xVelocity, 0);
        if (rb.linearVelocity.x < 0)
            transform.Rotate(0, 180, 0);

        // 记录起始点和原始速度
        startPosition = transform.position;
        originalSpeed = Mathf.Abs(xVelocity);
        isTracking = false;
        isHit = false;
        spawnTime = Time.time;

        // 跟踪配置
        trackingEnabled = enableTracking;
        if (trackingEnabled)
        {
            trackingMode = mode;
            trackingDelayOrDistance = delayOrDistance;
            trackingSpeed = (trackSpeed > 0) ? trackSpeed : originalSpeed;
        }
        else
            trackingMode = TrackingMode.None;
    }
    private void Update()
    {
        if (isHit || !trackingEnabled) return;

        // 根据跟踪模式判断是否开始跟踪
        if (!isTracking)
        {
            bool startTrack = false;
            switch (trackingMode)
            {
                case TrackingMode.TimeDelay:
                    if (Time.time - spawnTime >= trackingDelayOrDistance)
                        startTrack = true;
                    break;
                case TrackingMode.DistanceThreshold:
                    if (Vector2.Distance(transform.position, startPosition) >= trackingDelayOrDistance)
                        startTrack = true;
                    break;
                default:
                    return;
            }
            if (startTrack) isTracking = true;
        }

        if (isTracking)
        {
            TrackPlayer();
        }
    }
    private void TrackPlayer()
    {
        if (Player.instance == null) return;

        Vector2 direction = (Player.instance.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * trackingSpeed;

        // 旋转箭矢指向移动方向（可选）
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // &位与，有0则0，<<左移符，LayerMask是32位int数，对应层级序号位为1
        //1的二进制左移layer序号位 然后异或 whatIsTarget 之后不为0说明存在同为1的位，即layer匹配
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0)
        {
            collision.GetComponent<EntityHp>().SetCustomKnockback(Knockback, KnockbackTime);//设定击退
            combat.PerformAttackOnTarget(collision.transform);
        }
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0 || ((1 << collision.gameObject.layer) & whatIsGround) != 0)
        {
            StopArrowOnHit(collision.transform);
        }
    }
    private void StopArrowOnHit(Transform target)
    {
        isHit = true;
        rb.linearVelocity = Vector2.zero;
        //刚体类型设置为运动学（Kinematic）由代码控制运动，不受重力、物理力影响，但可与其它物体碰撞并推动它们
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;
        anim.enabled = false;//禁用动画
        GetComponentInChildren<TrailRenderer>().enabled = false;//关闭轨迹
        transform.parent = target;
        Destroy(gameObject, 3);
    }
    public void HandleCounter()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * -1, 0);
        transform.Rotate(0, 180, 0);

        // 切换攻击者为玩家，如果反弹后命中敌人，OnTriggerEnter2D中会基于玩家设置正确的击退方向
        Player player = Player.instance;
        if (player != null)
            combat = player.playerCombat;

        int enemyLayer = LayerMask.NameToLayer("敌人");
        // | 位或，有1则1，相当于给whatIsTarget加上enemyLayer对应位
        whatIsTarget = whatIsTarget | (1 << enemyLayer);

        // 弹反后禁用跟踪功能
        trackingEnabled = false;
        isTracking = false;
    }
}
