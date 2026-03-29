using UnityEngine;

public class UI_DeathScreen : MonoBehaviour
{
    public void SwitchToCheckPointButton()
    {
         GameSceneManager.instance.RestartScene();
    }
    public void SwitchToMainMenuButton()
    {
        GameSceneManager.instance.ChangeScene("主菜单", RespawnType.None);
    }
}
