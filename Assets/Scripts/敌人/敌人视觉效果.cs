using UnityEngine;

public class 敌人视觉效果 : Entity_VFX
{
    [Header("反击窗口")]
    [SerializeField] private GameObject 攻击警告;

    public void 可弹出攻击警告(bool enable)
    {
        攻击警告.SetActive(enable);
        //SetActive用于激活/禁用游戏对象，值：true/false
    }
    public override Color GetHitVfxColor()
    {
        ColorUtility.TryParseHtmlString("#ABABAB", out Color color);
        return color;
    }
}
