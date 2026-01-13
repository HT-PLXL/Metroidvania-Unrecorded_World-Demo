using UnityEngine;

//这个脚本用于管理玩家的技能
public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash { get; private set; }
    public Skill_Crystal crystal { get; private set; }
    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        crystal = GetComponentInChildren<Skill_Crystal>();
    }
    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.Dash:return dash;
            default:
                Debug.Log($"尚未实现的技能{type}");
                return null;
        }
    }
}
