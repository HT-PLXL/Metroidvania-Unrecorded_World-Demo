using Unity.VisualScripting;
using UnityEngine;

public class 视差背景 : MonoBehaviour
{
    private Camera 主摄像机;
    private float 上次摄像机X位置;
    private float 摄像机半宽度;

    [SerializeField] private 视差层[] 背景图层;

    private void Awake()
    {
        主摄像机 = Camera.main;
        摄像机半宽度 = 主摄像机.orthographicSize * 主摄像机.aspect;
        //orthographicSize，正交尺寸，定义了相机纵向（垂直方向）的半高度
        //aspect，长宽比，此处是宽高比
        计算图片宽度();
    }
    private void FixedUpdate()
    {
        float 当前摄像机X位置 = 主摄像机.transform.position.x;
        float 移动距离 = 当前摄像机X位置 - 上次摄像机X位置;
        上次摄像机X位置 = 当前摄像机X位置;
        //第一帧移动距离为0，但给“上次摄像机X位置”确定了位置

        float 摄像机左边缘 = 当前摄像机X位置 - 摄像机半宽度;
        float 摄像机右边缘 = 当前摄像机X位置 + 摄像机半宽度;

        foreach (视差层 图片 in 背景图层)
        {
            图片.Move(移动距离);
            图片.循环背景(摄像机左边缘, 摄像机右边缘);
        }
    }
    private void 计算图片宽度()
    { 
        foreach (视差层 图片 in 背景图层)
            图片.计算图片宽度();
    }


}
