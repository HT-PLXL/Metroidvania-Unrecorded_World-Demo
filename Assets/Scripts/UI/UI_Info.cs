using TMPro;
using UnityEngine;

public class UI_Info : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;
    private NoticeBoard User;

    public bool IsActive => gameObject.activeSelf;

    public void ShowInfo(string message, NoticeBoard User)
    {
        if (IsActive)
        {
            gameObject.SetActive(false);
            this.User = null;
            return;
        }

        this.User = User;
        infoText.text = message;
        gameObject.SetActive(true);
    }

    public void Hide(NoticeBoard User)
    {
        if(this.User == User)
            gameObject.SetActive(false);
    }
}