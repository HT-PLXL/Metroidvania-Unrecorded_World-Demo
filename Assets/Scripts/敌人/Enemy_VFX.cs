using UnityEngine;

public class Enemy_VFX : Entity_VFX
{
    [Header("反击窗口")]
    [SerializeField][cn("攻击警示预制体")] private GameObject attackWarning;

    public void PlayAttackWarning(bool enable)
    {
        attackWarning.SetActive(enable);
    }
    public override Color GetHitVfxColor(Transform target)
    {
        ColorUtility.TryParseHtmlString("#ABABAB", out Color color);
        return color;
    }
}
