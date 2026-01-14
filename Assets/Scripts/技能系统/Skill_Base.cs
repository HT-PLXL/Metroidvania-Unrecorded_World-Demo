using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    [Header("普通细节")]
    [SerializeField][cn("冷却时间")] protected float cooldown;
    [SerializeField][cn("技能种类")] protected SkillType skillType;
    [SerializeField][cn("技能升级种类")] protected SkillUpgradeType upgradeType;
    private float lastTimeUsed;

    protected virtual void Awake()
    {
        lastTimeUsed = lastTimeUsed - cooldown;//初始/重生时重置冷却
    }
    public virtual void TryUseSkill()
    {

    }
    private bool OnCooldown() => Time.time < lastTimeUsed + cooldown;
    public void setSkillOnCooldown() => lastTimeUsed = Time.time;//使其进入冷却
    private void ResetCooldownBy(float cooldownReduction) => lastTimeUsed = lastTimeUsed + cooldownReduction;//修改冷却时间
    public void ResetCooldown() => lastTimeUsed = Time.time;//重置冷却时间

    public bool CanUseSkill()
    {
        if(upgradeType == SkillUpgradeType.None)
            return false;
        if (OnCooldown())
        {
            Debug.Log("技能冷却中");
            return false;
        }
        return true;
    }
    public void SetSkillUpgrade(UpgradeData Type)
    {
        upgradeType = Type.upgradeType;
        cooldown = Type.cooldown;
    }
    protected bool Unlocked(SkillUpgradeType upgradeCheck)=> upgradeType == upgradeCheck;
}
