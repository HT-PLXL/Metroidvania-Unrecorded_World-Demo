using UnityEngine;

public class 敌人视觉效果 : 实体视觉效果
{
    [Header("反击窗口")]
    [SerializeField] private GameObject 攻击警告;

    public void 可弹出攻击警告(bool enable)
    {
        攻击警告.SetActive(enable);
        //SetActive用于激活/禁用游戏对象，值：true/false
    }
}
