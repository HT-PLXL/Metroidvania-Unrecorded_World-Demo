using UnityEngine;
//此类集中玩家的属性值，并提供了属性的计算方法
public class Entity_Stats : MonoBehaviour
{
    [cn("默认数据-右键菜单更新")] public Stat_SetupSO defaultStatSetup;

    [cn("最大生命值与回复")] public Stat_ResourceGroup resources;//资源
    [cn("攻击")] public Stat_AttackGroup attack;
    [cn("防御")] public Stat_DefenseGroup defense;
    [cn("基础附加属性")] public Stat_MajorGroup major;
    [Header("免疫设定")]
    public bool immunePhysical;
    public bool immuneFire;
    public bool immuneIce;
    public bool immuneLightning;

    [HideInInspector] public ElementType currentWeaponElement = ElementType.None;

    protected virtual void Awake()
    {
        
    }
    public AttackData GetAttackData(DamageScaleFactor damageScale)
    {
        return new AttackData(this, damageScale);
    }
    public void SetWeaponElement(ElementType element) => currentWeaponElement = element;
    /// <summary>
    /// 通过降低/提升的百分比调整子史莱姆的属性
    /// </summary>
    /// <param name="reduction">降低百分比</param>
    /// <param name="increase">提升百分比</param>
    public void GetNewSlimeStat(Stat_ResourceGroup resourceGroup, Stat_AttackGroup offenseGroup, Stat_DefenseGroup defenseGroup, float reduction, float increase)
    {   //没有在此修改的值就是大史莱姆的默认值

        // 提升的属性
        attack.attackSpeed.SetBaseValue(offenseGroup.attackSpeed.GetValue() * increase);
        attack.critChance.SetBaseValue(offenseGroup.critChance.GetValue() * increase);
        attack.critPower.SetBaseValue(offenseGroup.critPower.GetValue() * increase);
        attack.fireDamage.SetBaseValue(offenseGroup.fireDamage.GetValue() * increase);
        attack.iceDamage.SetBaseValue(offenseGroup.iceDamage.GetValue() * increase);
        attack.lightningDamage.SetBaseValue(offenseGroup.lightningDamage.GetValue() * increase);

        defense.evasion.SetBaseValue(defenseGroup.evasion.GetValue() * increase);

        // 降低的属性
        attack.damage.SetBaseValue(offenseGroup.damage.GetValue() * reduction);
        resources.maxHp.SetBaseValue(resourceGroup.maxHp.GetValue() * reduction);
        resources.HpRegen.SetBaseValue(resourceGroup.HpRegen.GetValue() * reduction);

        defense.armor.SetBaseValue(defenseGroup.armor.GetValue() * reduction);
        defense.lightningResistance.SetBaseValue(defenseGroup.lightningResistance.GetValue() * reduction);
        defense.fireResistance.SetBaseValue(defenseGroup.fireResistance.GetValue() * reduction);
        defense.iceResistance.SetBaseValue(defenseGroup.iceResistance.GetValue() * reduction);
    }
    public float GetElementalDamage(out ElementType element, float scareFactor = 1)
    {
        // 如果武器指定了元素，则直接使用该元素，伤害取对应元素数值
        if (currentWeaponElement != ElementType.None)
        {
            element = currentWeaponElement;
            float damage = 0;
            switch (element)
            {
                case ElementType.Fire: damage = attack.fireDamage.GetValue(); break;
                case ElementType.Ice: damage = attack.iceDamage.GetValue(); break;
                case ElementType.Lightning: damage = attack.lightningDamage.GetValue(); break;
            }
            // 智力加成保留
            float bonusElementalDamage = major.intelligence.GetValue() * 2;
            return (damage + bonusElementalDamage) * scareFactor;
        }
        // 无武器元素时，返回None
        element = ElementType.None;
        return 0;
    }
    //public float GetElementalDamage(out ElementType element, float scareFactor = 1)//scareFactor比例因子，用于实现不同倍率的伤害
    //{
    //    float fireDamage = attack.fireDamage.GetValue();
    //    float iceDamage = attack.iceDamage.GetValue();
    //    float lightningDamage = attack.lightningDamage.GetValue();

    //    float bonusElementalDamage = (major.intelligence.GetValue() * 2);//每点智力+2元素伤害

    //    float hightstDamage = fireDamage;
    //    element = ElementType.Fire;

    //    if (iceDamage > hightstDamage)
    //    {
    //        hightstDamage = iceDamage;
    //        element = ElementType.Ice;
    //    }
    //    if (lightningDamage > hightstDamage)
    //    {
    //        hightstDamage = lightningDamage;
    //        element = ElementType.Lightning;
    //    }
    //    if (hightstDamage <= 0)
    //    {
    //        element = ElementType.None;
    //        return 0;
    //    }
    //    float bonusFire = (element == ElementType.Fire) ? 0 : fireDamage * .5f;
    //    float bonusIce = (element == ElementType.Ice) ? 0 : iceDamage * .5f;
    //    float bonusLightning = (element == ElementType.Lightning) ? 0 : lightningDamage * .5f;

    //    float weakerElementsDamage = bonusFire + bonusIce + bonusLightning;
    //    float finalDamage = hightstDamage + weakerElementsDamage + bonusElementalDamage;
    //    return finalDamage * scareFactor;
    //}
    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * .5f;//每点智力+0.5各属性抗性

        switch (element)
        {
            case ElementType.Fire:
                baseResistance = defense.fireResistance.GetValue();
                break;
            case ElementType.Ice:
                baseResistance = defense.iceResistance.GetValue();
                break;
            case ElementType.Lightning:
                baseResistance = defense.lightningResistance.GetValue();
                break;
        }
        float resistance = baseResistance + bonusResistance;
        float resistanceCap = 99f;//元素抗性最大值
        float finalResistance = Mathf.Clamp(resistance, 0, resistanceCap) / 100;//转化为百分比

        return finalResistance;
    }
    public float GetMaxHp()
    {
        float baseMaxHp = resources.maxHp.GetValue();
        float bounsMaxHp = major.vitality.GetValue() * 5;//每点活力+5最大生命
        float finalMaxHp = baseMaxHp + bounsMaxHp;
        return finalMaxHp;
    }
    public float GetArmorMitigation(float armorReduction)//护甲值伤害减免
    {
        float baseArmor = GetBaseArmor();

        float reductionMultiplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveArmor = baseArmor * reductionMultiplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100);//调整常数即可改变每点护甲收益
        float mitigationCap = 0.99f;//最大护甲值伤害减免
        float finalmitigation = Mathf.Clamp(mitigation, 0, mitigationCap);

        return finalmitigation;
    }
    public float GetBaseArmor() => defense.armor.GetValue() + (major.vitality.GetValue() * 2);//每点活力值+2护甲
    public float GetArmorReduction()//护甲削减
    {
        float finalReduction = attack.armorReduction.GetValue() / 100;
        return finalReduction;
    }
    public float GetPhysicalDamage(out bool isCrit, float scareFactor = 1)//out关键字用于额外返回多个值供调用，此处的isCrit在形参中声明在方法体中确定值
    {
        float baseDamage = GetBaseDamage();
        float critChance = GetCritChance();
        float critPower = GetCritPower() / 100;

        isCrit = Random.Range(0, 100) < critChance;//注意形参列表中已经声明isCrit为bool值
        float finalDamage = isCrit ? baseDamage *(1+ critPower) : baseDamage;
        return finalDamage * scareFactor;
    }
    public float GetBaseDamage() => attack.damage.GetValue() + (major.strength.GetValue() * 2f);
    public float GetCritChance() => attack.critChance.GetValue() + (major.agility.GetValue() * 0.5f);//每点敏捷+0.5%暴击率
    public float GetCritPower() => attack.critPower.GetValue() + (major.strength.GetValue() * 0.5f);//每点力量+0.5%的暴击伤害
    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * .5f;//每点敏捷+0.5%闪避率

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 85f;//上限值

        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap);//数学-钳制（插值）函数，限制第一个值在第二第三值之间，超出范围的会被自动约束

        return finalEvasion;
    }
    public Stat GetStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHP: return resources.maxHp;
            case StatType.HpRegen: return resources.HpRegen;

            case StatType.Strength: return major.strength;
            case StatType.Agility: return major.agility;
            case StatType.Intelligence: return major.intelligence;
            case StatType.Vitality: return major.vitality;

            case StatType.AttackSpeed: return attack.attackSpeed;
            case StatType.Damage: return attack.damage;
            case StatType.CritChance: return attack.critChance;
            case StatType.CritPower: return attack.critPower;
            case StatType.ArmorReduction: return attack.armorReduction;
            case StatType.FireDamage: return attack.fireDamage;
            case StatType.IceDamage: return attack.iceDamage;
            case StatType.LightningDamage: return attack.lightningDamage;

            case StatType.Armor: return defense.armor;
            case StatType.Evasion: return defense.evasion;
            case StatType.IceResistance: return defense.iceResistance;
            case StatType.FireResistance: return defense.fireResistance;
            case StatType.LightningResistance: return defense.lightningResistance;

            default:
                return null;
        }
    }
    [ContextMenu("更新默认数据")]
    public void ApplyDefaultStatSetup()
    {
        if (defaultStatSetup == null)
        {
            Debug.Log("没有默认数据已分配");
            return;
        }
        resources.maxHp.SetBaseValue(defaultStatSetup.maxHp);
        resources.HpRegen.SetBaseValue(defaultStatSetup.HpRegen);

        major.strength.SetBaseValue(defaultStatSetup.strength);
        major.agility.SetBaseValue(defaultStatSetup.agility);
        major.intelligence.SetBaseValue(defaultStatSetup.intelligence);
        major.vitality.SetBaseValue(defaultStatSetup.vitality);

        attack.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
        attack.damage.SetBaseValue(defaultStatSetup.damage);
        attack.critChance.SetBaseValue(defaultStatSetup.critChange);
        attack.critPower.SetBaseValue(defaultStatSetup.critPower);
        attack.armorReduction.SetBaseValue(defaultStatSetup.armorReduction);


        attack.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        attack.iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
        attack.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);

        defense.armor.SetBaseValue(defaultStatSetup.armor);
        defense.evasion.SetBaseValue(defaultStatSetup.evasion);

        defense.iceResistance.SetBaseValue(defaultStatSetup.iceResistance);
        defense.fireResistance.SetBaseValue(defaultStatSetup.fireResistance);
        defense.lightningResistance.SetBaseValue(defaultStatSetup.LightningResistance);
    }
    //[ContextMenu("更新默认数据")]
    //public void ApplyDefaultStatSetup()
    //{
    //    if (defaultStatSetup == null)
    //    {
    //        Debug.Log("没有默认数据已分配");
    //        return;
    //    }
    //    resources.maxHp.SetBaseValue(defaultStatSetup.maxHp);
    //    resources.HpRegen.SetBaseValue(defaultStatSetup.HpRegen);

    //    major.strength.SetBaseValue(defaultStatSetup.strength);
    //    major.agility.SetBaseValue(defaultStatSetup.agility);
    //    major.intelligence.SetBaseValue(defaultStatSetup.intelligence);
    //    major.vitality.SetBaseValue(defaultStatSetup.vitality);

    //    attack.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
    //    attack.damage.SetBaseValue(defaultStatSetup.damage);
    //    attack.critChance.SetBaseValue(defaultStatSetup.critChange);
    //    attack.critPower.SetBaseValue(defaultStatSetup.critPower);
    //    attack.armorReduction.SetBaseValue(defaultStatSetup.armorReduction);

    //    attack.iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
    //    attack.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
    //    attack.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);

    //    defense.armor.SetBaseValue(defaultStatSetup.armor);
    //    defense.evasion.SetBaseValue(defaultStatSetup.evasion);

    //    defense.iceResistance.SetBaseValue(defaultStatSetup.iceResistance);
    //    defense.fireResistance.SetBaseValue(defaultStatSetup.fireResistance);
    //    defense.lightningResistance.SetBaseValue(defaultStatSetup.LightningResistance);
    //}
}