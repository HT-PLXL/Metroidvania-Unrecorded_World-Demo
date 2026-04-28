public class Skill_DoubleJump : Skill_Base
{
    public override bool CanUseSkill()
    {
        return upgradeType != SkillUpgradeType.None;
    }
}