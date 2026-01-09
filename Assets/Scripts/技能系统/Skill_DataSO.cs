using UnityEngine;
//此脚本实现技能数据可自定义对象
[CreateAssetMenu(menuName = "游戏设置/技能数据", fileName = "技能数据")]

public class Skill_DataSO : ScriptableObject
{

    [Header("技能描述")]
    public string skillName;
    public int cost;
    [TextArea]
    public string description;
    public Sprite icon;

}
