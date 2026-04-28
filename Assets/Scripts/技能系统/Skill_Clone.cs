using UnityEngine;

public class Skill_Clone : Skill_Base
{
    [SerializeField][cn("克隆体预制体")] private GameObject clonePrefab;
    [SerializeField][cn("克隆体存在时间")] private float cloneDuration;
    [Header("巨齿鲨升级")]
    [SerializeField][cn("最大攻击次数")][Range(0,3)]private int maxAttackCounts = 3;
    [SerializeField][cn("克隆几率")] private float CloneChance = 0.33f;
    [Header("灵魂粒子")]
    [SerializeField][cn("敌人伤害转治疗量百分比")] private float damagePercentHeal = 0.3f;
    [cn("灵魂粒子保底治疗量")] public float minRegen = 50f;
    [SerializeField][cn("减少冷却时间(秒)")] private float cooldownReduceSeconds;

    public override void TryUseSkill()//按键执行
    {
        if (CanUseSkill() == false)
            return;
        CreateClone();
        setSkillOnCooldown();
    }
    public void CreateClone(Vector3? targetPosition = null)//生成实体并将此处参数传入实体脚本，vector3? 允许为空的3维向量，设不传参时默认值为null
    {
        Vector3 position = targetPosition ?? transform.position;//选择生成位置，??空合并运算符，不为空则使用1值，空则使用2值
        GameObject clone = Instantiate(clonePrefab, position, Quaternion.identity);
        clone.GetComponent<SkillObject_Clone>().SetUpClone(this,IsPhysicalDamage);
    }
    public float GetDamagePercentHeal()
    {
        if (ShouldUseTrail() == false)
            return 0;
        return damagePercentHeal;
    }
    public float GetcooldownReduceSeconds()
    {
        if (upgradeType != SkillUpgradeType.魔能澎湃)
            return 0;
        return cooldownReduceSeconds;
    }
    public bool CanRemoveDebuff()
    {
        return upgradeType == SkillUpgradeType.神圣加护;
    }
    public bool ShouldUseTrail()
    {
        return upgradeType == SkillUpgradeType.悲悯之灵 || upgradeType == SkillUpgradeType.神圣加护 || upgradeType == SkillUpgradeType.魔能澎湃;
    }
    public float GetCloneChance()
    {
        if (upgradeType != SkillUpgradeType.永生意志)
            return 0;
        return CloneChance;
    }
    public int GetMaxAttackCounts()
    {
        if (upgradeType == SkillUpgradeType.投影咬人啦 || upgradeType == SkillUpgradeType.永生意志)
            return 1;
        if (upgradeType == SkillUpgradeType.巨齿鲨)
            return maxAttackCounts;
        return 0;
    }
    public float GetCloneDuration()
    {
        return cloneDuration;
    }
}