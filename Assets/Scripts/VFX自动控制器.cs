using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class VFX自动控制器 : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private bool 自动销毁 = true;
    [SerializeField] private float 销毁延迟 = 1;
    [Header("淡化效果")]
    [SerializeField][cn("允许淡化？")]private bool canFade;
    [SerializeField][cn("淡化速度")] private float fadeSpeed=1;

    [Header("随机位置")]
    [SerializeField] private bool 是否启用随机偏移 = true;
    [SerializeField] private bool 是否启用随机旋转 = true;
    [Space]
    [SerializeField] private float 最小旋转 = 0;
    [SerializeField] private float 最大旋转 = 360;
    [Space]
    [SerializeField] private float x最大偏移 = 0.5f;
    [SerializeField] private float x最小偏移 = -0.5f;
    [Space]
    [SerializeField] private float y最大偏移 = 0.5f;
    [SerializeField] private float y最小偏移 = -0.5f;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }
    private void Start()
    {
        if (canFade)
            StartCoroutine(FadeCo());

        启用随机偏移();
        启用随机旋转();

        if (自动销毁)
            Destroy(gameObject, 销毁延迟);
    }
    private IEnumerator FadeCo()
    {
        Color targetColor = Color.white;
        while (targetColor.a > 0)
        {
            targetColor.a = targetColor.a - (fadeSpeed * Time.deltaTime);
            sr.color = targetColor;
            yield return null;
        }
        sr.color = targetColor;//透明度<0时立刻终止循环，会缺失一次赋值，因此需要补足
    }
    private void 启用随机偏移()
    {
        if (是否启用随机偏移 == false)
            return;
        float x偏移 = Random.Range(x最小偏移, x最大偏移);
        float y偏移 = Random.Range(y最小偏移, y最大偏移);

        transform.position = transform.position + new Vector3(x偏移, y偏移);
    }
    private void 启用随机旋转()
    {
        if (是否启用随机旋转 == false)
            return;

        float 沿z轴旋转 = Random.Range(最小旋转, 最大旋转);
        transform.Rotate(0, 0, 沿z轴旋转);
    }

}
