using UnityEngine;
using UnityEngine.UI;

public class 实体血量 : MonoBehaviour, IDamageable
{
    private Slider 血条;
    private 实体视觉效果 实体视觉效果;
    private Entity entity;
    private Entity_stats stats;

    [SerializeField] protected float currentHp;
    [SerializeField] protected bool isDead;
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
        实体视觉效果 = GetComponent<实体视觉效果>();
        血条 = GetComponentInChildren<Slider>();
        stats = GetComponent<Entity_stats>();

        currentHp = stats.GetMaxHp();
        血条更新();
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

        float mitigation = stats.GetArmorMitigation(armorReduction);//护甲在实体自身上，可以直接使用，削减在攻击者身上，需要获取并提供给方法以计算最终减免比例
        float finalDamage = damage * (1 - mitigation);

        float resistance = stats.GetElementalResistance(element);
        float finalElementalDamage = elementalDamage * (1 - resistance);

        施加击退(伤害来源, finalDamage);

        减少血量(finalDamage);

        return true;
    }

    private bool attackEvaded()
    {
        return Random.Range(0,100) < stats.GetEvasion();
    }
    private void 施加击退(Transform 伤害来源, float finalDamage)
    {
        float 持续时间 = 计算持续时间(finalDamage);
        Vector2 击退 = 计算受击方向与力度(finalDamage, 伤害来源);
        entity?.受击击退(击退, 持续时间);
    }

    protected void 减少血量(float damage)
    {
        实体视觉效果 ?. 播放受伤视觉效果();//此处是变白效果，打击效果在实体战斗中
        currentHp -= damage;
        血条更新();
        if (currentHp <= 0)
            Die();
    }
    private void Die()
    {
        isDead = true;
        entity.实体死亡();
    }
    private void 血条更新()
    {
        if(血条 == null)
            return;

        血条.value = currentHp / stats.GetMaxHp();
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
    private bool 是否是强力攻击(float damage) => damage / stats.GetMaxHp() > 强力击退伤害阈值;
}
