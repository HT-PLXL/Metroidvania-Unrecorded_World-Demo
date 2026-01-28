using UnityEngine;
//子类挂载在技能容器上，下方声明的参数用来在技能容器修改对应技能实体脚本声明的参数的值
public class Skill_Base : MonoBehaviour
{
    public Player player { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public DamageScaleFactor damageScale { get; private set; }
    [Header("当前技能详情")]
    [SerializeField][cn("冷却时间")] protected float cooldown;
    [SerializeField][cn("技能种类")] protected SkillType skillType;
    [SerializeField][cn("技能升级种类")] protected SkillUpgradeType upgradeType;
    private float lastTimeUsed;

    protected virtual void Awake()
    {
        player = GetComponentInParent<Player>();
        skillManager = GetComponentInParent<Player_SkillManager>();
        lastTimeUsed = lastTimeUsed - cooldown;//初始/重生时重置冷却
        damageScale = new DamageScaleFactor();//暂时没做技能数据，所以需要初始化一下
    }
    public virtual void TryUseSkill()
    {

    }
    protected bool OnCooldown() => Time.time < lastTimeUsed + cooldown;
    public void setSkillOnCooldown() => lastTimeUsed = Time.time;//使其进入冷却
    public void ReduceCooldown(float cooldownReduction) => lastTimeUsed = lastTimeUsed - cooldownReduction;//减少冷却时间
    public void ResetCooldown() => lastTimeUsed = Time.time - cooldown;//重置冷却时间

    public virtual bool CanUseSkill()
    {
        if (upgradeType == SkillUpgradeType.None)
            return false;
        if (OnCooldown())
        {
            Debug.Log("技能冷却中");
            return false;
        }
        return true;
    }
    public void SetSkillUpgrade(UpgradeData Type)//更新升级后数据
    {
        upgradeType = Type.upgradeType;
        cooldown = Type.cooldown;
        damageScale = Type.damageScale;
        ResetCooldown();
    }
    protected bool Unlocked(SkillUpgradeType upgradeCheck) => upgradeType == upgradeCheck;
    public SkillUpgradeType GetUpgradeType() => upgradeType;
    public SkillType GetSkillType() => skillType;
}
