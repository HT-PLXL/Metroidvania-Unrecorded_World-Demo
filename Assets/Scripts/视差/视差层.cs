using UnityEngine;
[System.Serializable]
public class 视差层
{
    [SerializeField] private Transform 图片;
    [SerializeField] private float 视差倍率;
    [SerializeField] private float 图片宽度偏移量 =10;


    private float 图片宽度;//图片使用宽高比
    private float 图片半宽;
    public void 计算图片宽度()
    {
        图片宽度 = 图片.GetComponent<SpriteRenderer>().bounds.size.x;
        图片半宽 = 图片宽度 / 2;
        //bounds:包围物体的盒子
    }

    public void Move(float 移动距离)
    {
        //背景.position = 背景.position + new Vector3(移动距离 * 视差倍率, 0, 0);
        图片.position += Vector3.right *(移动距离 * 视差倍率);
    }
    public void 循环背景(float 摄像机左边缘,float 摄像机右边缘)
    {
        //float 图片右边缘 = (背景.position.x + 图片宽度 / 2 - 图片宽度偏移量);
        //float 图片左边缘 = (背景.position.x - 图片宽度 / 2 - 图片宽度偏移量);
        float 图片右边缘 = (图片.position.x + 图片半宽) - 图片宽度偏移量;
        float 图片左边缘 = (图片.position.x - 图片半宽) + 图片宽度偏移量;

        if (图片右边缘 < 摄像机左边缘)
            图片.position += Vector3.right * 图片宽度;
        if (图片左边缘 > 摄像机右边缘)
            图片.position += Vector3.right * -图片宽度;

    }
}
