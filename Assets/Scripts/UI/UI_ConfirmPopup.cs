using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//———————————————弹窗式UI———————————————

public class UI_ConfirmPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action onConfirm;
    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
    }

    public void Show(string message, Action confirmAction)
    {
        messageText.text = message;
        onConfirm = confirmAction;
        gameObject.SetActive(true);
    }

    private void OnConfirmClicked()
    {
        gameObject.SetActive(false);
        onConfirm?.Invoke();
    }

    private void OnCancelClicked()
    {
        gameObject.SetActive(false);
    }
}