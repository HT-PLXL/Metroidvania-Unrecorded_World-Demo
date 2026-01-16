using Unity.VisualScripting;
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
        SkillManager.shard.CreateOriginalShard();
    }
    private 
        void CreateClone()
    {
        Debug.Log("create time echo!");
    }
}
