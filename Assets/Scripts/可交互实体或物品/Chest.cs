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
    [Header("重生设置")]
    [cn("是否可重生")] public bool canRebornOnLoad = true;//默认可以重生（重新加载场景重新出现）
    [SerializeField][cn("不可重生请手动设置唯一id")] private string chestId;//唯一标识，用于不可重生宝箱的存档
    private bool isOpened = false;//当前是否已打开（防止重复操作）
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

        if (isOpened)
            return true;

        isOpened = true;
        //如果不可重生，记录到 SaveManager（下次加载时会直接销毁）
        if (!canRebornOnLoad)
        {
            if (string.IsNullOrEmpty(chestId))
                Debug.LogError($"宝箱 {gameObject.name} 缺少 chestId，无法保存永久打开状态");
            else
                SaveManager.instance?.MarkChestOpened(chestId);
        }

        return true;
    }
    public string GetChestId() => chestId;// 供 SaveManager 获取 ID
    // 供 SaveManager 在加载存档时强制设置宝箱为已打开状态
    public void SetOpenedState(bool opened)
    {
        if (isOpened == opened) return;
        isOpened = opened;
        if (opened)
        {
            // 执行打开效果：动画、关闭碰撞、防止掉落两次等
            if (CanDropItem)
            {
                dropManager?.DropItems();
                CanDropItem = false;
            }
            anim.SetBool("打开宝箱", true);
            // 可选：禁用碰撞体，防止再次被攻击
            var coll = GetComponent<Collider2D>();
            if (coll != null) coll.enabled = false;
        }
    }
}
