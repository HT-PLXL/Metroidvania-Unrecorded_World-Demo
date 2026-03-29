using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_FadeScreen : MonoBehaviour
{
    private Image fadeImage;
    private UI_MainMenu mainMenu;
    public Coroutine fadeEffectCo { get; private set; }
    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        mainMenu = FindAnyObjectByType<UI_MainMenu>();
        if (mainMenu != null)
            fadeImage.color = new Color(0, 0, 0, 1);//运行时主菜单默认黑，用于渐变
    }
    public void DoFadeIn(float duration = 1) // 黑=>透明
    {
        fadeImage.color = new Color(0, 0, 0, 1);
        FadeEffect(0f, duration);
    }
    public void DoFadeOut(float duration = 1) // 透明=>黑
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        FadeEffect(1f, duration);
    }
    private void FadeEffect(float targetAlpha, float duration)
    {
        if (fadeEffectCo != null)
            StopCoroutine(fadeEffectCo);

        fadeEffectCo = StartCoroutine(FadeEffectCo(targetAlpha, duration));
    }
    private IEnumerator FadeEffectCo(float targetAlpha, float duration)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < duration)
        {
            time = time + Time.deltaTime;
            var color = fadeImage.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);//插值（初始值，结束值，百分比）
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
    }
}
