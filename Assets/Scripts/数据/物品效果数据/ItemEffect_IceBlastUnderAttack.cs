using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/物品效果/冰爆", fileName = "物品效果-冰爆")]

public class ItemEffect_IceBlastUnderAttack : ItemEffectDataSO
{
    [SerializeField] private ElementEffectData effectData;
    [SerializeField] private float IceDamage;
    [SerializeField] private LayerMask WhoIsEnemy;
    [SerializeField] private float HpPercentTrigger = 0.25f;
    [SerializeField] private float cooldown;
    [System.NonSerialized] private float lastTimeBeUsed = -99f;//防止值被保存到装备上导致下次运行冷却异常
    [Header("特效对象")]
    [SerializeField] private GameObject iceBlastPrefab;
    [SerializeField] private GameObject onHitPrefab;
    public override void ExecuteEffect()
    {
        bool noCooldown = Time.time >= lastTimeBeUsed + cooldown;
        bool meetCondition = player.Hp.GetHpPercent() <= HpPercentTrigger;

        if (noCooldown && meetCondition)
        {
            player.vfx.CreateEffectForTarget(iceBlastPrefab, player.transform);
            lastTimeBeUsed = Time.time;
            DamageEnemybyIceBlast();
        }
    }
    private void DamageEnemybyIceBlast()//冰爆的伤害方法
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, 1.5f, WhoIsEnemy);
        foreach (var target in enemies)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null)
                continue;
            bool targetGotHit = damageable.TakeDamage(0, IceDamage, ElementType.Ice, player.transform);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();
            statusHandler?.ApplyStatusEffect(ElementType.Ice, effectData);
            if (targetGotHit)
                player.vfx.CreateEffectForTarget(onHitPrefab, target.transform);
        }
    }
    public override void Subscribe(Player player)//订阅
    {
        base.Subscribe(player);
        player.Hp.OnUnderAttack += ExecuteEffect;
    }
    public override void Unsubscribe()
    {
        base.Unsubscribe();
        player.Hp.OnUnderAttack -= ExecuteEffect;
        player = null;
    }
}
