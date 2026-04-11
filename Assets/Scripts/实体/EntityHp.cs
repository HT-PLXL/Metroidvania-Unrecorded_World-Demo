using System;
using UnityEngine;
using UnityEngine.UI;

public class EntityHp : MonoBehaviour, IDamageable
{
    public event Action OnUnderAttack;
    public event Action OnHpUpdate;
    private Slider HpBar;
    private Entity_VFX entityVfx;
    private Entity entity;
    private Entity_Stats entityStats;
    private Entity_DropManager dropManager;//掉落物管理器
    //private bool miniHpActive;

    public float lastDamage { get; private set; }//用于灵魂粒子
    public bool isDead { get; private set; }
    protected bool canTakeDamage = true;
    [SerializeField] public float currentHp;
    [Header("血量回复")]
    [SerializeField][cn("回复间隔")] private float regenInterval = 1;
    [SerializeField][cn("允许自动回复？")] private bool canRegenHp = true;
    [Header("hitBack")]
    [SerializeField][cn("击退硬直时间")] private float hitBackStunDuration = .2f;
    [SerializeField][cn("击退力度")] private Vector2 hitBackForce = new Vector2(1.5f, 2f);
    [Header("强力攻击与击退")]
    [SerializeField][cn("强力击退(伤害/血量)阈值")] private float powerfulHitBackThreshold = 0.3f;
    [SerializeField][cn("强力击退硬直时间")] private float powerfulHitBackDuration = 0.5f;
    [SerializeField][cn("强力击退力度")] private Vector2 powerfulHitBackForce = new Vector2(7, 7);
    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        HpBar = GetComponentInChildren<Slider>();
        entityStats = GetComponent<Entity_Stats>();
        dropManager = GetComponent<Entity_DropManager>();

    }
    protected virtual void Start()
    {   //这个更新血量不能在Awake中实现，否则会影响创建的子史莱姆血量
        if (entityStats != null)
        {
            currentHp = entityStats.GetMaxHp();
            OnHpUpdate += HpBarUpdate;
            HpBarUpdate();
            InvokeRepeating(nameof(RegenHp), 0, regenInterval);//自带的重复调用函数(方法名，延迟，间隔)，方法名可以使用‘ “方法名” ’，但使用nameof转为字符串利于修改，比如ctrl+R+R的名称修改
            //上面这个重复延迟执行方法也可以让血条更新
        }
    }
    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageSource)//对受击目标执行
    {
        if (attackEvaded())//闪避
        {
            Debug.Log($"{gameObject.name} evaded the attack!");
            return false;
        }

        Entity_Stats attackerStats = damageSource.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;

        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0;//护甲在实体自身上，可以直接使用，削减在攻击者身上，需要获取并提供给方法以计算最终减免比例
        float finalPhysicalDamage = damage * (1 - mitigation);

        float resistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0;
        float finalElementalDamage = elementalDamage * (1 - resistance);

        UseHitBack(damageSource, element != ElementType.None ? finalElementalDamage : finalPhysicalDamage);

        if (element == ElementType.None)
            ReduceHp(finalPhysicalDamage);
        else
            ReduceHp(finalElementalDamage, element);
        lastDamage = finalPhysicalDamage + finalElementalDamage;

        OnUnderAttack?.Invoke();//每次受到攻击执行
        return true;
    }
    public float GetHpPercent()
    {
        float hpPercent = currentHp / entityStats.GetMaxHp();
        return hpPercent;
    }
    public void CanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;
    private bool attackEvaded()
    {
        if (entityStats == null)
            return false;
        else
            return UnityEngine.Random.Range(0, 100) < entityStats.GetEvasion();
    }
    public void RegenHp()//自动回复生命值
    {
        if (canRegenHp == false)
            return;
        float regenAmount = entityStats.resources.HpRegen.GetValue();
        IncreaseHp(regenAmount);
    }
    public void IncreaseHp(float healAmount)//用于增加当前生命值的基础方法
    {
        if (isDead)
            return;

        float newHp = currentHp + healAmount;
        float maxHp = entityStats.GetMaxHp();
        currentHp = Mathf.Min(newHp, maxHp);//限制增加后的血量不高于最大血量

        OnHpUpdate?.Invoke();
    }
    public void ReduceHp(float damage)
    {
        entityVfx?.PlayBeAttackVFX(ElementType.None);//此处是变白效果，打击效果在实体战斗中
        currentHp -= damage;

        OnHpUpdate?.Invoke();
        if (currentHp <= 0)
            Die();
    }
    public void ReduceHp(float damage, ElementType element)//元素伤害专用，不播放变白效果
    {
        entityVfx?.PlayBeAttackVFX(element);//此处是变白效果，打击效果在实体战斗中
        currentHp -= damage;

        OnHpUpdate?.Invoke();
        if (currentHp <= 0)
            Die();
    }
    protected virtual void Die()
    {
        isDead = true;
        entity.EntityDead();
        entityVfx.StopAllVfx();
        dropManager?.DropItems();//怪物掉落物
    }
    private void HpBarUpdate()
    {
        if (HpBar == null && HpBar.transform.parent.gameObject.activeSelf == false)
            return;

        HpBar.value = currentHp / entityStats.GetMaxHp();
    }
    public void EnableHpBar(bool enable)
    {
        HpBar?.transform.parent.gameObject.SetActive(enable);
    }
    private void UseHitBack(Transform damageSource, float finalDamage)
    {
        float duration = CalculateDuration(finalDamage);
        Vector2 hitBack = CalculateDirectionAndForce(finalDamage, damageSource);
        entity?.StartknockBackCo(hitBack, duration);
    }
    private Vector2 CalculateDirectionAndForce(float damage, Transform damageSource)
    {
        int direction = transform.position.x > damageSource.position.x ? 1 : -1;
        Vector2 hitBack = IsHeavyDamage(damage) ? powerfulHitBackForce : hitBackForce;
        hitBack.x = hitBack.x * direction;
        return hitBack;
    }
    private float CalculateDuration(float damage)
    {
        return IsHeavyDamage(damage) ? powerfulHitBackDuration : hitBackStunDuration;
    }
    private bool IsHeavyDamage(float damage)
    {
        if (entityStats == null)
            return false;
        else
            return damage / entityStats.GetMaxHp() > powerfulHitBackThreshold;
    }
}
