using UnityEngine;

// 当背景离开摄像机视野时，重新定位到另一侧，实现无限循环
public class BackgroundLoop : MonoBehaviour
{
    private Camera mainCamera;
    private float bgWidth;    // 背景宽度
    public bool needDebugBgWidth;
    void Start()
    {
        mainCamera = Camera.main;
        GetBgWidth();
    }

    void Update()
    {
        BgMove();
    }

    void GetBgWidth()
    {
        // 获取背景实际宽度
        SpriteRenderer spriterenderer = GetComponent<SpriteRenderer>();
        bgWidth = spriterenderer.bounds.size.x;

        if (needDebugBgWidth)
            Debug.Log(transform.parent.name + "精灵图片的宽度: " + bgWidth);
    }

    void BgMove()
    {
        // 背景与摄像机的水平距离
        float distance = mainCamera.transform.position.x - transform.position.x;

        // 如果背景完全离开了摄像机视野，距离超过一个背景宽度，把背景移到另一侧
        if (Mathf.Abs(distance) > bgWidth)
        {
            transform.position += Vector3.right * bgWidth * 2 * Mathf.Sign(distance);
        }
    }
}
