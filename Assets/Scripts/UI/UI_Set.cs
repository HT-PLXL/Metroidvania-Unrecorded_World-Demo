using UnityEngine;
using UnityEngine.UI;

public class UI_Set : MonoBehaviour
{
    private Player player;
    [SerializeField] private Toggle hpBarToggle;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        hpBarToggle.onValueChanged.AddListener(OnToggleMiniHpBar);
    }
    private void OnToggleMiniHpBar(bool isOpen)
    {
        player.hp.EnableHpBar(isOpen);
    }
    public void ToggleToMainMenuButton()
    {
        GameSceneManager.instance.ChangeScene("主菜单", RespawnType.None);
    }
}
