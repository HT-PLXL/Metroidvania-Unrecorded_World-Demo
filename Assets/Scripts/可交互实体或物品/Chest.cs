using UnityEngine;
//宝箱脚本
public class Chest : MonoBehaviour, IDamageable
{
    //当类附加了接口的时候，就必须实现接口的功能，相当于一种约束
    private Animator anim;
    private Rigidbody2D rb;
    private Entity_VFX vfx;
    private Entity_DropManager dropManager;
    [Header("打开细节")]
    [SerializeField][cn("被击退力度")] private Vector2 knockBack;
    [SerializeField][cn("开启掉落物机制？")] private bool CanDropItem = true;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody2D>();
        vfx = GetComponent<Entity_VFX>();
        dropManager = GetComponent<Entity_DropManager>();
    }
    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer, bool isCrit = false)
    {
        if (CanDropItem == true)
        {
            dropManager?.DropItems();
            CanDropItem = false;
        }
        vfx.PlayBeAttackVFX(ElementType.None);
        anim.SetBool("打开宝箱", true);
        rb.linearVelocity = knockBack;
        rb.angularVelocity = Random.Range(-200f, 200f);

        return true;
    }
}
