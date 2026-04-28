using UnityEngine;

public class Arrow : MonoBehaviour, ICounter
{
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private LayerMask whatIsGround;
    private Collider2D col;
    private Rigidbody2D rb;
    private Entity_Combat combat;
    private Animator anim;

    public bool CanBeCountered => true;

    public void SetupArrow(float xVelocity, Entity_Combat combat)
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
        this.combat = combat;


        rb.linearVelocity = new Vector2(xVelocity, 0);
        if (rb.linearVelocity.x < 0)
            transform.Rotate(0, 180, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // &位与，有0则0，<<左移符，LayerMask是32位int数，对应层级序号位为1
        //1的二进制左移layer序号位 然后异或 whatIsTarget 之后不为0说明存在同为1的位，即layer匹配
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0)
        {
            combat.PerformAttackOnTarget(collision.transform);
        }
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0 || ((1 << collision.gameObject.layer) & whatIsGround) != 0)
        {
            StopArrowOnHit(collision.transform);
        }
    }
    private void StopArrowOnHit(Transform target)
    {
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

        int enemyLayer = LayerMask.NameToLayer("敌人");
        // | 位或，有1则1，相当于给whatIsTarget加上enemyLayer对应位
        whatIsTarget = whatIsTarget | (1 << enemyLayer);
    }
}
