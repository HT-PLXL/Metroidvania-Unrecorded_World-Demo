using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string bgmGroupName;
    private void Start()
    {
        AudioManager.instance.PlayBGM(bgmGroupName);
    }
}
