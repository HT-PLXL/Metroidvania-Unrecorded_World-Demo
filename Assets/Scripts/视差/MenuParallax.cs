using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    [cn("偏移比例")]public float offsetMultiplier = 1f;
    [cn("平滑过渡时间")] public float smoothTime = .3f;

    private Vector2 startPosition;
    private Vector3 velocity;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);//屏幕分辨率像素是不固定的，转化为统一的视点坐标（0,1）
        transform.position = Vector3.SmoothDamp(transform.position, startPosition + (offset * offsetMultiplier), ref velocity, smoothTime);//带阻尼的平滑过渡
    }
}