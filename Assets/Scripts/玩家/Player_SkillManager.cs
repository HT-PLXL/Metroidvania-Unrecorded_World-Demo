using UnityEngine;

//这个脚本用于管理玩家的技能，写新技能种类需要来这里更新
public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_ThrowSword throwSword { get; private set; }
    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        shard = GetComponentInChildren<Skill_Shard>();
        throwSword = GetComponentInChildren<Skill_ThrowSword>();
    }
    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.Dash:return dash;//返回实例（技能）
            case SkillType.魔能晶体: return shard;

            default:
                Debug.Log($"尚未实现的技能{type}");
                return null;
        }
    }
}
