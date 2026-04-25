using UnityEngine;

//视差层的每帧移动
public class ParallaxLayer : MonoBehaviour
{
    private Transform mainCamera;    // 主相机Tf
    private Vector3 lastPosition;    // 相机位置

    [SerializeField] private float xSpeed = 1f;    // 视差效果参数
    [SerializeField] private float ySpeed = 1f;    // 视差效果参数

    private void Start()
    {
        mainCamera = Camera.main.transform;    // 获取相机
        lastPosition = mainCamera.position;   // 相机位置
    }

    private void LateUpdate()
    {
        ParallaxMove();    // 视差移动
    }

    private void ParallaxMove()
    {
        //float deltaX = mainCamera.position.x - lastPosition.x;    // deltaX: 相机在过去一帧移动的距离
        //transform.position += new Vector3(deltaX * xSpeed, 0, 0);    // 更新背景位置

        Vector3 delta = mainCamera.position - lastPosition;    // deltaX: 相机在过去一帧移动的距离
        transform.position += new Vector3(delta.x * xSpeed, delta.y * ySpeed, 0);    // 更新背景位置



        lastPosition = mainCamera.position;                       // 更新
    }
}
