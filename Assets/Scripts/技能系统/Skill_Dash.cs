using UnityEngine;

public class Skill_Dash : Skill_Base
{
    public void OnstartEffect()
    {
        if (Unlocked(SkillUpgradeType.Dash_CloneOnStart)|| Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
        {
            CreateClone();
        }
        if (Unlocked(SkillUpgradeType.Dash_ShardOnStart) || Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
        {
            CreateShard();
        }
    }
    public void OnEndEffect()
    {
        if (Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
        {
            CreateClone();
        }
        if (Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
        {
            CreateShard();
        }
    }
    private  void CreateShard()
    {
        skillManager.shard.CreateOriginalShard();
    }
    private void CreateClone()
    {
        skillManager.clone.CreateClone();
    }
}
