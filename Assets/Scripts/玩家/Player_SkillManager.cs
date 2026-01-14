using UnityEngine;

//这个脚本用于管理玩家的技能
public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        shard = GetComponentInChildren<Skill_Shard>();
    }
    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.Dash:return dash;//返回实例（技能）
            case SkillType.TimeShard: return shard;

            default:
                Debug.Log($"尚未实现的技能{type}");
                return null;
        }
    }
}
