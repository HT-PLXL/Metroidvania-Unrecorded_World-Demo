using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Platform : MonoBehaviour
{
    [Header("碰撞体引用")]
    [Tooltip("用于站立的实体碰撞体 (IsTrigger = false)")]
    [SerializeField] private Collider2D solidCollider;
    [Tooltip("检测玩家进出区域的触发器 (IsTrigger = true)，需完全包裹平台")]
    [SerializeField] private Collider2D triggerZone;

    private bool playerInZone;          // 玩家是否在触发器内
    private bool isPassingThrough;      // 是否正在忽略碰撞（穿过中）
    private Player player;              // 玩家引用

    private void Start()
    {
        // 自动查找碰撞体（未手动拖拽时）
        if (solidCollider == null)
        {
            foreach (var col in GetComponents<Collider2D>())
            {
                if (!col.isTrigger)
                {
                    solidCollider = col;
                    break;
                }
            }
        }
        if (triggerZone == null)
        {
            foreach (var col in GetComponents<Collider2D>())
            {
                if (col.isTrigger)
                {
                    triggerZone = col;
                    break;
                }
            }
        }

        player = Player.instance;
    }

    private void Update()
    {
        if (player == null || !playerInZone)
            return;

        // 未处于穿过状态时，检测是否要开始穿过
        if (!isPassingThrough)
        {
            // 条件1：跳跃状态（普通跳跃或蹬墙跳） → 从下方跳起穿过
            bool jumpThrough = player.CompareCurrentState(player.jumpState)
                            || player.CompareCurrentState(player.wallJumpState);

            // 条件2：按“下”键 → 从平台上落下穿过
            bool dropThrough = player.moveInput.y < 0f;

            if (jumpThrough || dropThrough)
                StartPassThrough();
        }
        // 一旦开始穿过，将持续忽略碰撞，直到玩家离开触发器区域（在OnTriggerExit2D中恢复）
    }

    private void StartPassThrough()
    {
        if (isPassingThrough) return;
        isPassingThrough = true;

        // 忽略玩家所有碰撞体与平台实体碰撞体的碰撞
        var playerColliders = player.GetComponentsInChildren<Collider2D>();
        foreach (var col in playerColliders)
        {
            if (col != solidCollider && col != null)
                Physics2D.IgnoreCollision(solidCollider, col, true);
        }
    }

    private void EndPassThrough()
    {
        if (!isPassingThrough) return;
        isPassingThrough = false;

        var playerColliders = player.GetComponentsInChildren<Collider2D>();
        foreach (var col in playerColliders)
        {
            if (col != solidCollider && col != null)
                Physics2D.IgnoreCollision(solidCollider, col, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
            playerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            playerInZone = false;
            EndPassThrough(); // 离开区域立即恢复碰撞，重新可站立
        }
    }
}