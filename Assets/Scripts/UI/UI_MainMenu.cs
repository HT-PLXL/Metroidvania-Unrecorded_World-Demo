using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    private void Start()
    {
        transform.root.GetComponentInChildren<UI_FadeScreen>().DoFadeIn();//主菜单初始化时黑色渐变透明
        //AudioManager.instance.PlayBGM("主菜单音乐列表");
    }
    public void Play()
    {
        AudioManager.instance.PlayGlobalSFX("点击按钮");
        GameSceneManager.instance.ContinuePlay();
    }
    public void Quit()
    {
        Application.Quit();
    }
}
