using UnityEngine;

/// <summary>
/// 虚空区域：玩家进入后重新加载场景（复活到重生点，不触发死亡UI）；
/// 怪物进入后受到巨额伤害直接死亡。
/// </summary>
public class FallToVoid : MonoBehaviour
{
    [Header("玩家设置")]
    [SerializeField] private bool resetPlayerOnEnter = true;   // 是否重置玩家场景

    [Header("怪物设置")]
    [SerializeField] private float monsterDamage = 99999f;     // 对怪物造成的伤害值

    //防止在同一个触发器中重复调用场景加载
    private bool isLoadingScene = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1) 处理玩家
        if (resetPlayerOnEnter)
        {
            Player player = other.GetComponent<Player>();
            if (player != null && !isLoadingScene)
            {
                isLoadingScene = true;
                //重新加载当前场景，回到最近的重生点，不经过死亡流程
                GameSceneManager.instance?.RestartScene();
                return;
            }
        }

        // 2) 处理怪物（敌人）
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemy.enemyHp != null)
        {
            //直接扣除巨额生命值（无视防御，必定致死）
            enemy.enemyHp.ReduceHp(monsterDamage);
        }
    }

#if UNITY_EDITOR
    //区域辅助线
    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        Gizmos.color = Color.magenta;
        if (col is BoxCollider2D box)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)box.offset, box.size);
        }
        else if (col is CircleCollider2D circle)
        {
            Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
#endif
}