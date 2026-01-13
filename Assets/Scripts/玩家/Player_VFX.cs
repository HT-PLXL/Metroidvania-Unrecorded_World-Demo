using System.Collections;
using UnityEngine;
//此类继承父类视觉效果，同时实现残影效果
public class Player_VFX : Entity_VFX
{
    [Header("残影效果")]
    [Range(0.01f, 0.2f)]
    [SerializeField][cn("间隔")] private float echoInterval = 0.05f;
    [SerializeField][cn("残影预制体")] private GameObject echoPrehab;
    private Coroutine echoCo;

    public void DoEchoEffect(float duration)
    {
        if (echoCo != null)
            StopCoroutine(echoCo);
        echoCo = StartCoroutine(echoEffectCo(duration));
    }
    private IEnumerator echoEffectCo(float duration)
    {
        float time = 0;
        while(time < duration)
        {
            CreateEcho();
            yield return new WaitForSeconds(echoInterval);
            time = time + echoInterval;
        }
    }
    private void CreateEcho()
    {
        GameObject echo = Instantiate(echoPrehab, transform.position, transform.rotation);
        echo.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;
    }
}
