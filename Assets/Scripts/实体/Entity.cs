using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action 翻转中;//声明一个封装了委托的事件，委托可以理解为任务列表
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    protected StateMachine stateMachine;//声明状态机容器（即变量stateMachine）


    private bool 面向右方 = true;
    public int 面对方向 { get; private set; } = 1;//区分正负，可当被乘数用于确定方向

    [Header("碰撞检测")]
    [SerializeField] protected LayerMask 什么是地面或墙壁;
    [SerializeField] private float 地面检测距离;
    [SerializeField] private float 墙壁检测距离;
    [SerializeField] private Transform 地面检测起点;
    //声明地面检测再给它赋值实体自身，就可以用地面检测.position代替xxx.position
    //好处是如果拖入子对象，就可以通过调整子对象的位置实现从不同的位置比如身前进行地面检测
    [SerializeField] private Transform 首要的墙壁检测;
    [SerializeField] private Transform 次要的墙壁检测;
    public bool 检测到地面 { get; private set; }
    public bool 检测到墙壁 { get; private set; }

    private Coroutine 击退协程;
    private bool 被击退;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();//为stateMachine创建实例，确保被使用时传出的参数不为null

    }

    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
        stateMachine.UpdataActiveState();
        碰撞检测();
    }
    public virtual void 实体死亡()
    {

    }

    public void 受击击退(Vector2 击退,float 持续时间)
    {
        if (击退协程 != null)
            StopCoroutine(击退协程);
        击退协程 = StartCoroutine(被击退协程(击退, 持续时间));
    }
    private IEnumerator 被击退协程(Vector2 击退, float 持续时间)
    {
        被击退 = true;
        rb.linearVelocity = 击退;
        yield return new WaitForSeconds(持续时间);
        rb.linearVelocity = Vector2.zero;
        被击退 = false;
    }
    public void 当前状态动画事件触发器()
    {
        stateMachine.currentState.动画事件触发();
        //注意currentState是在stateMachine中定义的EntityState类型变量
        //实例.属性.对属性所属类型执行方法（）
        //所以调用的EntityState中的动画事件触发()
    }
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (被击退)
            return;//受击ban移动
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        翻转处理(xVelocity);
    }

    public void 翻转()
    {
        transform.Rotate(0, 180, 0);
        面向右方 = !面向右方;
        面对方向 = 面对方向 * -1;

        翻转中?.Invoke();//实体翻转的时候调用事件保持血条不翻转
    }

    public void 翻转处理(float xVelocity)
    {
        if (xVelocity > 0 && 面向右方 == false)
        {
            翻转();
        }
        else if (xVelocity < 0 && 面向右方 == true)
        {
            翻转();
        }
    }

    private void 碰撞检测()
    {
        检测到地面 = Physics2D.Raycast(地面检测起点.position, Vector2.down, 地面检测距离, 什么是地面或墙壁);
        //2d.光线投射（起始点，方向，距离，所需检测图层）
        if (次要的墙壁检测 != null)
        {
            检测到墙壁 = Physics2D.Raycast(首要的墙壁检测.position, Vector2.right * 面对方向, 墙壁检测距离, 什么是地面或墙壁)
                      && Physics2D.Raycast(次要的墙壁检测.position, Vector2.right * 面对方向, 墙壁检测距离, 什么是地面或墙壁);
        }
        else
            检测到墙壁 = Physics2D.Raycast(首要的墙壁检测.position, Vector2.right * 面对方向, 墙壁检测距离, 什么是地面或墙壁);
    }
    protected virtual void OnDrawGizmos()//辅助线
    {
        Gizmos.DrawLine(地面检测起点.position, 地面检测起点.position + new Vector3(0, -地面检测距离, 0));
        Gizmos.DrawLine(首要的墙壁检测.position, 首要的墙壁检测.position + new Vector3(墙壁检测距离 * 面对方向, 0, 0));
        if (次要的墙壁检测 != null)
            Gizmos.DrawLine(次要的墙壁检测.position, 次要的墙壁检测.position + new Vector3(墙壁检测距离 * 面对方向, 0, 0));
    }
}
