using UnityEngine;

public class ElementEffectData
{
    public float chillDuration;
    public float chillSlowMulitiplier;
    public float burnDuration;
    public float totalBurnDamage;
    public float lightningDuration;
    public float lightningDamage;
    public float lightningCharge;

    public ElementEffectData(Entity_Stats entity_Stats, DamageScaleFactor damageScale )
    {
        chillDuration = damageScale.chillDuration;
        chillSlowMulitiplier = damageScale.chillSlowMulitiplier;
        burnDuration = damageScale.burnDuration;
        totalBurnDamage = entity_Stats.attack.fireDamage.GetValue() * damageScale.burnDamageScale;
        lightningDuration = damageScale.lightningDuration;
        lightningDamage = entity_Stats.attack.lightningDamage.GetValue() * damageScale.lightningDamageScale;
        lightningCharge= damageScale.lightningCharge;
    }

}


