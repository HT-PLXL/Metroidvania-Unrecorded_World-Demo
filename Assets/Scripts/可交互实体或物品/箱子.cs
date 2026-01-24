using UnityEngine;

public class 箱子 : MonoBehaviour, IDamageable
{
    //当类附加了接口的时候，就必须实现接口的功能，相当于一种约束
    private Animator anim;
    private Rigidbody2D rb;
    private Entity_VFX vfx;
    [Header("打开细节")]
    [SerializeField] private Vector2 被击退;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody2D>();
        vfx = GetComponent<Entity_VFX>();
    }
    public bool TakeDamage(float damage,float elementalDamage,ElementType element, Transform damageDealer)
    {
        vfx.播放受伤视觉效果(ElementType.None);
        anim.SetBool("打开宝箱", true);
        rb.linearVelocity = 被击退;
        rb.angularVelocity = Random.Range(-200f, 200f);

        return true;
    }
}
