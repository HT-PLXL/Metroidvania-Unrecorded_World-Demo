using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Camera mainCamera;
    private float lastCameraXPosition;
    private float cameraHalfWidth;

    [SerializeField] private ParallaxPicture[] backgroundPictures;

    private void Awake()
    {
        mainCamera = Camera.main;
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        //orthographicSize，正交尺寸，定义了相机纵向（垂直方向）的半高度
        //aspect，长宽比，此处是宽高比
        CalculatePictureWidth();
    }
    private void FixedUpdate()
    {
        float currentCameraXPosition = mainCamera.transform.position.x;
        float moveDistance = currentCameraXPosition - lastCameraXPosition;
        lastCameraXPosition = currentCameraXPosition;
        //第一帧移动距离为0，但给“lastCameraXPosition”确定了位置

        float cameraLeftEdge = currentCameraXPosition - cameraHalfWidth;
        float cameraRightEdge = currentCameraXPosition + cameraHalfWidth;

        foreach (ParallaxPicture Picture in backgroundPictures)
        {
            Picture.Move(moveDistance);
            Picture.LoopBackground(cameraLeftEdge, cameraRightEdge);
        }
    }
    private void CalculatePictureWidth()
    { 
        foreach (ParallaxPicture Picture in backgroundPictures)
            Picture.CalculatePictureWidth();
    }


}
