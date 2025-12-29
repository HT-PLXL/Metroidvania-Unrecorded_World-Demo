using UnityEngine;
using UnityEngine.UI;

public class EntityHp : MonoBehaviour, IDamageable
{
    private Slider HpBar;
    private Entity_VFX entityVfx;
    private Entity entity;
    private Entity_stats entityStats;

    [SerializeField] protected float currentHp;
    [SerializeField] public bool isDead;
    [Header("血量回复")]
    [SerializeField] private float regenInterval = 1;
    [SerializeField] private bool canRegenHp = true;
    [Header("受击击退")]
    [SerializeField] private float 击退硬直时间 = .2f;
    [SerializeField] private Vector2 击退力度 = new Vector2(1.5f, 2f);
    [Header("强力攻击与击退")]
    [SerializeField] private float 强力击退伤害阈值 = 0.3f;
    [SerializeField] private float 强力击退硬直时间 = 0.5f;
    [SerializeField] private Vector2 强力击退力度 = new Vector2(7, 7);
    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        HpBar = GetComponentInChildren<Slider>();
        entityStats = GetComponent<Entity_stats>();

        currentHp = entityStats.GetMaxHp();
        HpBarUpdate();

        InvokeRepeating(nameof(RegenHp), 0, regenInterval);//自带的重复调用函数(方法名，延迟，间隔)，方法名可以使用‘ “方法名” ’，但使用nameof转为字符串利于修改，比如ctrl+R+R的名称修改
    }
    public virtual bool 造成伤害(float damage,float elementalDamage, ElementType element, Transform 伤害来源)
    {
        if (isDead)
            return false;

        if (attackEvaded())
        {
            Debug.Log($"{gameObject.name} evaded the attack!");
            return false;
        }

        Entity_stats attackerStats = 伤害来源.GetComponent<Entity_stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;

        float mitigation = entityStats.GetArmorMitigation(armorReduction);//护甲在实体自身上，可以直接使用，削减在攻击者身上，需要获取并提供给方法以计算最终减免比例
        float finalDamage = damage * (1 - mitigation);

        float resistance = entityStats.GetElementalResistance(element);
        float finalElementalDamage = elementalDamage * (1 - resistance);

        施加击退(伤害来源, finalDamage);

        ReduceHp(finalDamage);

        return true;
    }

    private bool attackEvaded()
    {
        return Random.Range(0,100) < entityStats.GetEvasion();
    }
    public void RegenHp()//自动回复生命值
    {
        if(canRegenHp == false)
            return;
        float regenAmount = entityStats.resources.HpRegen.GetValue();
        IncreaseHp(regenAmount);
    }
    public void IncreaseHp(float healAmount)//用于增加当前生命值的基础方法
    {
        if(isDead)
            return;
        
        float newHp = currentHp + healAmount;
        float maxHp = entityStats.GetMaxHp();
        currentHp = Mathf.Min(newHp, maxHp);//限制增加后的血量不高于最大血量
        HpBarUpdate();
    }
    public void ReduceHp(float damage)
    {
        entityVfx ?. 播放受伤视觉效果();//此处是变白效果，打击效果在实体战斗中
        currentHp -= damage;
        HpBarUpdate();
        if (currentHp <= 0)
            Die();
    }
    private void Die()
    {
        isDead = true;
        entity.实体死亡();
        entityVfx.StopAllVfx();
    }
    private void HpBarUpdate()
    {
        if(HpBar == null)
            return;

        HpBar.value = currentHp / entityStats.GetMaxHp();
    }
    private void 施加击退(Transform 伤害来源, float finalDamage)
    {
        float 持续时间 = 计算持续时间(finalDamage);
        Vector2 击退 = 计算受击方向与力度(finalDamage, 伤害来源);
        entity?.受击击退(击退, 持续时间);
    }
    private Vector2 计算受击方向与力度(float damage, Transform 伤害来源)
    {
        int direction = transform.position.x > 伤害来源.position.x ? 1 : -1;
        Vector2 击退 = 是否是强力攻击(damage) ? 强力击退力度 : 击退力度;
        击退.x = 击退.x * direction;
        return 击退;
    }
    private float 计算持续时间(float damage)
    {
        return 是否是强力攻击(damage) ? 强力击退硬直时间 : 击退硬直时间;
    }
    private bool 是否是强力攻击(float damage) => damage / entityStats.GetMaxHp() > 强力击退伤害阈值;
}
