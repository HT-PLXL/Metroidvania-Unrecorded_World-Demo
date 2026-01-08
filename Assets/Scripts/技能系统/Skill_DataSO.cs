using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/技能数据", fileName = "技能数据")]

public class Skill_DataSO : ScriptableObject
{
    public int cost;

    [Header("技能描述")]
    public string skillName;
    [TextArea]
    public string description;
    public Sprite icon;

}
