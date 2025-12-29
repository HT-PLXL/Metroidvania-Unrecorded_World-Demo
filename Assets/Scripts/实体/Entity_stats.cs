using UnityEngine;

public class Entity_stats : MonoBehaviour
{
    public Stat_ResourceGroup resources;
    public Stat_MajorGroup major;
    public Stat_AttackGroup attack;
    public Stat_DefenseGroup defense;
    public float GetElementalDamage(out ElementType element,float scareFactor = 1)//scareFactor比例因子，用于实现不同倍率的伤害
    {
        float fireDamage = attack.fireDamage.GetValue();
        float iceDamage = attack.iceDamage.GetValue();
        float lightningDamage = attack.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue();//每点智力+1元素伤害

        float hightstDamage = fireDamage;
        element = ElementType.Fire;

        if (iceDamage > hightstDamage)
        {
            hightstDamage = iceDamage;
            element = ElementType.Ice;
        }
        if (lightningDamage > hightstDamage)
        {
            hightstDamage = lightningDamage;
            element = ElementType.Lightning;
        }
        if (hightstDamage <= 0)
        {
            element = ElementType.None;
            return 0;
        }
        float bonusFire = (element == ElementType.Fire) ? 0 : fireDamage * .5f;
        float bonusIce = (element == ElementType.Ice) ? 0 : iceDamage * .5f;
        float bonusLightning = (element == ElementType.Lightning) ? 0 : lightningDamage * .5f;

        float weakerElementsDamage = bonusFire + bonusIce + bonusLightning;
        float finalDamage = hightstDamage + weakerElementsDamage + bonusElementalDamage;
        return finalDamage * scareFactor;
    }
    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * .5f;//每点智力+0.5抗性

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
        float bounsMaxHp = major.vitality.GetValue() * 5;
        float finalMaxHp = baseMaxHp + bounsMaxHp;
        return finalMaxHp;
    }
    public float GetArmorMitigation(float armorReduction)//护甲减免
    {
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = major.vitality.GetValue();//每点活力值+1护甲
        float totalArmor = baseArmor + bonusArmor;

        float reductionMultiplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveArmor = totalArmor * reductionMultiplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100);//调整常数即可改变每点护甲收益
        float mitigationCap = 0.99f;//最大护甲值伤害减免
        float finalmitigation = Mathf.Clamp(mitigation, 0, mitigationCap);

        return finalmitigation;
    }
    public float GetArmorReduction()//护甲削减
    {
        float finalReduction = attack.armorReduction.GetValue() / 100;
        return finalReduction;
    }
    public float GetPhyiscalDamage(out bool isCrit, float scareFactor = 1)//out关键字用于额外返回多个值供调用，此处的isCrit在形参中声明在方法体中确定值
    {
        float baseDamage = attack.damage.GetValue();
        float bonusDamage = major.strength.GetValue();
        float totalBaseDamage = baseDamage + bonusDamage;

        float baseCritChance = attack.critChance.GetValue();
        float bonusCritChance = major.agility.GetValue() * 0.3f;//每点敏捷+0.3%暴击率
        float finalCritChance = baseCritChance + bonusCritChance;

        float baseCritPower = attack.critPower.GetValue();
        float bonusCritPower = major.strength.GetValue() * 0.5f;//每点力量+0.5%的暴击伤害
        float finalCritPower = (baseCritPower + bonusCritPower) / 100;//最终暴击伤害（倍率）

        isCrit = Random.Range(0, 100) < finalCritChance;//注意形参列表中已经声明isCrit为bool值
        float finalDamage = isCrit ? totalBaseDamage * finalCritPower : totalBaseDamage;

        return finalDamage * scareFactor;
    }
    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * .5f;//每点敏捷+0.5%闪避率

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 85f;//上限值

        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap);//数学-钳制（插值）函数，限制第一个值在第二第三值之间，超出范围的会被自动约束

        return finalEvasion;
    }
}