using UnityEngine;

public class Object_Signpost : Object_NPC, IInteractable
{
    [SerializeField] private string transferToScene;
    [Space]
    [SerializeField] private RespawnType signpost;//当前点种类
    [SerializeField] private RespawnType connectedSignpost;//连接点种类
    [SerializeField] private Transform respwanPoint;//传送后具体位置
    //[SerializeField] private bool canBeTriggered = true;
    //[SerializeField] private LayerMask whatIsPlayer;
    public RespawnType GetRespawnType() => signpost;
    public Vector3 GetPositionAndSetTriggerFalse()
    {
        //canBeTriggered = false;
        return respwanPoint == null ? transform.position : respwanPoint.position;//选择传送后位置
    }
    private void OnValidate()
    {
        gameObject.name = "路标 - " + signpost.ToString() + " - " + transferToScene;

        if (signpost == RespawnType.Enter)
            connectedSignpost = RespawnType.Exit;

        if (signpost == RespawnType.Exit)
            connectedSignpost = RespawnType.Enter;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (canBeTriggered == false)
    //        return;
    //    if (((1 << collision.gameObject.layer) & whatIsPlayer) != 0)
    //        GameSceneManager.instance.ChangeScene(transferToScene, connectedSignpost);
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    canBeTriggered = true;
    //}

    public void Interact()
    {
        GameSceneManager.instance.ChangeScene(transferToScene, connectedSignpost);
    }
}
