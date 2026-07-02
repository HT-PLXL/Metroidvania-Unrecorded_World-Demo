using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BOOSHpBar : MonoBehaviour
{
    private EntityHp entityHp;
    public TextMeshProUGUI bossNameText;
    public Image hpbar;
    public Image delayHpbar;
    private Coroutine delayCoroutine;

    public void SetTarget(Entity entity, string bossName)
    {
        if (entityHp != null)
            entityHp.OnHpUpdate -= UpdateBoosHpBar;

        entityHp = entity.GetComponent<EntityHp>();

        entityHp.OnHpUpdate += UpdateBoosHpBar;
        gameObject.SetActive(true);

        // 设置名字
        if (bossNameText != null)
            bossNameText.text = bossName;

        UpdateBoosHpBar(); // 立即刷新初始血量
    }

    private void OnDisable()
    {
        if (entityHp != null)
            entityHp.OnHpUpdate -= UpdateBoosHpBar;
        if (delayCoroutine != null)
            StopCoroutine(delayCoroutine);
    }

    public void UpdateBoosHpBar()
    {
        if (entityHp == null) return;

        float targetFill = entityHp.GetHpPercent();
        hpbar.fillAmount = targetFill;

        if (delayCoroutine != null)
            StopCoroutine(delayCoroutine);
        delayCoroutine = StartCoroutine(DelayHpBarLerp(targetFill));
    }

    private IEnumerator DelayHpBarLerp(float targetFill)
    {
        yield return new WaitForSeconds(0.2f);

        float startFill = delayHpbar.fillAmount;
        float duration = 0.25f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            delayHpbar.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        delayHpbar.fillAmount = targetFill;
        delayCoroutine = null;
    }
}

    //IEnumerator DelayHpBarLerp(float targetFill)
    //{
    //    yield return new WaitForSeconds(0.2f);

    //    float startFill = delayHpbar.fillAmount;
    //    for (float t = 0; t < 0.25f; t += Time.deltaTime)
    //    {
    //        delayHpbar.fillAmount = Mathf.Lerp(startFill, targetFill, t / 0.25f);//t / 0.25f整体作为插值因子，t=0.25s时因子为1，插值完成
    //        yield return null;
    //    }
    //    delayHpbar.fillAmount = targetFill;
    //}
