using UnityEngine;

public class Player_VFX : Entity_VFX
{
    public void CreateEffectForTarget(GameObject effectPrefab ,Transform target)//为目标应用特效
    {
        Instantiate(effectPrefab, target.position ,Quaternion.identity);
    }

    public override Color GetHitVfxColor(Transform target)
    {
        if (target.CompareTag("BOOS"))
        {
            ColorUtility.TryParseHtmlString("#C6854E", out Color color1);
            return color1;
        }
        ColorUtility.TryParseHtmlString("#ABABAB", out Color color2);
        return color2;
    }
}
