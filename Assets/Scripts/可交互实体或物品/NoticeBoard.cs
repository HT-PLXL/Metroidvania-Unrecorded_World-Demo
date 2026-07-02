using UnityEngine;

public class NoticeBoard : Object_NPC, IInteractable
{
    [TextArea(3, 5)] public string info = "默认提示文本";

    protected override void Update()
    {
        HandleToolTipFloat();
    }
    public void Interact()
    {
        if (ui == null || ui.infoUI == null) return;
        ui.infoUI.ShowInfo(info, this);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        if (ui != null && ui.infoUI != null)
            ui.infoUI.Hide(this);
    }
}