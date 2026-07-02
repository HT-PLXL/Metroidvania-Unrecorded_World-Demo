using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

[Serializable]
public class PopupImageSetting
{
    [Tooltip("是否启用该类型的背景图片")]
    public bool enabled = false;
    [Tooltip("背景图片素材")]
    public Sprite sprite;
    [Tooltip("相对于跳字根节点的偏移（本地坐标）")]
    public Vector2 offset = Vector2.zero;
    [Tooltip("图片尺寸（本地缩放或像素）")]
    public Vector2 size = new Vector2(1f, 1f);
}

public class DamagePopup : MonoBehaviour
{
    [Header("重叠避免")]
    [SerializeField] private float randomHorizontalRange = 0.5f;// 随机水平偏移范围
    [Header("伤害设置")]
    [SerializeField] private AnimationCurve damageScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve damageAlphaCurve = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] private AnimationCurve damageVerticalCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    [SerializeField] private float damageDuration = 1.2f;
    [SerializeField] private float damageVerticalDistance = 1.5f;
    [SerializeField] private float critScaleMultiplier = 1.4f;
    [SerializeField] private Color physicalColor = Color.white;
    [SerializeField] private Color fireColor = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color iceColor = new Color(0.5f, 0.8f, 1f);
    [SerializeField] private Color lightningColor = new Color(0.8f, 0.4f, 1f);
    [SerializeField] private Color critColor = Color.yellow;

    [Header("治疗设置")]
    [SerializeField] private AnimationCurve healScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve healAlphaCurve = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] private AnimationCurve healVerticalCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0.5f));
    [SerializeField] private float healDuration = 1.0f;
    [SerializeField] private float healVerticalDistance = 1.0f;
    [SerializeField] private Color healColor = Color.green;

    [Header("背景图片配置")]
    [SerializeField] private Image backgroundImage;// 拖入预制体中的 Image 子对象
    [SerializeField] private PopupImageSetting physicalImageSetting = new PopupImageSetting();
    [SerializeField] private PopupImageSetting fireImageSetting = new PopupImageSetting();
    [SerializeField] private PopupImageSetting iceImageSetting = new PopupImageSetting();
    [SerializeField] private PopupImageSetting lightningImageSetting = new PopupImageSetting();
    [SerializeField] private PopupImageSetting critImageSetting = new PopupImageSetting();

    [SerializeField] private TextMeshProUGUI valueText;
    private Canvas canvas;
    private Coroutine animCoroutine;
    private Transform followTarget;
    public Transform FollowTarget => followTarget;
    private Vector3 followOffset;
    private float currentTotalDamage;
    private bool isCritOverall; // 如果合并中任意一次是暴击，就保持暴击外观
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);
    }

    public void Show(float damage, ElementType element, bool isCrit, Vector3 worldPosition, Transform followTarget = null, Vector3? customOffset = null)
    {
        gameObject.SetActive(true);

        currentTotalDamage = damage;
        isCritOverall = isCrit;
        Vector3 randOffset = customOffset ?? Vector3.zero;
        if (randOffset == Vector3.zero && randomHorizontalRange > 0)
            randOffset = new Vector3(UnityEngine.Random.Range(-randomHorizontalRange, randomHorizontalRange), 0, 0);

        transform.position = worldPosition + randOffset;
        this.followTarget = followTarget;
        followOffset = (followTarget != null) ? (worldPosition + randOffset) - followTarget.position : Vector3.zero;

        valueText.text = Mathf.RoundToInt(damage).ToString();

        // 文字颜色与缩放
        valueText.color = isCrit ? critColor : GetElementColor(element);
        float baseScale = isCrit ? critScaleMultiplier : 1f;
        transform.localScale = Vector3.one * baseScale;

        // 配置背景图片
        SetupBackgroundImage(element, isCrit);

        // 防御性重置 Canvas
        canvas.transform.localScale = Vector3.one;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (canvasRect != null)
            canvasRect.sizeDelta = new Vector2(2f, 1f);

        if (canvas != null && canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;

        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(DamageAnimate(baseScale));
    }

    public void ShowHeal(float healAmount, Vector3 worldPosition, Transform followTarget = null, Vector3? customOffset = null)
    {
        gameObject.SetActive(true);
        Vector3 randOffset = customOffset ?? Vector3.zero;
        if (randOffset == Vector3.zero && randomHorizontalRange > 0)
            randOffset = new Vector3(UnityEngine.Random.Range(-randomHorizontalRange, randomHorizontalRange), 0, 0);

        transform.position = worldPosition + randOffset;
        this.followTarget = followTarget;
        followOffset = (followTarget != null) ? (worldPosition + randOffset) - followTarget.position : Vector3.zero;

        valueText.text = "+" + Mathf.RoundToInt(healAmount).ToString();
        valueText.color = healColor;
        transform.localScale = Vector3.one;

        // 治疗不显示背景图片
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);

        canvas.transform.localScale = Vector3.one;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (canvasRect != null)
            canvasRect.sizeDelta = new Vector2(2f, 1f);

        if (canvas != null && canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;

        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(HealAnimate());
    }

    private void SetupBackgroundImage(ElementType element, bool isCrit)
    {
        if (backgroundImage == null) return;

        PopupImageSetting setting = null;
        if (isCrit)
            setting = critImageSetting;
        else
        {
            switch (element)
            {
                case ElementType.Fire: setting = fireImageSetting; break;
                case ElementType.Ice: setting = iceImageSetting; break;
                case ElementType.Lightning: setting = lightningImageSetting; break;
                default: setting = physicalImageSetting; break;
            }
        }

        if (setting == null || !setting.enabled || setting.sprite == null)
        {
            backgroundImage.gameObject.SetActive(false);
            return;
        }

        backgroundImage.gameObject.SetActive(true);
        backgroundImage.sprite = setting.sprite;

        // 设置位置（相对于当前 GameObject 的本地坐标）
        RectTransform bgRect = backgroundImage.rectTransform;
        bgRect.anchorMin = bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.anchoredPosition = setting.offset;
        bgRect.sizeDelta = setting.size;

    }

    private IEnumerator DamageAnimate(float baseScale)
    {
        float timer = 0;
        Vector3 startWorldPos = transform.position;

        while (timer < damageDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / damageDuration;

            Vector3 currentBasePos = GetCurrentBasePosition(startWorldPos);
            Vector3 pos = currentBasePos + Vector3.up * (damageVerticalCurve.Evaluate(progress) * damageVerticalDistance);
            transform.position = pos;

            float scale = Mathf.Clamp(damageScaleCurve.Evaluate(progress) * baseScale, 0f, 5f);
            transform.localScale = Vector3.one * scale;

            Color c = valueText.color;
            c.a = damageAlphaCurve.Evaluate(progress);
            valueText.color = c;

            // 同步背景图片透明度
            if (backgroundImage != null && backgroundImage.gameObject.activeSelf)
            {
                Color bgColor = backgroundImage.color;
                bgColor.a = c.a;
                backgroundImage.color = bgColor;
            }

            yield return null;
        }

        DamagePopupManager.Instance.ReturnToPool(this);
    }

    private IEnumerator HealAnimate()
    {
        float timer = 0;
        Vector3 startWorldPos = transform.position;

        while (timer < healDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / healDuration;

            Vector3 currentBasePos = GetCurrentBasePosition(startWorldPos);
            Vector3 pos = currentBasePos + Vector3.up * (healVerticalCurve.Evaluate(progress) * healVerticalDistance);
            transform.position = pos;

            float scale = Mathf.Clamp(healScaleCurve.Evaluate(progress), 0f, 5f);
            transform.localScale = Vector3.one * scale;

            Color c = valueText.color;
            c.a = healAlphaCurve.Evaluate(progress);
            valueText.color = c;

            yield return null;
        }

        DamagePopupManager.Instance.ReturnToPool(this);
    }

    private Vector3 GetCurrentBasePosition(Vector3 originalWorldPos)
    {
        if (followTarget != null)
            return followTarget.position + followOffset;
        return originalWorldPos;
    }

    private Color GetElementColor(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire: return fireColor;
            case ElementType.Ice: return iceColor;
            case ElementType.Lightning: return lightningColor;
            default: return physicalColor;
        }
    }

    public void ResetState()
    {
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
        followTarget = null;
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    public void AddDamage(float additionalDamage, bool additionalIsCrit)
    {
        currentTotalDamage += additionalDamage;
        if (additionalIsCrit) isCritOverall = true;
        valueText.text = Mathf.RoundToInt(currentTotalDamage).ToString();
        if (isCritOverall) valueText.text += "!";
        valueText.color = isCritOverall ? critColor : valueText.color;
    }
}