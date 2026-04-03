using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlip;
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Collider2D col { get; private set; }
    public Entity_SFX sfx { get; private set; }
    protected StateMachine stateMachine;//声明状态机容器（即变量stateMachine）
    private bool isFacingRight = true;
    public int faceDirection { get; private set; } = 1;//区分正负，可当被乘数用于确定方向

    [Header("碰撞检测详情")]
    [cn("什么是地面")] public LayerMask whatIsGround;
    [cn("地面检测距离")][SerializeField] private float groundDetectionDistance;
    [cn("墙壁检测距离")][SerializeField] private float wallDetectionDistance;
    [cn("地面检测起点")][SerializeField] private Transform groundDetectionStart;
    //声明地面检测再给它赋值实体自身，就可以用地面检测.position代替xxx.position
    //好处是如果拖入子对象，就可以通过调整子对象的位置实现从不同的位置比如身前进行地面检测
    [cn("核心墙壁检测（对象）")][SerializeField] private Transform majorWallDetection;
    [cn("次要墙壁检测（对象）")][SerializeField] private Transform secondaryWallDetection;
    public bool isDetectGround { get; private set; }
    public bool isDetectWall { get; private set; }

    private Coroutine knockBackCo;//击退协程
    private Coroutine slowDownCo;//减速协程
    private bool isKnockBack;//被击退

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sfx = GetComponent<Entity_SFX>();
        col = GetComponent<Collider2D>();

        stateMachine = new StateMachine();//为stateMachine创建实例，确保被使用时传出的参数不为null

    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
        stateMachine.UpdataActiveState();
        collisionDetection();
    }
    public virtual void EntityDead()//实体死亡
    {

    }
    public virtual void SlowDownEntity(float duration, float slowMultiplier, bool canOverrideSlowDown = false)
    {
        if (slowDownCo != null)
        {
            if (canOverrideSlowDown)
                StopCoroutine(slowDownCo);
            else
                return;
        }
        slowDownCo = StartCoroutine(SlowDownEntityCo(duration, slowMultiplier));
    }
    protected virtual IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)//协程，允许暂停并在将来执行函数
    {
        yield return null;
    }
    public virtual void StopSlowDown()
    {
        StopCoroutine(slowDownCo);
        //slowDownCo = null;
    }
    public void StartknockBackCo(Vector2 knockBack, float duration)
    {
        if (knockBackCo != null)
            StopCoroutine(knockBackCo);
        knockBackCo = StartCoroutine(wasKnockedBackCo(knockBack, duration));
    }
    private IEnumerator wasKnockedBackCo(Vector2 knockBack, float duration)
    {
        isKnockBack = true;
        rb.linearVelocity = knockBack;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isKnockBack = false;
    }
    public void CurrentStateAnimTrigger()
    {
        stateMachine.currentState.AnimTrigger();
        //注意currentState是在stateMachine中定义的EntityState类型变量
        //实例.属性.对属性所属类型执行方法（）
        //所以调用的EntityState中的AnimTrigger()
    }
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnockBack)
            return;//受击ban移动
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }
    public void Flip()//翻转
    {
        transform.Rotate(0, 180, 0);
        isFacingRight = !isFacingRight;
        faceDirection = faceDirection * -1;

        OnFlip?.Invoke();//实体翻转的时候调用事件保持血条不翻转
    }
    public void HandleFlip(float xVelocity)//handleFlip
    {
        if (xVelocity > 0 && isFacingRight == false)
        {
            Flip();
        }
        else if (xVelocity < 0 && isFacingRight == true)
        {
            Flip();
        }
    }
    private void collisionDetection()
    {
        isDetectGround = Physics2D.Raycast(groundDetectionStart.position, Vector2.down, groundDetectionDistance, whatIsGround);
        //2d.光线投射（起始点，方向，距离，所需检测图层）
        if (secondaryWallDetection != null)
        {
            isDetectWall = Physics2D.Raycast(majorWallDetection.position, Vector2.right * faceDirection, wallDetectionDistance, whatIsGround)
                      && Physics2D.Raycast(secondaryWallDetection.position, Vector2.right * faceDirection, wallDetectionDistance, whatIsGround);
        }
        else
            isDetectWall = Physics2D.Raycast(majorWallDetection.position, Vector2.right * faceDirection, wallDetectionDistance, whatIsGround);
    }
    protected virtual void OnDrawGizmos()//辅助线
    {
        Gizmos.DrawLine(groundDetectionStart.position, groundDetectionStart.position + new Vector3(0, -groundDetectionDistance, 0));
        Gizmos.DrawLine(majorWallDetection.position, majorWallDetection.position + new Vector3(wallDetectionDistance * faceDirection, 0, 0));
        if (secondaryWallDetection != null)
            Gizmos.DrawLine(secondaryWallDetection.position, secondaryWallDetection.position + new Vector3(wallDetectionDistance * faceDirection, 0, 0));
    }
}
