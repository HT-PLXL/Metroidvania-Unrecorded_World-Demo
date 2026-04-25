using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_BOOSHpBar : MonoBehaviour
{
    private EntityHp entityHp;
    public Image hpbar, delayHpbar;
    private Coroutine co;
    public bool beUsed { get; set; }
    public void SetHpbar(Entity entity)
    {
        entityHp = entity.GetComponent<EntityHp>();
        gameObject.SetActive(true);
        beUsed = true;
        //UpdateBOOSHpbar();
    }
    public void UpdateBOOSHpbar()
    {
        if (beUsed)
        {
            float targetFill = entityHp.GetHpPercent();
            hpbar.fillAmount = targetFill;
            if (co != null)
            {
                StopCoroutine(co);
            }
            co = StartCoroutine(DelayHpBarLerp(targetFill));
        }
    }

    IEnumerator DelayHpBarLerp(float targetFill)
    {
        yield return new WaitForSeconds(0.2f);

        float startFill = delayHpbar.fillAmount;
        for (float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            delayHpbar.fillAmount = Mathf.Lerp(startFill, targetFill, t / 0.25f);//t / 0.25f整体作为插值因子，t=0.25s时因子为1，插值完成
            yield return null;
        }
        delayHpbar.fillAmount = targetFill;
    }
}
