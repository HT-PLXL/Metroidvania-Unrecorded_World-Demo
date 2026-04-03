using UnityEngine;
[System.Serializable]
public class ParallaxPicture
{
    [SerializeField] private Transform picture;
    [SerializeField] private float parallaxMultiplier;//视差倍率
    [SerializeField] private float pictureWidthOffset =10;


    private float pictureWidth;//图片使用宽高比
    private float pictureHalfWidth;
    public void CalculatePictureWidth()
    {
        pictureWidth = picture.GetComponent<SpriteRenderer>().bounds.size.x;
        pictureHalfWidth = pictureWidth / 2;
        //bounds:包围物体的盒子
    }

    public void Move(float moveDistance)
    {
        picture.position += Vector3.right *(moveDistance * parallaxMultiplier);
    }
    public void LoopBackground(float cameraLeftEdge,float cameraRightEdge)
    {
        float pictureRightEdge = (picture.position.x + pictureHalfWidth) - pictureWidthOffset;
        float pictureLeftEdge = (picture.position.x - pictureHalfWidth) + pictureWidthOffset;

        if (pictureRightEdge < cameraLeftEdge)
            picture.position += Vector3.right * pictureWidth;
        if (pictureLeftEdge > cameraRightEdge)
            picture.position += Vector3.right * -pictureWidth;

    }
}
