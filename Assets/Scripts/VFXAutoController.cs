using System.Collections;
using UnityEngine;

public class VFXAutoController : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField][cn("自动销毁")] private bool autoDestroy = true;
    [SerializeField][cn("销毁延迟")] private float destroydelay = 1     ;
    [Header("淡化效果")]
    [SerializeField][cn("允许淡化？")]private bool canFade;
    [SerializeField][cn("淡化速度")] private float fadeSpeed=1;
    [Header("随机位置")]
    [SerializeField][cn("是否启用随机偏移")] private bool enableOffset = true;
    [SerializeField][cn("是否启用随机旋转")] private bool enableRotation = true;
    [Space]
    [SerializeField][cn("最小旋转")] private float minRotation = 0;
    [SerializeField][cn("最大旋转")] private float maxRotation = 360;
    [Space]
    [SerializeField][cn("x最大偏移")] private float xMaxOffset = 0.5f;
    [SerializeField][cn("x最小偏移")] private float xMinOffset = -0.5f;
    [Space]
    [SerializeField][cn("y最大偏移")] private float yMaxOffset = 0.5f;
    [SerializeField][cn("y最小偏移")] private float yMinOffset = -0.5f;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }
    private void Start()
    {
        if (canFade)
            StartCoroutine(FadeCo());

        UseOffset();
        UseRotation();

        if (autoDestroy)
            Destroy(gameObject, destroydelay);
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
    private void UseOffset()
    {
        if (enableOffset == false)
            return;
        float xOffset = Random.Range(xMinOffset, xMaxOffset);
        float yOffset = Random.Range(yMinOffset, yMaxOffset);

        transform.position = transform.position + new Vector3(xOffset, yOffset);
    }
    private void UseRotation()
    {
        if (enableRotation == false)
            return;

        float zRotation = Random.Range(minRotation, maxRotation);
        transform.Rotate(0, 0, zRotation);
    }

}
